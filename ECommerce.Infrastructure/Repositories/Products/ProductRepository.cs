using ECommerce.Application.Abstraction.Repository;
using ECommerce.Application.Common;
using ECommerce.Domain.Entity;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, int categoryId)
        {
           return await _context.Products
                .AnyAsync(p => p.Name == name && p.CategoryId == categoryId);

        }

        public async Task<PagedResult<Product>> GetAllProductsAsync(int? categoryId, int pageNumber, int pageSize)
        {
            var query = _context.Products
                .AsNoTracking();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            var totalCount = await query.CountAsync();
            query = query.
                OrderBy(p =>p.Id).
                Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            var products = await query.ToListAsync();
            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Product?> GetAvailableByIdAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id &&
                !p.IsArchived &&
                p.StockQuantity > 0&&
                !p.Category.IsArchived)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<Product>> GetAvailableProductsAsync(int? categoryId
            ,int pageNumber
            ,int pageSize)
        {
         var query = _context.Products.
                Where(p =>
                !p.IsArchived&&
                p.StockQuantity > 0&&
                !p.Category.IsArchived)
                .AsNoTracking();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            var totalCount = await query.CountAsync();
            var products = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TryReserveStockAsync(
            int productId,
            int quantity)
        {
            var rowsAffected = await _context.Products
                .Where(p =>
                    p.Id == productId &&
                    p.StockQuantity - p.ReservedQuantity >= quantity)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        p => p.ReservedQuantity,
                        p => p.ReservedQuantity + quantity
                    )
                );

            return rowsAffected == 1;
        }

        public async Task<bool> FinalizeStockAsync(
            int productId,
            int quantity)
        {
            var rowsAffected = await _context.Products
                .Where(p =>
                    p.Id == productId &&
                    p.ReservedQuantity >= quantity &&
                    p.StockQuantity >= quantity)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(
                            p => p.StockQuantity,
                            p => p.StockQuantity - quantity)
                        .SetProperty(
                            p => p.ReservedQuantity,
                            p => p.ReservedQuantity - quantity));

            return rowsAffected == 1;
        }
    }
}
