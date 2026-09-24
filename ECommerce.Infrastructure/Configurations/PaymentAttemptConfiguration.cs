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
    public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
        {
            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(pa => pa.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(pa => pa.ReferenceId)
                .HasMaxLength(200);

            builder.Property(pa => pa.FailureReason)
                .HasMaxLength(500);

            builder.Property(pa => pa.ProviderResponse)
                .HasMaxLength(2000);

            builder.HasOne(pa => pa.Order)
                .WithMany(o => o.PaymentAttempts)
                .HasForeignKey(pa => pa.OrderId)
                .IsRequired();
        }
    }
}
