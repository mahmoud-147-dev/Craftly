using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Payments
{
    public class PaymentResponse
    {
        public int PaymentAttemptId { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public string? ReferenceId { get; set; }

        public string? FailureReason { get; set; }
    }
}
