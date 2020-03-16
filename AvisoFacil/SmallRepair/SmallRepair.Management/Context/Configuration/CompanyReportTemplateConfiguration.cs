using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class CompanyReportTemplateConfiguration : IEntityTypeConfiguration<CompanyReportTemplate>
    {
        public void Configure(EntityTypeBuilder<CompanyReportTemplate> builder)
        {
            builder.HasKey(a => new { a.IdCompany, a.Code });            

            builder.Property(a => a.Code).HasMaxLength(400);

            builder.Property(a => a.Template)
                .HasColumnType("varchar(max)");

            builder.HasOne(a => a.Company)
                .WithMany(a => a.ReportTemplates)
                .HasForeignKey(a => a.IdCompany);

            builder.ToTable("CompanyReportTemplate");
        }
    }
}
