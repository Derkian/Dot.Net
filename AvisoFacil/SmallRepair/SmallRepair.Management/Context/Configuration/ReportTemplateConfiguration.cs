using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
    {
        public void Configure(EntityTypeBuilder<ReportTemplate> builder)
        {
            builder.HasKey(a => a.Code );            

            builder.Property(a => a.Code).HasMaxLength(400);

            builder.Property(a => a.Template)
                .HasColumnType("varchar(max)");

            builder.ToTable("ReportTemplate");
        }
    }
}
