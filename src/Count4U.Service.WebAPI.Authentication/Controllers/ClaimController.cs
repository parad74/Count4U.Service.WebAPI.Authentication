using Count4U.Service.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Service.Model;
using Monitor.Service.Urls;
using Monitor.Service.Model;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
	[Authorize]
	[ApiController]
	[Produces("application/json")]
	[Consumes("application/json")]
	[ServiceFilter(typeof(ControllerTraceServiceFilter))]
	public class ClaimController : ControllerBase
    {
        private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };

		private readonly ILogger<ClaimController> _logger;
		public ClaimController(ILoggerFactory loggerFactory)
        {
			this._logger = loggerFactory.CreateLogger<ClaimController>();
		}

		[HttpGet(WebApiAuthenticationClaim.GetClaims)]
		public ActionResult<IEnumerable<ClaimConvertItem>> GetClaims()
		{
			List<ClaimConvertItem> returnList = new List<ClaimConvertItem>();

			if (this.HttpContext == null)
				return BadRequest();
			if (this.HttpContext.User == null)
				return NotFound();
			if (this.HttpContext.User.Claims == null)
				return NotFound();

			var array = this.HttpContext.User.Claims.ToArray();

			foreach (var arr in array)
			{
				string name = arr.Subject != null ? arr.Subject.Name : "";
				returnList.Add(new ClaimConvertItem(name, arr.Issuer, arr.Type, arr.Value));
			}

			return Ok(returnList.AsEnumerable());
		}

		//Тест синхронный
		[HttpGet(WebApiAuthenticationClaim.GetClaimConvertItems)]
		public ActionResult<IEnumerable<ClaimConvertItem>> GetClaimConvertItems()
		{
			if (this.HttpContext == null)
				return BadRequest();
			if (this.HttpContext.User == null)
				return NotFound();
			if (this.HttpContext.User.Claims == null)
				return NotFound();

			var claimList = this.HttpContext.User.Claims.ToList();
			if (claimList == null)
				return NotFound();

			List<ClaimConvertItem> retList = new List<ClaimConvertItem>();
			foreach (var claim in claimList)
			{
				string name = claim.Subject != null ? claim.Subject.Name : "";
				string type = claim.Type != null ? claim.Type : "";
				string claimType = type.Split('/', '_').Last();
				ClaimConvertItem ret = new ClaimConvertItem(name, claim.Issuer, claimType, claim.Value);
				retList.Add(ret);
			}

			return Ok(retList);
		}

		//Тест асинхронный
		[HttpGet(WebApiAuthenticationClaim.GetClaimConvertItemFirstAsync)]
		public async Task<ActionResult<ClaimConvertItem>> GetClaimConvertItemFirstAsync()
		{
			if (this.HttpContext == null)
				return BadRequest();
			if (this.HttpContext.User == null)
				return NotFound();
			if (this.HttpContext.User.Claims == null)
				return NotFound();

			var array =  this.HttpContext.User.Claims.ToArray();

			if (array.Length == 0)
			{
				return NotFound();
			}
			string name = array[0].Subject != null ? array[0].Subject.Name : "";
			string type = array[0].Type != null ? array[0].Type : "";
			string claimType = type.Split('/', '_').Last();
			ClaimConvertItem ret = new ClaimConvertItem(name, array[0].Issuer, claimType, array[0].Value);
			return Ok(ret);
		}


		[HttpGet(WebApiAuthenticationClaim.GetClaimEnumList)]
		public ActionResult<IEnumerable<ClaimEnum>> GetClaimEnumList()
		{
			var list =  ClaimItems.ClaimItemsToDisplay(typeof(ClaimEnum)) as IEnumerable<ClaimEnum>;
			return Ok(list.ToList());
		}

	
		[HttpGet(WebApiAuthenticationClaim.GetClaimValue)]
		public ActionResult<string> GetClaimValue([FromRoute] string key)     //Не используйте, [FromRoute]когда значения могут содержать %2f(то есть /). %2fне будет неэкранированный к /. Используйте, [FromQuery]
		{
			if (this.HttpContext == null)
				return Ok("");
			if (this.HttpContext.User == null)
				return Ok("");
			var identity = this.HttpContext.User;
			if (identity == null)
				return Ok("");

			var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
			if (claim == null)
				return Ok("");
			return Ok(claim.Value);
		}


		//private void AddUpdateClaim(IPrincipal currentPrincipal, string key, string value)
		//{
		//	if (currentPrincipal == null)
		//		return;
		//	var identity = currentPrincipal.Identity as ClaimsIdentity;
		//	if (identity == null)
		//		return;

		//	// check for existing claim and remove it
		//	var existingClaim = identity.FindFirst(key);
		//	if (existingClaim != null)
		//		identity.RemoveClaim(existingClaim);

		//	// add new claim
		//	identity.AddClaim(new Claim(key, value));
		//	//var result = await HttpContext.AuthenticateAsync();

		//	//var authenticationManager = this.HttpContext.Current.GetOwinContext().Authentication;
		//	//authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
		//}

	}
}


//Claims
//Возвращает коллекцию объектов Claim, описывающих утверждения для пользователя.

//AddClaim(claim)
//Добавляет новое утверждение к информации о пользователе.

//AddClaims(claims)
//Добавляет список утверждений к информации о пользователе.

//HasClaim(predicate)
//Возвращает true, если информация о пользователе содержит утверждение с заданным предикатом.

//RemoveClaim(claim)
//Удаляет утверждение для пользователя.

//Issuer
//Возвращает название источника, откуда поступают данные о пользователе

//Subject
//Возвращает базовый объект ClaimsIdentity, связанный с текущим утверждением

//Type
//Возвращает тип информации об утверждении

//Value
//Возвращает информацию об утверждении

//public class LocationClaimsProvider
//{
//	public static IEnumerable<Claim> GetClaims(ClaimsIdentity user)
//	{
//		List<Claim> claims = new List<Claim>();

//		if (user.Name.ToLower() == "елена")
//		{
//			claims.Add(CreateClaim(ClaimTypes.PostalCode, "DC 20500"));
//			claims.Add(CreateClaim(ClaimTypes.StateOrProvince, "DC"));
//		}
//		else
//		{
//			claims.Add(CreateClaim(ClaimTypes.PostalCode, "NY 10036"));
//			claims.Add(CreateClaim(ClaimTypes.StateOrProvince, "NY"));
//		}
//		return claims;
//	}

//	private static Claim CreateClaim(string type, string value)
//	{
//		return new Claim(type, value, ClaimValueTypes.String, "RemoteClaims");
//	}
//}

//public class ClaimsRoles
//{
//	public static IEnumerable<Claim> CreateRolesFromClaims(ClaimsIdentity user)
//	{
//		List<Claim> claims = new List<Claim>();

//		if (user.HasClaim(x => x.Type == ClaimTypes.StateOrProvince
//				&& x.Issuer == "RemoteClaims" && x.Value == "DC")
//				&& user.HasClaim(x => x.Type == ClaimTypes.Role
//				&& x.Value == "Employees"))
//		{
//			claims.Add(new Claim(ClaimTypes.Role, "DCStaff"));
//		}

//		return claims;
//	}
//}

//==	 how use
//ident.AddClaims(LocationClaimsProvider.GetClaims(ident));
//ident.AddClaims(ClaimsRoles.CreateRolesFromClaims(ident));

//=====================================		//Фильтр авторизации в примере 
//	using System.Security.Claims;
//using System.Web;
//using System.Web.Mvc;

//namespace Users.Infrastructure
//{
//	public class ClaimsAccessAttribute : AuthorizeAttribute
//	{
//		public string Issuer { get; set; }
//		public string ClaimType { get; set; }
//		public string Value { get; set; }

//		protected override bool AuthorizeCore(HttpContextBase context)
//		{
//			return context.User.Identity.IsAuthenticated
//						&& context.User.Identity is ClaimsIdentity
//						&& ((ClaimsIdentity)context.User.Identity).HasClaim(x =>
//								x.Issuer == Issuer && x.Type == ClaimType && x.Value == Value
//						);
//		}
//	}
//}

//using Users.Infrastructure;

//namespace Users.Controllers
//{
//	public class ClaimsController : Controller
//	{
//		// ...

//		[ClaimsAccess(Issuer = "RemoteClaims", ClaimType = ClaimTypes.PostalCode,
//			Value = "DC 20500")]
//		public string OtherAction()
//		{
//			return "Это защищенный метод действия";
//		}
//	}
//}
