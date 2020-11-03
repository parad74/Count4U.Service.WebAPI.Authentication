using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Count4U.Service.Core.Server.Data
{
        public class AdminWithRolesConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
        {
        private const string adminUserId = "6df513bb-c6e1-4791-9cf8-15fbf1c79b9e";
        private const string adminRoleId = "5f998083-c061-4f3a-9589-7aeedaa306c8";

        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
            {
                IdentityUserRole<string> iur = new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                };

                builder.HasData(iur);
            }
        }
    }
