using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Payment
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string? ReferenceId { get; set; }

        public string? FailureReason { get; set; }
    }
}
