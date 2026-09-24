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
    public class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservation>
    {
        public void Configure(EntityTypeBuilder<InventoryReservation> builder)
        {
            builder.HasKey(P => P.Id);
            builder.Property(P => P.ExpirationAt)
                .IsRequired();
            builder.Property(ir => ir.Status)
            .IsRequired()
            .HasConversion<string>();

            builder.HasOne(ir => ir.Order)
             .WithOne()
             .HasForeignKey<InventoryReservation>(ir => ir.OrderId)
             .IsRequired();

        }
    }
}
