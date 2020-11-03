using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Count4U.Service.Core.Server.Data
{
    public class ManagerConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        private const string managerCount4uLocalId = "0d37e9b7-1111-1111-1111-3d7ce13a2070";

        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var count4uLocal = new ApplicationUser
            {
                Id = managerCount4uLocalId,
                UserName = "manager@count4u.local",
                NormalizedUserName = "MANAGER@COUNT4U.LOCAL",
                Email = "manager@count4u.local",
                NormalizedEmail = "MANAGER@COUNT4U.LOCAL",
                PhoneNumber = "XXXXXXXXXXXXX",
                DataServerAddress = @"http://localhost:12347",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = new Guid().ToString("D"),

            };

            count4uLocal.PasswordHash = PassGenerate(count4uLocal);

            builder.HasData(count4uLocal);
        }

        public string PassGenerate(ApplicationUser user)
        {
            var passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, "123456");
        }
    }
}
