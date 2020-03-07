using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;

namespace SmallRepair.Management.Context.Configuration
{
    public class BaremoConfiguration : IEntityTypeConfiguration<Baremo>
    {
        public void Configure(EntityTypeBuilder<Baremo> builder)
        {
            builder.HasKey(baremo => baremo.IdBaremo);                

            builder.Property(baremo => baremo.IntensityType);

            builder.Property(baremo => baremo.MalfunctionType);
            
            builder.ToTable("Baremo");
        }
    }
}
