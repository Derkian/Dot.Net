using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmallRepair.Management.Model;


namespace SmallRepair.Management.Context.Configuration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(customer => customer.IdCompany);

            builder.Property(customer => customer.Name)
                .HasMaxLength(200);

            builder.Property(customer => customer.IdCompany)
                .HasMaxLength(450);

            builder.ToTable("Company");
        }
    }
}
