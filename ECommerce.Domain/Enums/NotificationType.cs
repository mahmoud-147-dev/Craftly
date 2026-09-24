using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Enums
{
    public enum NotificationType
    {
        OrderCreated,
        PaymentSuccessful,
        PaymentFailed,
        OrderShipped,
        OrderDelivered,
        ReservationExpired
    }
}
