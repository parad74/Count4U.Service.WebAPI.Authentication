using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Count4U.Service.Core.Server
{
    public class Program
	{
		public static void Main(string[] args)
		{
           BuildWebHost(args).Run();
         //   BuildWebHost(args).MigrateDatabase<ApplicationDbContext>().Run();
        }

		public static IWebHost BuildWebHost(string[] args) =>
		WebHost.CreateDefaultBuilder(args)
		  .UseUrls(	"http://0.0.0.0:52025")
		  .UseStartup<StartupAuthenticationWebAPI>() 
		 .Build();
 
    }

    //ngrok http 52025 -host-header=localhost:52025
    //http://414b1a82.ngrok.io/swagger/index.html
    //http://blazorhelpwebsite.com/Blog/tabid/61/EntryId/4366/Deploying-Your-Blazor-App-Using-Azure-Pipelines.aspx

    //https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/april/data-points-ef-core-in-a-docker-containerized-app
    //public static class ProgramExtention        //временно
    //{
    //    public static IWebHost MigrateDatabase<T>(this IWebHost webHost) where T : DbContext
    //    {
    //        using (var scope = webHost.Services.CreateScope()) {
    //            var services = scope.ServiceProvider;
    //            try {
    //                var db = services.GetRequiredService<T>();
    //                db.Database.Migrate();
    //            }
    //            catch (Exception ex) {
    //                var logger = services.GetRequiredService<ILogger<Program>>();
    //                logger.LogError(ex, "An error occurred while migrating the database.");
    //            }
    //        }
    //        return webHost;
    //    }
    //}
 }


//https://weblog.west-wind.com/posts/2016/sep/28/external-network-access-to-kestrel-and-iis-express-in-aspnet-core
//netsh advfirewall firewall add rule name = "Http Port 52025" dir=in action=allow protocol = TCP localport=52025

//nuget key BOSDimexNextLineKey oy2p54jintwxxjbfiirqbs4ibcnpxbuelj3vkujctvzdkq
//dotnet nuget push Count4U.Service.Common.1.0.0.nupkg -k oy2p54jintwxxjbfiirqbs4ibcnpxbuelj3vkujctvzdkq -s https://api.nuget.org/v3/index.json
//dotnet nuget push Count4U.Service.Model.1.0.0.nupkg -k oy2p54jintwxxjbfiirqbs4ibcnpxbuelj3vkujctvzdkq -s https://api.nuget.org/v3/index.json

//!!! Docker MSSQL
//https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/april/data-points-ef-core-in-a-docker-containerized-app