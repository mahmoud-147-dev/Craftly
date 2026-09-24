using ECommerce.Application.Dtos.Orders;
using ECommerce.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interface.Orders
{
    public interface IOrderService
    {
        Task<Result<OrderResponse>> CheckoutAsync(string userId);
    }
}
