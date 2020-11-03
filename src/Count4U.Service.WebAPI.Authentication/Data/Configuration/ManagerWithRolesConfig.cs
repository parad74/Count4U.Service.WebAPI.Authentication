using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Count4U.Service.Core.Server.Data
{
        public class ManagerWithRolesConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
        {
        private const string managerCount4uLocalId = "0d37e9b7-1111-1111-1111-3d7ce13a2070";
        private const string managerRoleId = "a3f5048f-55a8-4766-b8ec-03f859b8c399";

        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
            {
                IdentityUserRole<string> iur = new IdentityUserRole<string>
                {
                    RoleId = managerRoleId,
                    UserId = managerCount4uLocalId
                };

                builder.HasData(iur);
            }
        }
    }
