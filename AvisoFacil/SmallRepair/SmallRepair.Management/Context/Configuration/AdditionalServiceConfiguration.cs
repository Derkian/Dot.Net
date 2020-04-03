using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class AdditionalServiceConfiguration : IEntityTypeConfiguration<AdditionalService>
    {
        public void Configure(EntityTypeBuilder<AdditionalService> builder)
        {
            builder.HasKey(a => a.IdAdditionalService);

            builder.Property(a => a.Description)
                .HasMaxLength(200);

            builder.HasOne(u => u.Company)
                .WithMany(u => u.AdditionalServices)
                .HasForeignKey(u => u.IdCompany);
        }
    }
}
