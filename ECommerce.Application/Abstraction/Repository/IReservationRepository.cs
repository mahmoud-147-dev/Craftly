using ECommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Repository
{
    public interface IReservationRepository
    {
        Task AddAsync(InventoryReservation reservation);
        Task<InventoryReservation?> GetByOrderIdAsync(int orderId);
    }
}
