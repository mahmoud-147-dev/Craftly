using ECommerce.Application.Dtos.Authentication;
using ECommerce.Application.Interface.Authentication;
using ECommerce.Application.Results;
using ECommerce.Infrastructure.Configurations.Options;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
       
        private readonly JwtOption _jwtOptions;
        public AuthenticationService(UserManager<ApplicationUser> userManger,
            IOptions<JwtOption> jwtOptions)
        {
            _userManager = userManger;
            _jwtOptions = jwtOptions.Value; ;
        }


        // Method to register a new user
        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            // Check if the email is already registered
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            // If the user already exists, return a failure result
            if (existingUser != null)
            {
                return Result<RegisterResponse>.Failure(
                 "Email is already registered.");
            }
            // Create a new ApplicationUser instance with the provided details
            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name
            };

            // Create the user using UserManager
            var result = await _userManager.CreateAsync(newUser, request.Password);
            // If the creation failed, return a failure result with the error messages
            if (!result.Succeeded)
            {
                var errors = string.Join(
                  ", ",
                 result.Errors.Select(e => e.Description));

                return Result<RegisterResponse>.Failure(errors);

            }
            // Assign the "Customer" role to the newly created user
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Customer");

            // If the role assignment failed, return a failure result with the error messages
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                return Result<RegisterResponse>.Failure(errors);
            }
            // If everything succeeded, return a success result with the new user's details


            var response = new RegisterResponse
            {
                UserId = newUser.Id,
                Email = newUser.Email!
            };
            return Result<RegisterResponse>.Success(response);

        }
        // Method to log in a user 
        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<LoginResponse>.Failure("Invalid email or password.");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
            {
                return Result<LoginResponse>.Failure("Invalid email or password.");
            }

            var token = await GenerateJwtTokenAsync(user);
            var response = new LoginResponse
            {
                Token = token
            };
            return Result<LoginResponse>.Success(response);
        }
        
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            
            
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(
                 securityKey,
                SecurityAlgorithms.HmacSha256
            );
            var token = new JwtSecurityToken(
                   issuer: _jwtOptions.Issuer,
                   audience: _jwtOptions.Audience,
                   claims: claims,
                   expires: DateTime.UtcNow.AddMinutes(
                       _jwtOptions.DurationInMinutes),
                   signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}



