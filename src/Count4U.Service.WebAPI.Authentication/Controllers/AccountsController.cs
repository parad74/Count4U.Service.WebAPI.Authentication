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
using Count4U.Service.Common;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Count4U.Service.Common.Urls;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
	[ApiController]
	[Produces("application/json")]
	[ServiceFilter(typeof(ControllerTraceServiceFilter))]
	public class AccountsController : ControllerBase
	{
		private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AccountsController> _logger;

		//	private readonly ILogger<ApplicationUser> _userManagerLogger;

		public AccountsController(ILoggerFactory loggerFactory,
			IConfiguration configuration, 
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this._logger = loggerFactory.CreateLogger<AccountsController>();
			this._configuration = configuration ??
							  throw new ArgumentNullException(nameof(configuration));
			this._userManager = userManager ??
							  throw new ArgumentNullException(nameof(userManager));
			this._signInManager = signInManager ??
							  throw new ArgumentNullException(nameof(signInManager));
			//_userManagerLogger = userManagerLogger;

		}

		[AllowAnonymous]
		[HttpPost(WebApiAuthenticationAccounts.PostRegister)]
		public async Task<IActionResult> Post([FromBody]RegisterModel model)
		{
			if (model == null)
			{
				List<string> errors = new List<string>() { "RegisterModel is null" };
				return Ok(new RegisterResult { Successful = false, Errors = errors });
			}

			ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				List<string> errors = new List<string>() { "User with the same e-mail there is" };
				return Ok(new RegisterResult { Successful = false, Errors = errors });
 			}

			var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email };

			var result = await _userManager.CreateAsync(newUser, model.Password);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(x => x.Description);

				return Ok(new RegisterResult { Successful = false, Errors = errors });

			}

			// Add all new users to the User role
			await _userManager.AddToRoleAsync(newUser, "User");

			// Add new users whose email starts with 'admin' to the Admin role
			if (newUser.Email.StartsWith("admin"))
			{
				await _userManager.AddToRoleAsync(newUser, "Admin");
			}

			return Ok(new RegisterResult { Successful = true });
		}


		[Authorize]
		[HttpPost(WebApiAuthenticationAccounts.PostProfile)]
		public async Task<IActionResult> PostProfile([FromBody]ProfileModel model)
		{
			if (model == null)
			{
				List<string> errors = new List<string>() { " ProfileModel is null" };
				return Ok(new RegisterResult { Successful = false, Errors = errors });
			}

			ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
			if (user != null)
			{
				List<string> errors = new List<string>() { "Can't get user from db" };
				return Ok(new RegisterResult { Successful = false, Errors = errors });
			}

			user.DataServerAddress = model.DataServerAddress != null ? model.DataServerAddress : "";
			//user.DataServerPort = model.DataServerPort != null ? model.DataServerPort : "";
			user.AccessKey = model.AccessKey != null ? model.AccessKey : "";
			user.CustomerCode = model.CustomerCode != null ? model.CustomerCode : "";
			user.BranchCode = model.BranchCode != null ? model.BranchCode : "";
			user.InventorCode = model.InventorCode != null ? model.InventorCode : "";
			user.DBPath = model.DBPath != null ? model.DBPath : "";
			//first = person?.FirstName ?? "Unspecified";

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(x => x.Description);

				return Ok(new RegisterResult { Successful = false, Errors = errors });

			}

			return Ok(new RegisterResult { Successful = true });
		}

		[Authorize]
		[HttpPost(WebApiAuthenticationAccounts.PostUpdateprofile)]
		//[ServiceFilter(typeof(ControllerTraceServiceFilter))]
		//[FeatureGate(C4UFeatureFlags.FeatureA)]
		public async Task<IActionResult> UpdateProfile([FromBody]ProfileModel profileModel)
		{
			if (profileModel == null)
			{
				return Ok(new LoginResult { Successful = false, Error = "ProfileModel is null" });
			}

			ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);      
			if (user == null)
			{
				return Ok(new LoginResult { Successful = false, Error = "User is null" });
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

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(x => x.Description);

				return Ok(new LoginResult { Successful = false, Error = "can't update ApplicationUser" });

			}

			var userClaims = this.HttpContext.User.Claims;
			List<Claim> claims = new List<Claim>();

			foreach ( var claim in userClaims)
			{
				if (claim.Type == ClaimEnum.DataServerAddress.ToString())
				{
					claims.Add(new Claim(ClaimEnum.DataServerAddress.ToString(), profileModel.DataServerAddress));
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

			//_logger.ControllerOnEnd(HttpContext, RouteData);
			return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
		}

		//===
		//[Authorize]
		[HttpGet(WebApiAuthenticationAccounts.GetUser)]
		public IActionResult GetUser()
		{
			if (User != null) return Ok(LoggedOutUser);
			if (User.Identity != null) return Ok(LoggedOutUser);

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

		[Authorize]
		[HttpGet(WebApiAuthenticationAccounts.GetProfile)]
		public IActionResult Profile()
		{
			if (User != null) return Ok(new ProfileModel());
			if (User.Claims != null) return Ok(new ProfileModel());

			return Ok(new ProfileModel()
			{
				DataServerAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DataServerAddress.ToString())?.Value,
				//DataServerPort = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DataServerPort.ToString())?.Value,
				AccessKey = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.AccessKey.ToString())?.Value,
				CustomerCode = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.CustomerCode.ToString())?.Value,
				BranchCode = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.BranchCode.ToString())?.Value,
				InventorCode = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.InventorCode.ToString())?.Value,
				DBPath = User.Claims.FirstOrDefault(c => c.Type == ClaimEnum.DBPath.ToString())?.Value
				//Name = User.Identity.Name,
				//Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
				//ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
			});
		}

		
	}
}
