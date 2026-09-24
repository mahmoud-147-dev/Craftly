using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Payment
{
    public interface IPaymentProvider
    {
        Task<PaymentResult> ProcessPaymentAsync(
            decimal amount);
    }
}
