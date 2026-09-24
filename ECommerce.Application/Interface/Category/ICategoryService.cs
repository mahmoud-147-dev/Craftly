using ECommerce.Application.Dtos.Category;
using ECommerce.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interface.Category
{
    public interface ICategoryService
    {
        Task<Result<CreateCategoryResponse>> CreateAsync(
        CreateCategoryRequest request);

        Task<Result<UpdataCategoryResponse>> UpdateAsync(int id,UpdataCategoryRequest request);
        Task<Result<ArchiveCategoryResponse>> ChangeCategoryStatusAsync(int id, bool isArchived);
        Task<Result<List<CategoryResponse>>> GetAllAsync();
        Task<Result<List<CategoryResponse>>> GetActiveAsync();
    }
}
