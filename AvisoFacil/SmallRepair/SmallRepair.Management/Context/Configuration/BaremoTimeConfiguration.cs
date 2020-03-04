using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class BaremoTimeConfiguration : IEntityTypeConfiguration<BaremoTime>
    {
        public void Configure(EntityTypeBuilder<BaremoTime> builder)
        {
            builder.HasKey(baremoTime => new
            {
                baremoTime.IdBaremo,
                baremoTime.IdCatalog,
                baremoTime.ServiceType
            });

            builder.Property(baremoTime => baremoTime.MaterialValue);

            builder.Property(baremoTime => baremoTime.ServiceTime);
            
            builder.HasOne(s => s.Baremo)
                   .WithMany(s => s.BaremoTimes)
                   .HasForeignKey(s => s.IdBaremo);

            builder.HasOne(s => s.Catalog)
                   .WithMany(s => s.BaremoTimes)
                   .HasForeignKey(s => s.IdCatalog);

            builder.ToTable("BaremoTime");
        }
    }
}
