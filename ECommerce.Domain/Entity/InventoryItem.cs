using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entity
{
    public class InventoryItem
    {
        public int ProductId { get; set; }
        public int InventoryReservationId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }= null!;
        public InventoryReservation InventoryReservation { get; set; }= null!;


    }
}
