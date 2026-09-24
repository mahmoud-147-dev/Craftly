using ECommerce.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

           builder.Property(P => P.Price)
                .HasPrecision(18, 2);
            builder.Property(p => p.StockQuantity)
                .IsRequired();
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);


            builder.ToTable("Products", t =>
            {
                t.HasCheckConstraint(
                    "CK_Product_Price",
                    "[Price] > 0");

                t.HasCheckConstraint(
                    "CK_Product_StockQuantity",
                    "[StockQuantity] >= 0");
            });
        }
    }
}
