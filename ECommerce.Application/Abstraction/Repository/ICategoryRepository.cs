using ECommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Repository
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsByNameAsync(string Name); 
        Task<Category?> GetById(int Id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetActiveAsync();
    }
}
