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
using Count4U.Service.Common;

namespace Count4U.Service.Core.Server.Controllers
{
    //[Route("test/[controller]")]
    //[ApiController]
	//ProfileTestController
	public class ProfileTest //: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public ProfileTest(IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
			_userManager = userManager;
		}

		//The sole job of the login controller is to verify the username and password in the LoginModel using the ASP.NET Core Identity SignInManger.
		//If they're correct then a new JSON web token is generated and passed back to the client in a LoginResult.
		//[Authorize]
		//[HttpPost]
		//public async Task<IActionResult> Profile([FromBody] ProfileModel profile)
  //      {
		//	//1 шаг проверяем есть ли такой пользователь с таким паролем
		//	//var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
		//	//if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
		//	//if (User.Identity.IsAuthenticated)
		//	//{
		//	//	string email = User.Identity.Name;
		//	//}

		//	var user = await _userManager.FindByEmailAsync(User.Identity.Name);
		//	//var correctuser = await _userManager.CheckPasswordAsync(user, user.);

		//	//2 шаг - если нет отправляем неудачно, (надо на регистрацию отправлять если что)
		//	//if (correctuser == false)
		//	//{
		//	//	return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
		//	//}
		//	//3 шаг - сохраняетм данные, что бы потом читать 
		//	//var claims = new[]
		//	//         {
		//	//             new Claim(ClaimTypes.Name, login.Email)
		//	//         };

		//	//var roles = await _signInManager.UserManager.GetRolesAsync(user);
		//	var claims = await _signInManager.UserManager.GetClaimsAsync(user);

			
		//	Dictionary<string, Claim> dics = claims.Select(e => e).Distinct().ToDictionary(k => k.Type);
		//	//имя пользователя
		//	//claims.Add(new Claim(ClaimTypes.Name, login.Email));
		//	//роли пользователя
		//	//foreach (var role in roles)
		//	//{
		//	//	claims.Add(new Claim(ClaimTypes.Role, role));
		//	//}

		//	//тестовый вариант все настройки из конфига
		//	{
		//		Claim claim = dics[ClaimEnum.DataServerAddress.ToString()];
		//		claims.Remove(claim);
		//		string dataServerAddress = profile.DataServerAddress;
		//		claims.Add(new Claim(ClaimEnum.DataServerAddress.ToString(), dataServerAddress));

		//		//claim = dics[ClaimEnum.DataServerPort.ToString()];
		//		//claims.Remove(claim);
		//		//string dataServerPort = profile.DataServerPort;
		//		//claims.Add(new Claim(ClaimEnum.DataServerPort.ToString(), dataServerPort));

		//		claim = dics[ClaimEnum.AccessKey.ToString()];
		//		claims.Remove(claim);
		//		string accessKey = profile.AccessKey;
		//		claims.Add(new Claim(ClaimEnum.AccessKey.ToString(), accessKey));

		//		claim = dics[ClaimEnum.CustomerCode.ToString()];
		//		claims.Remove(claim);
		//		string customerCode = profile.CustomerCode;
		//		claims.Add(new Claim(ClaimEnum.CustomerCode.ToString(), customerCode));

		//		claim = dics[ClaimEnum.BranchCode.ToString()];
		//		claims.Remove(claim);
		//		string branchCode = profile.BranchCode;
		//		claims.Add(new Claim(ClaimEnum.BranchCode.ToString(), branchCode));

		//		claim = dics[ClaimEnum.InventorCode.ToString()];
		//		claims.Remove(claim);
		//		string inventorCode = profile.InventorCode;
		//		claims.Add(new Claim(ClaimEnum.InventorCode.ToString(), inventorCode));

		//		claim = dics[ClaimEnum.DBPath.ToString()];
		//		claims.Remove(claim);
		//		string dbPath = profile.DBPath;
		//		claims.Add(new Claim(ClaimEnum.DBPath.ToString(), dbPath));

				
		//	}

		//	//????
		//	//await _userManager.UpdateAsync(user);
		//	//await _signInManager.UserManager.
				
		//		// создаем 	JwtSecurityToken , подписываем
		//	var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
  //          var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);     // ключ для шифрования
		//	var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

  //          var token = new JwtSecurityToken(
		//		issuer: _configuration["JwtIssuer"], //issuer - издатель токена f.e "MyAuthServer" //является заявкой и в первую очередь указывает, какое приложение сгенерировало этот токен.
		//		audience:	_configuration["JwtIssuer"],      // потребитель токена	 f.e "http://localhost:51884/"
		//		claims,
  //              expires: expiry,
  //              signingCredentials: creds
  //          );

		//	//issuer- Если у вас есть несколько клиентских приложений, которые могут взаимодействовать с вашим API, может быть полезно включить указание на целевую аудиторию в самом токене.
		//	//Константа ISSUER представляет издателя токена. Здесь можно определить любое название. AUDIENCE представляет потребителя токена - опять же может быть любая строка, но в данном случае указан адрес текущего приложения.
		//   //	Константа KEY хранит ключ, который будет применяться для создания токена.

		//	return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
  //      }
    }
}


