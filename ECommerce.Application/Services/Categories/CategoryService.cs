using ECommerce.Application.Abstraction.Repository;
using ECommerce.Application.Dtos.Category;
using ECommerce.Application.Interface.Category;
using ECommerce.Application.Results;
using ECommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services.Categories
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<ArchiveCategoryResponse>> ChangeCategoryStatusAsync(int id, bool isArchived)
        {
            var category = await _categoryRepository.GetById(id);
            if (category == null)
            {
                return Result<ArchiveCategoryResponse>.Failure(
                    "Category not found."
                );
            }
            if (category.IsArchived == isArchived)
            {
                return Result<ArchiveCategoryResponse>.Failure(
                    "Category status is already the same."
                );
            }
            category.IsArchived = isArchived;
            await _categoryRepository.UpdateAsync(category);
            return Result<ArchiveCategoryResponse>.Success(
                new ArchiveCategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsArchived = category.IsArchived,
                }
            );

        }


        // Create category
        public async Task<Result<CreateCategoryResponse>> CreateAsync
            (CreateCategoryRequest request)
        {
            var existingcategory= await _categoryRepository.ExistsByNameAsync(request.Name);
            if (existingcategory) {
                return Result<CreateCategoryResponse>.Failure(
                     "Category already exists."
                ); 
            }
            var category = new Category
            {
                Name = request.Name,
            };
            await _categoryRepository.AddAsync(category);
            var response = new CreateCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
            };
            return Result<CreateCategoryResponse>.Success( response );
        }

        public async Task<Result<List<CategoryResponse>>> GetActiveAsync()
        {
            var categories = await _categoryRepository.GetActiveAsync();
            return Result<List<CategoryResponse>>.Success(
                categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                }).ToList()
            );
        }

        public async Task<Result<List<CategoryResponse>>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
             
                return Result<List<CategoryResponse>>.Success(
                    categories.Select(c => new CategoryResponse
                    {
                        Id = c.Id,
                        Name = c.Name,
                      
                    }).ToList()
                );
            
        }





        // Update category
        public async Task<Result<UpdataCategoryResponse>> UpdateAsync
            (int id,UpdataCategoryRequest request)
        {
            var category = await _categoryRepository.GetById(id);
            if (category == null) { 
                return Result<UpdataCategoryResponse>.Failure
                (
                "Category not found."
                );
            }

            if (category.Name == request.Name)
            {
                return Result<UpdataCategoryResponse>.Failure(
                    "Category name is already the same."
                );
            }

            var existingcategory = await _categoryRepository.ExistsByNameAsync(request.Name);

           

            if (existingcategory)
            {
                return Result<UpdataCategoryResponse>.Failure(
                    "Category already exists."
                );
            }
            category.Name = request.Name;
            await _categoryRepository.UpdateAsync(category);
            return Result<UpdataCategoryResponse>.Success(
                new UpdataCategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                }
            );

        }
    }
}
