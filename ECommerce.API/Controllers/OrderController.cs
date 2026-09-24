using ECommerce.API.Common;
using ECommerce.Application.Dtos.Orders;
using ECommerce.Application.Interface.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _orderService
                .CheckoutAsync(userId);

            if (!result.IsSuccess)
            {
                return BadRequest(
                    new ApiResponse<OrderResponse>
                    {
                        Success = false,
                        Message = result.Error
                                  ?? "Checkout failed.",
                        Data = null
                    });
            }

            return Ok(
                new ApiResponse<OrderResponse>
                {
                    Success = true,
                    Message = "Checkout completed successfully.",
                    Data = result.Data
                });
        }
    }
}
