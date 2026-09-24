using ECommerce.API.Common;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos.Pagination;
using ECommerce.Application.Dtos.Product;
using ECommerce.Application.Interface.Product;
using ECommerce.Application.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product/available
        [HttpGet]
       
        public async Task<IActionResult> GetAvailableProducts(
            [FromQuery] int? categoryId,[FromQuery] PaginationRequest request)
        {
            var result = await _productService
                .GetAvailableProductsAsync(categoryId,request);

           if(!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<PagedResult<GetProductResponse>>
                {
                    Success = false,
                    Message = result.Error!,
                    Data = null,
                    Errors = new List<string> { result.Error! }
                });
            }
            return Ok(new ApiResponse<PagedResult<GetProductResponse>>
            {
                Success = true,
                Message = "Available products retrieved successfully.",
                Data = result.Data,
                Errors = null
            });
        }


        [HttpGet("{id}")]
        
        public async Task<IActionResult> GetAvailableProductById(int id)
        {
            var result = await _productService.GetAvailableProductByIdAsync(id);

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
