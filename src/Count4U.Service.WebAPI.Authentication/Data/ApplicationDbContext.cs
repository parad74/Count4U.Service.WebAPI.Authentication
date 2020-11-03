using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Count4U.Service.Core.Server.Data
{

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			//Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			//This will pick up all configurations that are defined in the assembly
			//builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			//Instead of this:
			builder.ApplyConfiguration(new RoleConfiguration());
			builder.ApplyConfiguration(new AdminConfiguration());
			builder.ApplyConfiguration(new AdminWithRolesConfig());
			builder.ApplyConfiguration(new ManagerConfiguration());
			builder.ApplyConfiguration(new ManagerWithRolesConfig());

			//	string OwnerId = "799e3d1d-febd-4bd4-9c1f-26afd13bb0c9";
			//	string UserId = "c401dbc0-080b-4fbe-a8a1-275cd4068eb7";
			//	string AdminId = "5f998083-c061-4f3a-9589-7aeedaa306c8";
			//	string ManagerId = "a3f5048f-55a8-4766-b8ec-03f859b8c399";
			//	string MonitorId = "0ccc4920-47aa-4b34-acf8-77f5e0ad6c40";
			//	string ContextId = "9e8f096e-c230-4917-b412-f0a5e403c499";
			//	string WorkerId = "a87b2288-7534-458d-b4f6-4831239f8570";
			//	string ProfileId = "38539cf9-77e9-4129-b6a6-9cfbbf3c64ac";

			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = OwnerId, Name = "Owner", NormalizedName = "OWNER", ConcurrencyStamp = Guid.NewGuid().ToString() });
			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = UserId, Name = "User", NormalizedName = "USER", ConcurrencyStamp = Guid.NewGuid().ToString() });
			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = AdminId, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = Guid.NewGuid().ToString() });  //users and roles
			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = ManagerId, Name = "Manager", NormalizedName = "MANAGER", ConcurrencyStamp = Guid.NewGuid().ToString() }); //For all profiles, process, customers params
			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = MonitorId, Name = "Monitor", NormalizedName = "MONITOR", ConcurrencyStamp = Guid.NewGuid().ToString() });  //metriks 
			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = ContextId, Name = "Context", NormalizedName = "CONTEXT", ConcurrencyStamp = Guid.NewGuid().ToString() });   //profile view
			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = WorkerId, Name = "Worker", NormalizedName = "WORKER",  ConcurrencyStamp = Guid.NewGuid().ToString() });      //Inventor in field
			//	builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = ProfileId, Name = "Profile", NormalizedName = "PROFILE", ConcurrencyStamp = Guid.NewGuid().ToString() });      //my Profile Edit
		}

	}


	//public class ApplicationDbContext : IdentityDbContext
	//{
	//    public ApplicationDbContext(DbContextOptions options) : base(options)
	//    {
	//    }
	//}
}


//[Authorize]
//public async Task<IActionResult> UserSetup()
//{
//	if (!await _roleManager.RoleExistsAsync("Admin")) {
//		var role = new IdentityRole { Name = "Admin" };
//		await _roleManager.CreateAsync(role);
//	}

//	// add role to user
//	var user = await _userManager.GetUserAsync(HttpContext.User);
//	if (!await _userManager.IsInRoleAsync(user, "Admin")) {
//		await _userManager.AddToRoleAsync(user, "Admin");
//	}

//	return View();
//}
//см IdentityServer4Example
//public class IdServer4ExampleDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
//{
//	public IdServer4ExampleDbContext(DbContextOptions<IdServer4ExampleDbContext> options)
//		: base(options)
//	{
//	}

//	public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }

//	protected override void OnModelCreating(ModelBuilder builder)
//	{
//		builder.Entity<ApplicationUser>().HasKey(p => p.Id);

//		base.OnModelCreating(builder);
//	}
//}

//////!!!!!!!!!!
///add-migration 
///update-database
///
//Add-Migration SeedRoles
//Update-Database

//Add-Migration ApplicationUser  -Context ApplicationDbContext
//Update-Database  -Context ApplicationDbContext

//enable-migrations -ContextProjectName MyProject.DBContexts -contexttypename MyProject.DBContexts.MyContextName -Verbose
//enable-migrations -ContextProjectName Count4U.Service.Server.Extensions -contexttypename Count4U.Service.Core.Server.Data.ApplicationDbContext -Verbose