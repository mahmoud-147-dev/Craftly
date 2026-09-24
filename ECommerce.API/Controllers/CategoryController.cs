using ECommerce.API.Common;
using ECommerce.Application.Dtos.Category;
using ECommerce.Application.Interface.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<CreateCategoryResponse>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }

            return Ok(new ApiResponse<CreateCategoryResponse>
            {
                Success = true,
                Message = "Category created successfully.",
                Data = result.Data,
                Errors = null
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdataCategoryRequest request)
        {
            var result = await _categoryService.UpdateAsync(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<UpdataCategoryResponse>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }
            return Ok(new ApiResponse<UpdataCategoryResponse>
            {
                Success = true,
                Message = "Category updated successfully.",
                Data = result.Data,
                Errors = null
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeCategoryStatus(int id, [FromQuery] bool isArchived)
        {
            var result = await _categoryService.ChangeCategoryStatusAsync(id, isArchived);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<ArchiveCategoryResponse>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }
            return Ok(new ApiResponse<ArchiveCategoryResponse>
            {
                Success = true,
                Message = "Category status changed successfully.",
                Data = result.Data,
                Errors = null
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllAsync();

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<List<CategoryResponse>>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }

            return Ok(new ApiResponse<List<CategoryResponse>>
            {
                Success = true,
                Message = "Categories retrieved successfully.",
                Data = result.Data,
                Errors = null
            });
        }

        [Authorize]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCategories()
        {
            var result = await _categoryService.GetActiveAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<List<CategoryResponse>>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }
            return Ok(new ApiResponse<List<CategoryResponse>>
            {
                Success = true,
                Message = "Active categories retrieved successfully.",
                Data = result.Data,
                Errors = null
            });
        }
    }
}
