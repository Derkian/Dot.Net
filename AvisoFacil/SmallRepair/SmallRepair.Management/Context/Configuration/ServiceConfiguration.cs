using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmallRepair.Management.Model;


namespace SmallRepair.Management.Context.Configuration
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(a => a.IdService);

            builder.HasOne(a => a.Part)
                .WithMany(a => a.Services)
                .HasForeignKey(a => a.IdPart);
        }
    }
}
