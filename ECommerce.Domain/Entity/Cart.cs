using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entity
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();


    }
}
