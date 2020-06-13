using Count4U.Service.Model;
using Count4U.Service.Core.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Count4U.Service.Common;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Monitor.Service.Urls;
using Monitor.Service.Model;
 using Count4U.Service.Shared;
using Monitor.Service.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
	[ApiController]
	[Produces("application/json")]
	[Consumes("application/json")]
	public class LoginController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<LoginController> _logger;
		private readonly IEmailSender _emailSender;

		public LoginController(ILoggerFactory loggerFactory
			, IConfiguration configuration
			, SignInManager<ApplicationUser> signInManager
			, UserManager<ApplicationUser> userManager
			, IEmailSender emailSender)
		{
			this._logger = loggerFactory.CreateLogger<LoginController>();
			_configuration = configuration ??
						  throw new ArgumentNullException(nameof(configuration));
			_userManager = userManager ??
							  throw new ArgumentNullException(nameof(userManager));
			_signInManager = signInManager ??
							  throw new ArgumentNullException(nameof(signInManager));
				this._emailSender = emailSender ??
							  throw new ArgumentNullException(nameof(emailSender));
		}

		//The sole job of the login controller is to verify the username and password in the LoginModel using the ASP.NET Core Identity SignInManger.
		//If they're correct then a new JSON web token is generated and passed back to the client in a LoginResult.
		[AllowAnonymous]
		[HttpPost(WebApiAuthenticationLogin.PostLogin)]
		[ServiceFilter(typeof(ControllerTraceServiceFilter))]
		//[FeatureGate(C4UFeatureFlags.FeatureA)]
		public async Task<LoginResult> LoginAsync([FromBody] LoginModel login)
		{
			if (login == null)
			{
				return new LoginResult { Successful = SuccessfulEnum.NotSuccessful, Error = "LoginModel is null " };
			}
			//TEST 1
			//_logger.ControllerOnStart(HttpContext, RouteData);
			//Console.WriteLine(testMessage + " OnInitialized  !!!");
			//1 шаг проверяем есть ли такой пользователь с таким паролем
			//var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
			//if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
			ApplicationUser applicationUser = await _userManager.FindByEmailAsync(login.Email);
			var correctuser = await _userManager.CheckPasswordAsync(applicationUser, login.Password);

			//2 шаг - если нет отправляем неудачно, (надо на регистрацию отправлять если что)
			if (correctuser == false)
			{
				return new LoginResult { Successful = SuccessfulEnum.NotSuccessful, Error = "Username and password are invalid. " };
			}
			//3 шаг - сохраняетм данные, что бы потом читать 
			//var claims = new[]
			//         {
			//             new Claim(ClaimTypes.Name, login.Email)
			//         };


			var claims = new List<Claim>();
			//ID 
			claims.Add(new Claim(ClaimEnum.ApplicationUserId.ToString(), applicationUser.Id));
			//имя пользователя
			claims.Add(new Claim(ClaimTypes.Name, login.Email));

			 var roles = await _signInManager.UserManager.GetRolesAsync(applicationUser);
			
			//роли пользователя
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			//ТЕСТ работает
			//var have = claims.Any(c => c.Type == "manager");
			//if (have == false) 
			//{
			//	await _signInManager.UserManager.AddClaimAsync(user, new Claim("manager", "full"));
			//}

		

			//тестовый вариант все настройки из конфига
			{
				//string dataServerAddress   = _configuration["DataServerAddress"];
				//claims.Add(new Claim(ClaimEnum.DataServerAddress.ToString(), dataServerAddress));

				//string dataServerPort = _configuration["DataServerPort"];
				//claims.Add(new Claim(ClaimEnum.DataServerPort.ToString(), dataServerPort));

				//string accessKey = _configuration["AccessKey"];
				//claims.Add(new Claim(ClaimEnum.AccessKey.ToString(), accessKey));

				//string customerCode = _configuration["CustomerCode"];
				//claims.Add(new Claim(ClaimEnum.CustomerCode.ToString(), customerCode));

				//string branchCode = _configuration["BranchCode"];
				//claims.Add(new Claim(ClaimEnum.BranchCode.ToString(), branchCode));

				//string inventorCode = _configuration["InventorCode"];
				//claims.Add(new Claim(ClaimEnum.InventorCode.ToString(), inventorCode));
			}

			{
				ProfileModel profileModel = applicationUser.ToProfileModel();
				if (profileModel != null)
				{
					claims.Add(new Claim(ClaimEnum.DataServerAddress.ToString(), profileModel.DataServerAddress));
					claims.Add(new Claim(ClaimEnum.AccessKey.ToString(), profileModel.AccessKey));
					claims.Add(new Claim(ClaimEnum.CustomerCode.ToString(), profileModel.CustomerCode));
					claims.Add(new Claim(ClaimEnum.BranchCode.ToString(), profileModel.BranchCode));
					claims.Add(new Claim(ClaimEnum.InventorCode.ToString(), profileModel.InventorCode));
					claims.Add(new Claim(ClaimEnum.DBPath.ToString(), profileModel.DBPath));

				}
			}

			//from AspNetUserClaims , аналог ролей
			var userClaims = await _signInManager.UserManager.GetClaimsAsync(applicationUser);
			foreach (var userClaim in userClaims)
			{
				var have = claims.Any(c => c.Type == userClaim.Type);
				if (have == false)
				{
					claims.Add(userClaim);
				}
			}

			// создаем 	JwtSecurityToken , подписываем
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);     // ключ для шифрования
			var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JwtIssuer"], //issuer - издатель токена f.e "MyAuthServer" //является заявкой и в первую очередь указывает, какое приложение сгенерировало этот токен.
				audience: _configuration["JwtIssuer"],      // потребитель токена	 f.e "http://localhost:51884/"
				claims,
				expires: expiry,
				signingCredentials: creds
			);

			//issuer- Если у вас есть несколько клиентских приложений, которые могут взаимодействовать с вашим API, может быть полезно включить указание на целевую аудиторию в самом токене.
			//Константа ISSUER представляет издателя токена. Здесь можно определить любое название. AUDIENCE представляет потребителя токена - опять же может быть любая строка, но в данном случае указан адрес текущего приложения.
			//	Константа KEY хранит ключ, который будет применяться для создания токена.

			return new LoginResult { Successful = SuccessfulEnum.Successful, Token = new JwtSecurityTokenHandler().WriteToken(token) };
		}


	

	
	

		
		//	var callbackUrl = Url.Action("ResetPassword", nameof(UtilController), new ResetPasswordModel{Code= token, Email = user.Email }, Request.Scheme);
			//var callbackUrl = Url.Action("ResetPassword", "Util", new { userId = user.Email, code = token }, protocol: HttpContext.Request.Scheme);
			//var callbackUrl = Url.Action(nameof(ResetPassword), "Util", new { token, email = user.Email }, Request.Scheme);
			//var message = new EmailMessage(new string[] { model.Email}, "Reset password token", passwordResetUrl, null);
			//await _emailSender.SendEmailAsync(message);


				//passwordResetUrl = navigationManager.BaseUri;
      //         passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
      //         string passwordResetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
      //         string passwordResetUrl = "https://localhost:27027/ResetPassword?email=" + user.Email + "&token=" + passwordResetToken;
	


		//	[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
		//{
		//    if (!ModelState.IsValid)
		//        return View(forgotPasswordModel);

		//    var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
		//    if (user == null)
		//        return RedirectToAction(nameof(ForgotPasswordConfirmation));

		//    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
		//    var callback = Url.Action(nameof(ResetPassword), nameof(AccountController), new { token, email = user.Email }, Request.Scheme);

		//    var message = new Message(new string[] { "codemazetest@gmail.com" }, "Reset password token", callback, null);
		//    await _emailSender.SendEmailAsync(message);

		//    return RedirectToAction(nameof(ForgotPasswordConfirmation));
		//}
	}
}


