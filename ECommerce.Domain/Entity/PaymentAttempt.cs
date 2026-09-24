using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entity
{
    public class PaymentAttempt
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public string? ReferenceId { get; set; }

        public string? FailureReason { get; set; }

        public string? ProviderResponse { get; set; }

        public DateTime CreatedAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}
