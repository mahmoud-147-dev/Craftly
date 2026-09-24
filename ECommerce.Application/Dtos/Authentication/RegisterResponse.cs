using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Authentication
{
    public class RegisterResponse
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        
    }
}
