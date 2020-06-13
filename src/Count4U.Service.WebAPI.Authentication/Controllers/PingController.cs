using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monitor.Service.Model;
using Monitor.Service.Urls;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
	[ApiController]
	[Produces("application/json")]
	[Consumes("application/json")]
	public class PingController : Controller
	{
		[HttpGet]
		[Route(WebApiAuthenticationPing.GetPing)]
		public string Ping()
		{
			return PingOpetarion.Pong;
		}

		[Authorize]
		[HttpGet]
		[Route(WebApiAuthenticationPing.GetPingSecure)]
		public string PingSecured()
		{
			// "All good. You only get this message if you are authenticated.";
			return PingOpetarion.Pong;
		}




	
		//[Authorize]
		//[HttpGet("claims")]
		//public object Claims()
		//{
		//	return User.Claims.Select(c =>
		//	new
		//	{
		//		Type = c.Type,
		//		Value = c.Value
		//	});
		//}
	}
}
