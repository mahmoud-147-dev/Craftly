using ECommerce.Application.Common;
using ECommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Repository
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task <bool> ExistsByNameAsync(string Name,int categoryId);
        Task UpdateAsync(Product product);
        Task<PagedResult<Product>> GetAvailableProductsAsync(int? categoryId,int pageNumber,int pageSize);
        Task<PagedResult<Product>> GetAllProductsAsync(int? categoryId, int pageNumber, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetAvailableByIdAsync(int id);
        Task<bool> TryReserveStockAsync(int productId, int quantity);
        Task<bool> FinalizeStockAsync( int productId,int quantity);
    }
}
