using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Carts
{
    public class AddCartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
