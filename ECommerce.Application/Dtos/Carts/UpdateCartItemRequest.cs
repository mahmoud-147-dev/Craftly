using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Carts
{
    public class UpdateCartItemRequest
    {
        public int productId { get; set; }  
        public int Quantity { get; set; }
    }
}
