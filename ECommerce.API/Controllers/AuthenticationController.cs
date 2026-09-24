using ECommerce.API.Common;
using ECommerce.Application.Dtos.Authentication;
using ECommerce.Application.Interface.Authentication;
using ECommerce.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;  
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {


            var result = await _authenticationService.RegisterAsync(request);
            if (!result.IsSuccess)
            {
                return Conflict(new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = result.Error,
                    Data = null
                });
            }

            return Ok(new ApiResponse<RegisterResponse>
            {
                Success = true,
                Message = "User registered successfully.",
                Data = result.Data
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);
            if (!result.IsSuccess) {
                return Unauthorized(new ApiResponse<LoginResponse> {
                    Success = false,
                    Message = result.Error!,
                    Data = null

                }
                );
            }

            return Ok(new ApiResponse<LoginResponse>
            {
                Success= true,
                Message="Login Successful",
                Data = result.Data
            }
                
             );
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult TestAuthentication()
        {
            return Ok("You are authenticated.");
        }
    }
}
