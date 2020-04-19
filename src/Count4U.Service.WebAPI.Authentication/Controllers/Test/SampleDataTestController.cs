//using Count4U.Service.Common;
//using Count4U.Service.Model;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Count4U.Service.Core.Server.Controllers
//{
//	[ApiController]
//	public class SampleDataTestController : Controller
//    {
//        private static string[] Summaries = new[]
//        {
//            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//        };

//		// [Authorize]
//		//[Authorize(Roles = "Admin")]
//		[Authorize(Policy = "IsAdmin")]
//		[HttpGet("test/[controller]/[action]")]
//        public IEnumerable<WeatherForecast> WeatherForecasts()
//        {
//			var cls = this.HttpContext.User.Claims;
//			var rng = new Random();
//            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//            {
//                Date = DateTime.Now.AddDays(index),
//                TemperatureC = rng.Next(-20, 55),
//                Summary = Summaries[rng.Next(Summaries.Length)]
//            });
//        }



//		// пример как использовать константы для путей
//		[Authorize]
//		[HttpGet(Common.Urls.BlogPosts.GetBlogPosts)]
//		public IEnumerable<WeatherForecast> BlogPosts()
//		{
//			var cls = this.HttpContext.User.Claims;
//			var rng = new Random();
//			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//			{
//				Date = DateTime.Now.AddDays(index),
//				TemperatureC = rng.Next(-20, 55),
//				Summary = Summaries[rng.Next(Summaries.Length)]
//			});

//		//	private readonly WeatherForecastService _service;
//		//public WeatherForecastController(IServiceProvider provider)
//		//{
//		//ServiceLocator
//		//	_service = provider.GetRequiredService<WeatherForecastService>();
//		//}

//		//[HttpGet]
//		//public IEnumerable<WeatherForecast> Get()
//		//{
//		//	return _service.GetForecasts();
//		//}
//	}
//	}
//}
