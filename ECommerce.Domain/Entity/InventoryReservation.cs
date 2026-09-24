using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entity
{
    public class InventoryReservation
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationAt { get; set; }
        public ReservationStatus Status { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();


    }
}
