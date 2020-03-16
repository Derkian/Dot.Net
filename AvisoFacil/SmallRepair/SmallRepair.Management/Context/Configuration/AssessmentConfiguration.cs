using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
    {
        public void Configure(EntityTypeBuilder<Assessment> builder)
        {
            builder.HasKey(a => a.IdAssessment);

            builder.Property(a => a.Plate)
                .HasMaxLength(10);

            builder.Property(a => a.Model)
                .HasMaxLength(300);

            builder.Property(a => a.Mileage)
                .HasMaxLength(100);

            builder.Property(a => a.CustomerName)
                .HasMaxLength(300);

            builder.Property(a => a.BodyType)
                .HasMaxLength(50);

            builder.Property(a => a.BodyType)
                .HasMaxLength(100);

            //Peças
            builder.HasMany(a => a.Parts)
                .WithOne(a => a.Assessment)
                .HasForeignKey(a => a.IdAssessment);

            //Cliente
            builder.HasOne(a => a.Company)
                .WithMany(a => a.Assessments)
                .HasForeignKey(a => a.IdCompany);

            //Serviço Adicional
            builder.HasMany(a => a.AssessmentAdditionalServices)
                .WithOne(a => a.Assessment)
                .HasForeignKey(a => a.IdAssessment);
        }
    }
}
