using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmallRepair.Management.Model;


namespace SmallRepair.Management.Context.Configuration
{
    public class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.HasKey(p => p.IdPart);

            builder.Property(p => p.Code)
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(250);

            builder.HasOne(p => p.Assessment)
                .WithMany(p => p.Parts)
                .HasForeignKey(p => p.IdAssessment);                
        }
    }
}
