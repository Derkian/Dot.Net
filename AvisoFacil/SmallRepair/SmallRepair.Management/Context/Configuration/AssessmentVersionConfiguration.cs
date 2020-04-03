using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class AssessmentVersionConfiguration : IEntityTypeConfiguration<AssessmentVersion>
    {
        public void Configure(EntityTypeBuilder<AssessmentVersion> builder)
        {
            builder.HasKey(a => a.IdAssessmentVersion);

            builder.Property(a => a.AssessmentData)
                .HasColumnType("varchar(max)");

            builder.Property(a => a.Email)
                .HasMaxLength(300);

            builder.ToTable("AssessmentVersion");
        }
    }
}
