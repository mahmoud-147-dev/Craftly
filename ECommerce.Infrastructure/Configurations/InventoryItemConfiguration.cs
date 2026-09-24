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
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.HasKey(i => new { i.ProductId, i.InventoryReservationId });

            builder.Property(ii => ii.Quantity)
                .IsRequired();

            builder.HasOne(i => i.Product)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(i => i.ProductId)
                .IsRequired();

            builder.HasOne(i => i.InventoryReservation)
                .WithMany(ir => ir.InventoryItems)
                .HasForeignKey(i => i.InventoryReservationId)
                .IsRequired();

            builder.ToTable("InventoryItems", t =>
            {
                t.HasCheckConstraint(
                    "CK_InventoryItem_Quantity",
                    "[Quantity] > 0");
            });
        }
    }
}
