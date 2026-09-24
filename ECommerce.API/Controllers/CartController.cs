using ECommerce.API.Common;
using ECommerce.Application.Dtos.Carts;
using ECommerce.Application.Interface.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.GetCartAsync(userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<CartResponse>
                {
                    Success = false,
                    Message = result.Error!
                });
            }

            return Ok(new ApiResponse<CartResponse>
            {
                Success = true,
                Message = "Cart retrieved successfully.",
                Data = result.Data
            });
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem(
            AddCartItemRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.AddItemToCartAsync(
                userId!,
                request);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<CartResponse>
                {
                    Success = false,
                    Message = result.Error!
                });
            }

            return Ok(new ApiResponse<CartResponse>
            {
                Success = true,
                Message = "Product added to cart successfully.",
                Data = result.Data
            });
        }

        [HttpPut("items")]
        public async Task<IActionResult> UpdateItem(
            UpdateCartItemRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.UpdateCartItemAsync(
                userId!,
                request);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<CartResponse>
                {
                    Success = false,
                    Message = result.Error!
                });
            }

            return Ok(new ApiResponse<CartResponse>
            {
                Success = true,
                Message = "Cart item updated successfully.",
                Data = result.Data
            });
        }

        [HttpDelete("items/{productId:int}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.RemoveItemFromCartAsync(
                userId!,
                productId);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<CartResponse>
                {
                    Success = false,
                    Message = result.Error!
                });
            }

            return Ok(new ApiResponse<CartResponse>
            {
                Success = true,
                Message = "Product removed from cart successfully.",
                Data = result.Data
            });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.ClearCartAsync(userId!);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<CartResponse>
                {
                    Success = false,
                    Message = result.Error!
                });
            }

            return Ok(new ApiResponse<CartResponse>
            {
                Success = true,
                Message = "Cart cleared successfully.",
                Data = result.Data
            });
        }
    }
}
