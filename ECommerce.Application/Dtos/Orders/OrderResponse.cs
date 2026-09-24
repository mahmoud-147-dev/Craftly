using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Orders
{
    public class OrderResponse
    {
        
        
            public int Id { get; set; }
            public OrderStatus Status { get; set; }
            public DateTime CreatedAt { get; set; }

            public List<OrderItemResponse> Items { get; set; } = new();

            public decimal Total { get; set; }
        
    }
}
