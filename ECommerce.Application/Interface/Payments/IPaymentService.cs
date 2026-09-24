using ECommerce.Application.Dtos.Payments;
using ECommerce.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interface.Payments
{
    public interface IPaymentService
    {
        Task<Result<PaymentResponse>> PayAsync(
            string userId,
            int orderId);
       
    }
}
