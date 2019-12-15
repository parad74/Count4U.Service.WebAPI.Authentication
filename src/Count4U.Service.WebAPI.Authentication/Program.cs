using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Count4U.Service.Core.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
		WebHost.CreateDefaultBuilder(args)
		  .UseUrls(	"http://0.0.0.0:52025")
		  .UseStartup<StartupAuthenticationWebAPI>() 
		 .Build();
	}
}
//https://weblog.west-wind.com/posts/2016/sep/28/external-network-access-to-kestrel-and-iis-express-in-aspnet-core
//netsh advfirewall firewall add rule name = "Http Port 52025" dir=in action=allow protocol = TCP localport=52025

//nuget key BOSDimexNextLineKey oy2p54jintwxxjbfiirqbs4ibcnpxbuelj3vkujctvzdkq
//dotnet nuget push Count4U.Service.Common.1.0.0.nupkg -k oy2p54jintwxxjbfiirqbs4ibcnpxbuelj3vkujctvzdkq -s https://api.nuget.org/v3/index.json
//dotnet nuget push Count4U.Service.Model.1.0.0.nupkg -k oy2p54jintwxxjbfiirqbs4ibcnpxbuelj3vkujctvzdkq -s https://api.nuget.org/v3/index.json
