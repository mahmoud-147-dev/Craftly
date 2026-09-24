using ECommerce.Application.Abstraction.Repository;
using ECommerce.Domain.Entity;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string Name)
        {
            return await _context.Categories
            .AnyAsync(c => c.Name == Name);
        }

        public async Task<List<Category>> GetActiveAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsArchived)
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .ToListAsync();
        }

        public async Task<Category?> GetById(int Id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }

}

