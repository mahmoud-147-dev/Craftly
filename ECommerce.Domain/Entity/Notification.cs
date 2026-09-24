using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entity
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }= null!;
        public int? OrderId { get; set; } 
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }
        public Order? Order { get; set; }



    }
}
