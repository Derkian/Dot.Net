using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class AssessmentServiceValueConfiguration : IEntityTypeConfiguration<AssessmentServiceValue>
    {
        public void Configure(EntityTypeBuilder<AssessmentServiceValue> builder)
        {
            builder.HasKey(a => a.IdAssessmentServiceValue);

            //Valor por servico
            builder.HasOne(a => a.Assessment)
                .WithMany(a => a.ServicesValues)
                .HasForeignKey(a => a.IdAssessment);
        }
    }
}
