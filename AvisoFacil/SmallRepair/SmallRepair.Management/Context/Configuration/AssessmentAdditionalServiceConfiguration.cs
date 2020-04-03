using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class AssessmentAdditionalServiceConfiguration
        : IEntityTypeConfiguration<AssessmentAdditionalService>
    {
        public void Configure(EntityTypeBuilder<AssessmentAdditionalService> builder)
        {
            builder.HasKey(a => a.IdAssessmentAdditionalService);

            builder.Property(a => a.Description)
                .HasMaxLength(200);

            builder.HasOne(a => a.Assessment)
                .WithMany(a => a.AdditionalServices)
                .HasForeignKey(a => a.IdAssessment);
        }
    }
}
