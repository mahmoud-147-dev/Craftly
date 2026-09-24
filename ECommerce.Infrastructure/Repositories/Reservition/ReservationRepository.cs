using ECommerce.Application.Abstraction.Repository;
using ECommerce.Domain.Entity;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.Reservition
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;
        public ReservationRepository(AppDbContext context) { 
            _context = context;
        }

        public async Task AddAsync(InventoryReservation reservation)
        {
            await _context.InventoryReservations.AddAsync(reservation);
        }

        public async Task<InventoryReservation?> GetByOrderIdAsync(int orderId)
        {
            return await _context.InventoryReservations
                .Include(r => r.InventoryItems)
                .FirstOrDefaultAsync(r => r.OrderId == orderId);
        }
    }
}
