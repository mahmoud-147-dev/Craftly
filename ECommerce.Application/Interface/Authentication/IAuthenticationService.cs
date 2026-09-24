using ECommerce.Application.Dtos.Authentication;
using ECommerce.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interface.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
