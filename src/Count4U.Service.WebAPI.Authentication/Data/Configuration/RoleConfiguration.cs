using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Count4U.Service.Core.Server.Data
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        private const string OwnerRoleId = "799e3d1d-febd-4bd4-9c1f-26afd13bb0c9";
        private const string UserRoleId = "c401dbc0-080b-4fbe-a8a1-275cd4068eb7";
        private const string AdminRoleId = "5f998083-c061-4f3a-9589-7aeedaa306c8";
        private const string ManagerRoleId = "a3f5048f-55a8-4766-b8ec-03f859b8c399";
        private const string MonitorRoleId = "0ccc4920-47aa-4b34-acf8-77f5e0ad6c40";
        private const string ContextRoleId = "9e8f096e-c230-4917-b412-f0a5e403c499";
        private const string WorkerRoleId = "a87b2288-7534-458d-b4f6-4831239f8570";
        private const string ProfileRoleId = "38539cf9-77e9-4129-b6a6-9cfbbf3c64ac";

        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {

            builder.HasData(new IdentityRole { Id = OwnerRoleId, Name = "Owner", NormalizedName = "OWNER" });
            builder.HasData(new IdentityRole { Id = UserRoleId, Name = "User", NormalizedName = "USER"});
            builder.HasData(new IdentityRole { Id = AdminRoleId, Name = "Admin", NormalizedName = "ADMIN"});  //users and roles
            builder.HasData(new IdentityRole { Id = ManagerRoleId, Name = "Manager", NormalizedName = "MANAGER" }); //For all profiles, process, customers params
            builder.HasData(new IdentityRole { Id = MonitorRoleId, Name = "Monitor", NormalizedName = "MONITOR" });  //metriks 
            builder.HasData(new IdentityRole { Id = ContextRoleId, Name = "Context", NormalizedName = "CONTEXT"});   //profile view
            builder.HasData(new IdentityRole { Id = WorkerRoleId, Name = "Worker", NormalizedName = "WORKER"});      //Inventor in field
            builder.HasData(new IdentityRole { Id = ProfileRoleId, Name = "Profile", NormalizedName = "PROFILE" });      //my Profile Edit

        }
    }
}
