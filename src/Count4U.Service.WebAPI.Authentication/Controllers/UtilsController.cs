using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Count4U.Service.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
	[ApiController]
	[Produces("application/json")]
	public class UtilsController : Controller
	{
		[AllowAnonymous]
		[HttpGet("Error")]
		public IActionResult Error()
		{
			return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}