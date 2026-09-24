using ECommerce.Application.Abstraction.Repository;
using ECommerce.Domain.Entity;
using ECommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.PaymentAttempts
{
    public class PaymentAttemptRepository : IPaymentAttemptRepository
    {
        private readonly AppDbContext _context;
        public PaymentAttemptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PaymentAttempt paymentAttempt)
        {
            await _context.PaymentAttempts.AddAsync(paymentAttempt);
            
        }
    }
}
