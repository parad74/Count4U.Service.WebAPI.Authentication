using Count4U.Service.Model;
using Count4U.Service.Core.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Monitor.Service.Model;
using Monitor.Service.Urls;
 using Count4U.Service.Shared;
using Monitor.Service.Shared;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
	[ApiController]
	[Produces("application/json")]
	[Consumes("application/json")]
	[ServiceFilter(typeof(ControllerTraceServiceFilter))]
	public class AccountsController : ControllerBase
	{
		private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false , Email=""};

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AccountsController> _logger;
	

		public AccountsController(ILoggerFactory loggerFactory
			, IConfiguration configuration
			, UserManager<ApplicationUser> userManager
			, SignInManager<ApplicationUser> signInManager
		
		)
		{
			this._logger = loggerFactory.CreateLogger<AccountsController>();
			this._configuration = configuration ??
							  throw new ArgumentNullException(nameof(configuration));
			this._userManager = userManager ??
							  throw new ArgumentNullException(nameof(userManager));
			this._signInManager = signInManager ??
							  throw new ArgumentNullException(nameof(signInManager));
		
		}

		[AllowAnonymous]
		[HttpPost(WebApiAuthenticationAccounts.PostRegister)]
		public async Task<RegisterResult> PostAsync([FromBody]RegisterModel model)
		{
			if (model == null)
			{
				return new RegisterResult { Successful = SuccessfulEnum.NotSuccessful , Error = "RegisterModel is null " };
			}

			ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				return new RegisterResult { Successful = SuccessfulEnum.NotSuccessful , Error = "User with the same e-mail there is " };
 			}

			var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email };

			IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);

			if (result.Succeeded == false)
			{
				var errors = result.Errors.Select(x => x.Description);
				var error = string.Join(" .", errors);
				return new RegisterResult { Successful = SuccessfulEnum.NotSuccessful , Error = error };
			}

			// Add all new users to the User role
			await _userManager.AddToRoleAsync(newUser, "User");

			// Add new users whose email starts with 'admin' to the Admin role
			//		builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Owner", NormalizedName = "OWNER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
			//builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "User", NormalizedName = "USER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
			//builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });  //users and roles
			//builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Manager", NormalizedName = "MANAGER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() }); //For all profiles, process, customers params
			//builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Monitor", NormalizedName = "MONITOR", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });  //metriks 
			//builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Context", NormalizedName = "CONTEXT", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });   //profile view
			//builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Worker", NormalizedName = "WORKER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });	   //Inventor in field
			//builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Profile", NormalizedName = "PROFILE", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });	  //my Profile Edit

				//test 
			if (newUser.Email.StartsWith("parad74@mail.ru") )
			{
				await _userManager.AddToRoleAsync(newUser, "Owner");
				await _userManager.AddToRoleAsync(newUser, "Admin");
				await _userManager.AddToRoleAsync(newUser, "Manager");
				await _userManager.AddToRoleAsync(newUser, "Monitor");
				await _userManager.AddToRoleAsync(newUser, "Context");
				await _userManager.AddToRoleAsync(newUser, "Worker");
				await _userManager.AddToRoleAsync(newUser, "Profile");
				//test 
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.owner.ToString(), "full"));
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.admin.ToString(), "full"));
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.manager.ToString(), "full"));
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.monitor.ToString(), "full"));
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.context.ToString(), "full"));
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.worker.ToString(), "full"));
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.profile.ToString(), "full"));
			}

			if (newUser.Email.StartsWith("admin"))
			{
				await _userManager.AddToRoleAsync(newUser, "Admin");
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.admin.ToString(), "full"));
			}

			if (newUser.Email.StartsWith("manager")) 
			{
				await _userManager.AddToRoleAsync(newUser, "Manager");
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.manager.ToString(), "full"));
			}

			if (newUser.Email.StartsWith("monitor")) 
			{
				await _userManager.AddToRoleAsync(newUser, "Monitor");
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.monitor.ToString(), "full"));
			}

			if (newUser.Email.StartsWith("context")) 
			{
				await _userManager.AddToRoleAsync(newUser, "Context");
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.context.ToString(), "full"));
			}

			if (newUser.Email.StartsWith("worker")) 
			{
				await _userManager.AddToRoleAsync(newUser, "Worker");
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.worker.ToString(), "full"));
			}

			if (newUser.Email.StartsWith("profile")) 
			{
				await _userManager.AddToRoleAsync(newUser, "Profile");
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.profile.ToString(), "full"));
			}

			if (string.IsNullOrWhiteSpace(newUser.Id) == false)
			{
				await _signInManager.UserManager.AddClaimAsync(newUser, new Claim(ClaimEnum.ApplicationUserId.ToString(), newUser.Id));
				return new RegisterResult { Successful = SuccessfulEnum.Successful, ApplicationUserID =  newUser.Id};
			}
			else
			 {
				return new RegisterResult { Successful = SuccessfulEnum.Successful , Message = "ApplicationUserID is empty? looks like error" };
			}

			
		}


		[Authorize]
		[HttpPost(WebApiAuthenticationAccounts.PostProfile)]
		public async Task<ProfileResult> PostProfileAsync([FromBody]ProfileModel profileModel)
		{
			if (profileModel == null)
			{
				return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful , Error = " ProfileModel is null " };
			}

			if (string.IsNullOrWhiteSpace(profileModel.ID))
			{
				return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful, Error = "User ID is empty " };
			}
		//	ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
			ApplicationUser user = await _userManager.FindByIdAsync(profileModel.ID);
			if (user != null)
			{
				return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful , Error = "Can't get user from db "  };
			}

			user.DataServerAddress = profileModel.DataServerAddress != null ? profileModel.DataServerAddress : "";
			//user.DataServerPort = model.DataServerPort != null ? model.DataServerPort : "";
			user.AccessKey = profileModel.AccessKey != null ? profileModel.AccessKey : "";
			user.CustomerCode = profileModel.CustomerCode != null ? profileModel.CustomerCode : "";
			user.BranchCode = profileModel.BranchCode != null ? profileModel.BranchCode : "";
			user.InventorCode = profileModel.InventorCode != null ? profileModel.InventorCode : "";
			user.DBPath = profileModel.DBPath != null ? profileModel.DBPath : "";
			//first = person?.FirstName ?? "Unspecified";

			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded == false)
			{
				var errors = result.Errors.Select(x => x.Description);
				 var error = string.Join(" .", errors);
					return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful , Error = error };
			}

			return new ProfileResult { Successful = SuccessfulEnum.Successful  };
		}

		[Authorize]
		[HttpPost(WebApiAuthenticationAccounts.PostUpdateprofile)]
		//[ServiceFilter(typeof(ControllerTraceServiceFilter))]
		//[FeatureGate(C4UFeatureFlags.FeatureA)]
		public async Task<ProfileResult> UpdateProfileAsync([FromBody]ProfileModel profileModel)
		{
			if (profileModel == null)
			{
				return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful, Error = "ProfileModel is null " };
			}
			if (string.IsNullOrWhiteSpace(profileModel.ID))
			{
				return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful, Error = "User ID is empty " };
			}
			//ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);     
			ApplicationUser user = await _userManager.FindByIdAsync(profileModel.ID);
			if (user == null)
			{
				return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful, Error = "User is null " };
			}
			user.DataServerAddress = profileModel.DataServerAddress != null ? profileModel.DataServerAddress : "";
			//user.DataServerPort = profileModel.DataServerPort != null ? profileModel.DataServerPort : "";
			user.AccessKey = profileModel.AccessKey != null ? profileModel.AccessKey : "";
			user.CustomerCode = profileModel.CustomerCode != null ? profileModel.CustomerCode : "";
			user.BranchCode = profileModel.BranchCode != null ? profileModel.BranchCode : "";
			user.InventorCode = profileModel.InventorCode != null ? profileModel.InventorCode : "";
			user.DBPath = profileModel.DBPath != null ? profileModel.DBPath : "";
			//first = person?.FirstName ?? "Unspecified";

			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded == false)
			{
				var errors = result.Errors.Select(x => x.Description);
				var error = string.Join(" ." , errors);
				return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful, Error = error };
			}

			var userClaims = this.HttpContext.User.Claims;
			List<Claim> claims = new List<Claim>();

			foreach ( var claim in userClaims)
			{
				if (claim.Type == ClaimEnum.DataServerAddress.ToString())
				{
					claims.Add(new Claim(ClaimEnum.DataServerAddress.ToString(), profileModel.DataServerAddress));
				}
				else if (claim.Type == ClaimEnum.ApplicationUserId.ToString())
				{
					claims.Add(new Claim(ClaimEnum.ApplicationUserId.ToString(), profileModel.ID));
				}

				//else if (claim.Type == ClaimEnum.DataServerPort.ToString())
				//{
				//	claims.Add(new Claim(ClaimEnum.DataServerPort.ToString(), profileModel.DataServerPort));
				//}

				else if (claim.Type == ClaimEnum.AccessKey.ToString())
				{
					claims.Add(new Claim(ClaimEnum.AccessKey.ToString(), profileModel.AccessKey));
				}

				else if (claim.Type == ClaimEnum.CustomerCode.ToString())
				{
					claims.Add(new Claim(ClaimEnum.CustomerCode.ToString(), profileModel.CustomerCode));
				}

				else if (claim.Type == ClaimEnum.BranchCode.ToString())
				{
					claims.Add(new Claim(ClaimEnum.BranchCode.ToString(), profileModel.BranchCode));
				}

				else if (claim.Type == ClaimEnum.InventorCode.ToString())
				{
					claims.Add(new Claim(ClaimEnum.InventorCode.ToString(), profileModel.InventorCode));
				}
				else if (claim.Type == ClaimEnum.DBPath.ToString())
				{
					claims.Add(new Claim(ClaimEnum.DBPath.ToString(), profileModel.DBPath));
				}
				
				else
				{
					claims.Add(claim);
				}
			}
	   
			// создаем 	JwtSecurityToken , подписываем
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);     // ключ для шифрования
			var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JwtIssuer"], //issuer - издатель токена f.e "MyAuthServer" //является заявкой и в первую очередь указывает, какое приложение сгенерировало этот токен.
				audience: _configuration["JwtIssuer"],      // потребитель токена	 f.e "http://localhost/"
				claims,
				expires: expiry,
				signingCredentials: creds
			);

			//issuer- Если у вас есть несколько клиентских приложений, которые могут взаимодействовать с вашим API, может быть полезно включить указание на целевую аудиторию в самом токене.
			//Константа ISSUER представляет издателя токена. Здесь можно определить любое название. AUDIENCE представляет потребителя токена - опять же может быть любая строка, но в данном случае указан адрес текущего приложения.
			//	Константа KEY хранит ключ, который будет применяться для создания токена.
			return new ProfileResult { Successful = SuccessfulEnum.Successful, Token = new JwtSecurityTokenHandler().WriteToken(token) };
		}

		//===
		[Authorize]
		[HttpGet(WebApiAuthenticationAccounts.GetUser)]
		public IActionResult GetUser()
		{
			if (User == null) return Ok(LoggedOutUser);
			if (User.Identity == null) return Ok(LoggedOutUser);

			if (User.Identity.IsAuthenticated)
			{
				var userModel = new UserModel
				{
					Email = User.Identity.Name,
					IsAuthenticated = true
				};

				return Ok(userModel);
			}

			return Ok(LoggedOutUser);
		}

		//[Authorize]
		//[HttpGet(WebApiAuthenticationAccounts.GetCurrentUserProfile)]		//NOT USE?		 вообще не то
		//public async Task<IActionResult> Profile()
		//{
		//	if (User == null) return Ok(new ProfileModel());
		//	if (User.Claims == null) return Ok(new ProfileModel());
		//	if (User.Identity == null) return Ok(new ProfileModel());
		//   ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);   
		//	if (user == null) return Ok(new ProfileModel());

		//	return Ok(new ProfileModel()
		//	{
				
		//		ID = user.Id,
		//		DataServerAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DataServerAddress.ToString())?.Value,
		//		//DataServerPort = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DataServerPort.ToString())?.Value,
		//		AccessKey = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.AccessKey.ToString())?.Value,
		//		CustomerCode = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.CustomerCode.ToString())?.Value,
		//		BranchCode = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.BranchCode.ToString())?.Value,
		//		InventorCode = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.InventorCode.ToString())?.Value,
		//		DBPath = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DBPath.ToString())?.Value
		//		//Name = User.Identity.Name,
		//		//Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
		//		//ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
		//	});
		//}

	

		//[Authorize]
		//[HttpPost(WebApiAuthenticationAccounts.DeleteUser)]
  //      public async Task<IActionResult> Delete(string id) 
		//{
		//	ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);      
		//	if (user == null)
		//	{
		//		return new ProfileResult { Successful = SuccessfulEnum.NotSuccessful, Error = "User is null " };
		//	}
  //          AppUser user = await userManager.FindByIdAsync(id);
  //          if (user != null) {
  //              IdentityResult result = await userManager.DeleteAsync(user);
  //              if (result.Succeeded) {
  //                  return RedirectToAction("Index");
  //              } else {
  //                  AddErrorsFromResult(result);
  //              }
  //          } else {
  //              ModelState.AddModelError("", "User Not Found");
  //          }
  //          return View("Index", userManager.Users);
  //      }
	}
}
