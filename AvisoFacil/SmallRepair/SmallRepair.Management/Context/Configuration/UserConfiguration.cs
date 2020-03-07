using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.IdUser);

            builder.Property(u => u.ClaimId)
                .HasMaxLength(450);

            builder.Property(u => u.Name)
                .HasMaxLength(300);

            builder.Property(u => u.Email)
                .HasMaxLength(200);

            builder.HasOne(u => u.Customer)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.IdCustomer);
        }
    }
}
