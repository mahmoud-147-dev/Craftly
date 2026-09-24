using ECommerce.API.Common;
using ECommerce.Application.Dtos.Payments;
using ECommerce.Application.Interface.Payments;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService) { 
            _paymentService = paymentService;
        }

        [HttpPost("{orderId:int}")]
        public async Task<IActionResult> Pay(int orderId)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _paymentService
                .PayAsync(userId, orderId);

            if (!result.IsSuccess)
            {
                return BadRequest(
                    new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = result.Error
                                  ?? "Payment failed.",
                        Data = null
                    });
            }

            return Ok(
                new ApiResponse<PaymentResponse>
                {
                    Success = true,
                    Message = result.Data?.Status
                              == PaymentStatus.Successful
                        ? "Payment successful."
                        : "Payment failed.",

                    Data = result.Data
                });
        }
    }
}
