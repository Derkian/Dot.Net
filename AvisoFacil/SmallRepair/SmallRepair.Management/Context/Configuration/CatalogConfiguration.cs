using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Context.Configuration
{
    public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder.HasKey(catalog => catalog.IdCatalog)
                    .HasName("IdCatalogo");

            builder.Property(catalog => catalog.Code)
                .HasMaxLength(100);

            builder.Property(catalog => catalog.Description)
                .HasMaxLength(300);                

            builder.ToTable("Catalog");
        }
    }
}
