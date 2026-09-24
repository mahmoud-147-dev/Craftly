using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Carts
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public List<CartItemResponse> Items { get; set; } = new();
        public decimal Total { get; set; }
    }
}
