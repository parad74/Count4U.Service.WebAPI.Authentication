using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Count4U.Service.Core.Server.Data
{
	
	public class ApplicationDbContext : IdentityDbContext  <ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Owner", NormalizedName = "OWNER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "User", NormalizedName = "USER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });  //users and roles
			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Manager", NormalizedName = "MANAGER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() }); //For all profiles, process, customers params
			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Monitor", NormalizedName = "MONITOR", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });  //metriks 
			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Context", NormalizedName = "CONTEXT", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });   //profile view
			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Worker", NormalizedName = "WORKER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });	   //Inventor in field
			builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Profile", NormalizedName = "PROFILE", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });	  //my Profile Edit
		}

	}


	//public class ApplicationDbContext : IdentityDbContext
	//{
	//    public ApplicationDbContext(DbContextOptions options) : base(options)
	//    {
	//    }
	//}
}


//[Authorize]
//public async Task<IActionResult> UserSetup()
//{
//	if (!await _roleManager.RoleExistsAsync("Admin")) {
//		var role = new IdentityRole { Name = "Admin" };
//		await _roleManager.CreateAsync(role);
//	}

//	// add role to user
//	var user = await _userManager.GetUserAsync(HttpContext.User);
//	if (!await _userManager.IsInRoleAsync(user, "Admin")) {
//		await _userManager.AddToRoleAsync(user, "Admin");
//	}

//	return View();
//}
//см IdentityServer4Example
//public class IdServer4ExampleDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
//{
//	public IdServer4ExampleDbContext(DbContextOptions<IdServer4ExampleDbContext> options)
//		: base(options)
//	{
//	}

//	public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }

//	protected override void OnModelCreating(ModelBuilder builder)
//	{
//		builder.Entity<ApplicationUser>().HasKey(p => p.Id);

//		base.OnModelCreating(builder);
//	}
//}

//////!!!!!!!!!!
///add-migration 
///update-database
///
//Add-Migration SeedRoles
//Update-Database

//Add-Migration ApplicationUser  -Context ApplicationDbContext
//Update-Database  -Context ApplicationDbContext

//enable-migrations -ContextProjectName MyProject.DBContexts -contexttypename MyProject.DBContexts.MyContextName -Verbose
//enable-migrations -ContextProjectName Count4U.Service.Server.Extensions -contexttypename Count4U.Service.Core.Server.Data.ApplicationDbContext -Verbose