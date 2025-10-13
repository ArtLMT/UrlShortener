using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Exceptions;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Infrastructure.Identity.Entities;

namespace UrlShortener.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public IdentityService(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException("User not found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid credentials");

            return new LoginResponse
            {
                token = GenerateJwtToken(user)
            };
        }

        public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // 1. Lấy tất cả các mô tả lỗi từ result.Errors
                var errorMessages = result.Errors.Select(e => e.Description);

                // 2. Nối chúng lại thành một chuỗi duy nhất
                var fullErrorMessage = string.Join(" | ", errorMessages);

                throw new RegisterException($"Register failed! Reasons: {fullErrorMessage}");
            }


            return new LoginResponse
            {
                token = GenerateJwtToken(user)
            };
        }

        private TokenPayLoad GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new TokenPayLoad
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo,
                User = new UserInfoResponseDto
                {
                    Email = user.Email,
                    Id = user.Id
                }
            };
        }
    }

}
