using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class ServiceValueConfiguration : IEntityTypeConfiguration<ServiceValue>
    {
        public void Configure(EntityTypeBuilder<ServiceValue> builder)
        {
            builder.HasKey(a => a.IdServiceValue);

            builder.HasOne(a => a.Customer)
                .WithMany(a => a.ServiceValues)
                .HasForeignKey(a => a.IdCustomer);

            builder.ToTable("CustomerServiceValue");
        }
    }
}
