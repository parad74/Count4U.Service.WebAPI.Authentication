using Count4U.Service.Core.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Count4U.Service.Common;
//using Unity;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Count4U.Service.Shared;
using Service.Filter;
using Monitor.Service.Shared;
using Monitor.Service.Shared.Settings;
using Microsoft.Extensions.Options;
using Monitor.Service.Model.Settings;
using Microsoft.AspNetCore.Http.Features;

namespace Count4U.Service.Core.Server
{
	public class StartupAuthenticationWebAPI
    {
        public StartupAuthenticationWebAPI(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();   //��������� ��� ������ ������ ������������ ����������
			services.AddMvc()			 //add
			.AddNewtonsoftJson(); //add

			services.AddDistributedMemoryCache();      //https://metanit.com/sharp/aspnet5/2.11.php
			services.AddSession();

			

			services.AddHttpContextAccessor();   // �� ������ ��������� ������ IHttpContextAccessor � ������������. ���� ������ IHttpContextAccessor �� ����� �������� User, �� ���� ��� ������ � ������� HttpContext, ������� ����� �������� User. 

			//services.AddSingleton(new LoggerFactory()
			//   .AddNLog()
			//   );
			services.AddLogging();
			services.AddLogging(config =>
			{
				// clear out default configuration
				config.ClearProviders();

				config.AddConfiguration(Configuration.GetSection("Logging"));
				config.AddDebug();
				config.AddEventSourceLogger();

				if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ==
				Microsoft.Extensions.Hosting.Environments.Development)
				{
					config.AddConsole();
				}
			});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Count4U.Service.WebAPI.Authentication",
					Description = "COUNT4U.SERVICE Authentication WEB API "
				});
				//// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
				c.CustomSchemaIds(i => i.FullName);
			});

			//����������� ������ � ������� � ������
			//var authConnection = SetupSqliteInMemoryConnection();
			//services.AddDbContext<ExtraAuthorizeDbContext>(options => options.UseSqlite(authConnection));

			//var authConnection = SetupSqliteInMemoryConnection();
			//services.AddDbContext<CustomerContext>(options => options.UseSqlite(authConnection));
			//services.AddDbContext<CustomerContext>(options => options.UseSqlite("DataSource=:memory:"));

			  //services.AddDbContext<ApplicationDbContext>(options => 
     //   options.UseSqlite("Data Source=userc4u.db"));

			//Setting up ApplicationDbContext
			 //services.AddDbContext<ApplicationDbContext>(options =>  
    //        {  
    //            // for in memory database  
    //            options.UseInMemoryDatabase("MemoryBaseDataBase");  
    //        });  
			services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlite(@"Data Source=App_Data\userc4u.db"));
		//	options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
	   			services.AddDefaultIdentity<ApplicationUser>(opts => {    //TODO �������� ���� ����� ��������� �� 	IdentityUser � add IdentityRole	   services.AddDefaultIdentity<IdentityUser, IdentityRole>()  
				opts.Password.RequiredLength = 5;   // ����������� �����
				opts.Password.RequireNonAlphanumeric = false;   // ��������� �� �� ���������-�������� �������
				opts.Password.RequireLowercase = false; // ��������� �� ������� � ������ ��������
				opts.Password.RequireUppercase = false; // ��������� �� ������� � ������� ��������
				opts.Password.RequireDigit = false; // ��������� �� �����
				opts.User.RequireUniqueEmail = true;    // ���������� email
														//opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz"; // ���������� �������
			})
					  .AddRoles<IdentityRole>()
					  .AddEntityFrameworkStores<ApplicationDbContext>();    //to do add Password.RequiredLength � �.�. ��� 895  and User.RequiredLength ��� 901 


			//Setting up Jwt Authentication
			//services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)   ����

			// ��  public async Task<IActionResult> Login([FromBody] LoginModel login) ���� ��������
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
							ValidateIssuer = false,//true,
							ValidIssuer = Configuration["JwtIssuer"],
							ValidateAudience = false,//true, �� ����� ������������
							ValidAudience = Configuration["JwtAudience"],
							ValidateLifetime = true,
							ValidateIssuerSigningKey = true,
							//ClockSkew = TimeSpan.FromSeconds(10),      //��������� ������ ����������� ��� �������� ������� ����� ������
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
						};
                    });


			services.AddAuthorization(config =>
			{
				config.AddPolicy(UserPolicy.IsAdmin, UserPolicy.IsAdminPolicy());
				config.AddPolicy(UserPolicy.IsUser, UserPolicy.IsUserPolicy());
				config.AddPolicy(UserPolicy.IsOwner, UserPolicy.IsOwnerPolicy());
				config.AddPolicy(UserPolicy.IsManager, UserPolicy.IsManagerPolicy());
				config.AddPolicy(UserPolicy.IsMonitor, UserPolicy.IsMonitorPolicy());
				config.AddPolicy(UserPolicy.IsContext, UserPolicy.IsContextPolicy());
				config.AddPolicy(UserPolicy.IsWorker, UserPolicy.IsWorkerPolicy());
				config.AddPolicy(UserPolicy.IsProfile, UserPolicy.IsProfilePolicy());
				config.AddPolicy(UserPolicy.HaveInventorCode, UserPolicy.HaveInventorCodePolicy());
			});

			//services.AddSingleton<IAuthorizationHandler, CanUseInventorHandler>();       //������������ ��� handler	 ??
			//services.AddAuthorization(options =>
			//{
			//	options.AddPolicy(UserPolicy.HaveInventorCode, x => { x.RequireClaim(ClaimEnum.InventorCode.ToString() ); });
			//});

			//services.AddSingleton<IAuthorizationHandler, MinAgeHandler>();	   //������������ ��� handler

			//services.AddAuthorization(options =>
			//{
			//	options.AddPolicy("age-policy", x => { x.RequireClaim("age"); });
			//});
			//����� ���� ������� ���������� �������� claim ����� ����� RequireClaim("x", params values), 
			//����� �������������� ����� ���������� � ��������� �������, ������ RequireClaim("x").RequireClaim("y"). 
			//services.AddAuthorization(options =>
			//{
			//	options.AddPolicy("Founders", policy =>
			//					  policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));			//������ ����������� ��������
			//});

			services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });


			// Add functionality to inject IOptions<T>
			services.AddOptions();    //			��������� ������� ��������� ��� ��������� IOptions<T>�������� �� ������ � ��� ���, ����������� ������� ������������ �� ��������.����� �� ������������� ���� ����������� ����� ������������ � ������������� ��� � �������� ������������, ������� �� ������ ������������ ��� ������ ������.
		

			services.Configure<EmailSettings>(this.Configuration.GetSection("EmailSettings"));
			// Explicitly register the settings object so IOptions not required (optional)
			services.AddSingleton(resolver =>
				resolver.GetRequiredService<IOptions<EmailSettings>>().Value);            //IOptionsSnapshot
																						   // Register as an IValidatable
			services.AddSingleton<IValidatable>(resolver =>
				resolver.GetRequiredService<IOptions<EmailSettings>>().Value);

			services.AddSingleton<IEmailSettings>(resolver =>
				resolver.GetRequiredService<IOptions<EmailSettings>>().Value);
			
			services.AddScoped<IEmailSender, EmailSender>();

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

			//services.AddTransient<HubConnectionBuilder>();
			// �������� ������� ������� ��� ���������������� ������ 400 �������� �������� 
			services.EnableLoggingForBadRequests();
	
			MapDependencyInjection(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env , ILoggerFactory loggerFactory/*, Microsoft.AspNetCore.Hosting.IApplicationLifetime applicationLifetime*/)
        {
			//applicationLifetime.ApplicationStarted.Register(OnAppStarted);
			//applicationLifetime.ApplicationStopping.Register(OnAppStopping);
			//applicationLifetime.ApplicationStopped.Register(OnAppStopped);
			app.UseResponseCompression();
			Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<StartupAuthenticationWebAPI>();
			if (env.IsDevelopment())
            {
				logger.LogInformation("Is Development enviroment");
				app.UseDeveloperExceptionPage();
  				app.UseDatabaseErrorPage();
			}
		
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core 3.1 web API v1");
				//c.RoutePrefix = string.Empty;
			});

			app.UseRouting();
			//app.UseHttpsRedirection();
   //         app.UseStaticFiles();

			app.UseCors(policy =>
			policy.AllowAnyOrigin() //WithOrigins("http://localhost:5000", "http://localhost:27515"
			.AllowAnyMethod()
			.AllowAnyHeader()
			.WithExposedHeaders());//.AllowCredentials());
  
			app.UseAuthentication();
            app.UseAuthorization();

			app.UseSession(); //�� ������ ������������ ������ � ������� HttpContext //context.Session.Keys.Contains("name")
			app.UseMailAsCorrelationId();
			//app.UseLoggerDebugPath();
			//app.UseLoggerConsolePath();
			app.UsePCBIContext();     //Always!!! must use 	   ��������� PCBIContext �� httpContext.Claims

			//app.UseLoggerDebugPCBIContext();       //����� 	 app.UsePCBIContext();
			//app.UseLoggerInformationPathAndPCBIContext();	  //����� 	 app.UsePCBIContext();
			//app.UseLoggerConsolePathAndPCBIContext ();	  //����� 	 app.UsePCBIContext();
			app.UseLoggerDebugPathAndPCBIContext();       //����� 	 app.UsePCBIContext();

			app.UseStatusCodePagesWithReExecute("/error/{0}");      //https://www.devtrends.co.uk/blog/handling-errors-in-asp.net-core-web-api
			app.UseExceptionHandler("/error/500");


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}



		private void MapDependencyInjection(IServiceCollection services)
		{
			services.AddScoped<ControllerTraceServiceFilter>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IPCBIContext, PCBIContext>();

			services.AddControllers()
				.AddControllersAsServices(); // Add the controllers to DI
		}

		private void OnAppStopped()
		{
			//this code is called when the application stops
		}

	}



}

//app.UseHttpsRedirection();
//        app.UseStaticFiles();
//        app.UseAuthentication()


//======
//Handler-� �������� �������� ��� ����� AND, ��� � ����� OR.
//���, ��� ����������� ���������� ����������� AuthorizationHandler<FooRequirement>, ��� ��� ����� �������.
//	��� ���� ����� context.Succeed() �� �������� ������������,
//	� ����� context.Fail() �������� � ������ ������ � ����������� ��� ����������� �� ���������� ������ handler.
//	�����, �� ����� ������������� ����� ����� ������������� ��������� ������� ��������� �������:
//Policy: AND
//Requirement: AND
//Handler: AND / OR.
//services.AddAuthorization(options =>
//{
//     options.AddPolicy("BadgeEntry", policy =>
//        policy.RequireAssertion(context =>
//            context.User.HasClaim(c =>
//                (c.Type == "BadgeId" ||
//                 c.Type == "TemporaryBadgeId") &&
//                 c.Issuer == "https://microsoftsecurity")));
//});

//services.AddAuthorization(options =>
//{
//    options.AddPolicy("Over18", policy =>
//    {
//        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
//        policy.RequireAuthenticatedUser();
//        policy.Requirements.Add(new MinimumAgeRequirement());
//    });
//});
//==========
//public void ConfigureServices(IServiceCollection services)
//{
//	//�������� Identity
//	services.AddIdentity<IdentityUser, IdentityRole>();

//	//������������ ���������
//	services.AddTransient<IUserStore<IdentityUser>, FakeUserStore>();
//	services.AddTransient<IRoleStore<IdentityRole>, FakeRoleStore>();
//}

//	 ������� MVC ����� ��������� ����� �������, ����� ��� �������������� � ����������� �� ��������� ����� �������. � ������� ������������ ���� ���� ����������� ������ MVC � ������ SomeMvcFilter. 
//���� ������ ������������ � �������� ��������� MVC, ������ ���� ���� FeatureA �������.
//services.AddMvc(options => {
//        options.Filters.AddForFeature<SomeMvcFilter>(nameof(MyFeatureFlags.FeatureA));
//    });

//����� ����, ����� ������� ��������� ��������� ����� ���������� � �� �������������� ���� � ������ ���������� �������.
//� ������� ������������ ���� ���� ��������� �� �������������� ���� ����������� � �������� �������, ������ ���� ���� FeatureA �������.
//app.UseMiddlewareForFeature<ThirdPartyMiddleware>(nameof(MyFeatureFlags.FeatureA));

//	private static SqliteConnection SetupSqliteInMemoryConnection()
//	{
//		//var connectionStringBuilder = new SQLiteConnectionStringBuilder { DataSource = ":memory:" };
//		//var connectionString = connectionStringBuilder.ToString();
//		//SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
//		//{
//		//	DataSource = ":memory:",
//		//};
////		var connection = new SQLiteConnection("Data Source=:memory:;mode=memory;cache=shared");

//		//var connectionString = builder.ConnectionString + ";mode=memory;cache=shared";
//		var connection = new SqliteConnection("Data Source =:memory:");
//		connection.Open();  //see https://github.com/aspnet/EntityFramework/issues/6968
//		return connection;
//	}