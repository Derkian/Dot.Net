using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmallRepair.Management.Model;


namespace SmallRepair.Management.Context.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(customer => customer.IdCustomer);

            builder.Property(customer => customer.Name)
                .HasMaxLength(200);

            builder.Property(customer => customer.ClaimId)
                .HasMaxLength(450);

            builder.ToTable("Customer");
        }
    }
}
