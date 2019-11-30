using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

//https://andrewlock.net/how-to-include-scopes-when-logging-exceptions-in-asp-net-core/
namespace Count4U.Service.Core.Server.Controllers
{
	[ApiController]
	public class ScopesTestController : Controller
	{
		ILogger<ScopesTestController> _logger;

		public ScopesTestController(ILogger<ScopesTestController> logger)
		{
			_logger = logger;
		}

		// GET api/scopes
		[HttpGet("test/[controller]/[action]")]
		public IEnumerable<string> Get()
		{
			_logger.LogInformation("Before");
			try
			{
				using (_logger.BeginScope("Some name"))
				using (_logger.BeginScope(42))
				using (_logger.BeginScope("Formatted {WithValue}", 12345))
				using (_logger.BeginScope(new Dictionary<string, object> { ["ViaDictionary"] = 100 }))
				{
					_logger.LogInformation("Hello from the Index!");
					_logger.LogDebug("Hello is done");
					throw new Exception("Oops, something went wrong!");
				}

				//_logger.LogInformation("After");

				//return new string[] { "value1", "value2" };
			}
			catch (Exception ex) when (LogError(ex))       //Теперь , когда происходит исключение, он регистрируется со всеми активными областями в точке происходит исключение ( Scope, WithValueили ViaDictionary), а не из активных областей внутри catchблока.
			{
				return new string[] { };
			}
		}

		//!!!  код в фильтре исключений выполняется в том же контексте, в котором произошло исходное исключение 
		// стек не поврежден и выгружается только в том случае, если фильтр исключений оценивает к true.
		[NonAction]
		bool LogError(Exception ex)
		{
			_logger.LogError(ex, "An unexpected exception occured");
			return true;
		}
	}
}
