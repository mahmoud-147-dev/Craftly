using ECommerce.Application.Abstraction.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Payment
{
    public class MockPaymentProvider : IPaymentProvider
    {
        public async Task<PaymentResult> ProcessPaymentAsync(decimal amount)
        {
            await Task.Delay(500);
            return new PaymentResult { 
                IsSuccess = true,
                ReferenceId=Guid.NewGuid().ToString()
            };

        }
    }
}
