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
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => new { ci.CartId, ci.ProductId });
            builder.Property(ci => ci.Quantity)
                .IsRequired();

            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .IsRequired();

            builder.HasOne(ci => ci.Product)
               .WithMany(p => p.CartItems)
               .HasForeignKey(ci => ci.ProductId)
               .IsRequired();

            builder.ToTable("CartItems", t =>
            {
                t.HasCheckConstraint(
                    "CK_CartItem_Quantity",
                    "[Quantity] > 0");
            });
        }
    }
}
