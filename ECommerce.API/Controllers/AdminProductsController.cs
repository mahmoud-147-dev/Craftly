using ECommerce.API.Common;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos.Pagination;
using ECommerce.Application.Dtos.Product;
using ECommerce.Application.Interface.Product;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<CreateProductRequest> _validator;
        private readonly IValidator<UpdateProductRequest> _updateValidator;

        public AdminProductsController(IProductService productService,
            IValidator<CreateProductRequest> validator,
            IValidator<UpdateProductRequest> updateValidator)
        {
            _productService = productService;
            _validator = validator;
            _updateValidator = updateValidator;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<CreateProductResponse>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = errors
                });
            }


            var result = await _productService.CreateAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<CreateProductResponse>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }
            return StatusCode(201,new ApiResponse<CreateProductResponse>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = result.Data,
                Errors = null
            });
        }

        [HttpPut("{id}")]
      
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromBody] UpdateProductRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<UpdateProductResponse>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = errors
                });
            }

            var result = await _productService.UpdateAsync(id, request);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<UpdateProductResponse>
                {
                    Success = false,
                    Message = result.Error,
                    Data = null,
                    Errors = new List<string> { result.Error }
                });
            }

            return Ok(new ApiResponse<UpdateProductResponse>
            {
                Success = true,
                Message = "Product updated successfully.",
                Data = result.Data,
                Errors = null
            });
        }


        [HttpPatch("{id}/status")]
       
        public async Task<IActionResult> ChangeProductStatus(int id)
        {
            var result = await _productService.ChangeStatusAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<ChangeProductStatusResponse>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }

            

            return Ok(new ApiResponse<ChangeProductStatusResponse>
            {
                Success = true,
                Message = "Product status changed successfully.",
                Data = result.Data,
                Errors = null
            });
        }

        [HttpGet("admin")]
       
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] int? categoryId, [FromQuery] PaginationRequest request)
        {
            var result = await _productService
                .GetAllProductsAsync(categoryId,request);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<PagedResult<GetProductResponse>> { 
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }

            return Ok(new ApiResponse<PagedResult<GetProductResponse>>
            {
                Success = true,
                Message = "Products retrieved successfully.",
                Data = result.Data,
                Errors = null
            });

        }

        [HttpGet("admin/{id}")]
        
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(new ApiResponse<GetProductResponse>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }

            return Ok(new ApiResponse<GetProductResponse>
            {
                Success = true,
                Message = "Product retrieved successfully.",
                Data = result.Data,
                Errors = null
            });
        }


    }
}
