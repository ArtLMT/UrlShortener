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
using UrlShortener.Application.Interfaces.Repositories;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Identity.Entities;
using UrlShortener.Infrastructure.Repositories;
using UrlShortener.Infrastructure.Services;

namespace UrlShortener.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly ITokenService _tokenService;

        public IdentityService(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               IConfiguration config,
                               IRefreshTokenRepository refreshTokenRepository,
                               IRefreshTokenService refreshTokenService,
                               ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _refreshTokenRepo = refreshTokenRepository;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;

        }

        public async Task<TokenResponse> LoginAsync(LoginRequestDTO request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException("User not found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid credentials");

            TokenResponse tokenResponse = await _tokenService.GenerateToken(user);
            tokenResponse.RefreshToken = await _refreshTokenService.GenerateAndStoreAsync(user);
            return tokenResponse;
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterRequestDTO request)
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

            BaseResponse<string> response = new()
            {
                Status = 201,
                Message = "The account has been created!",
                Data = "You have registered successfully" 
            };

            return response;
            
        }


        Task IIdentityService.LogoutAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }

}       
//private TokenResponse GenerateToken(ApplicationUser user)
         //{
         //    var claims = new List<Claim>
         //{
         //    new Claim(ClaimTypes.NameIdentifier, user.Id),
         //    new Claim(ClaimTypes.Name, user.UserName ?? ""),
         //    new Claim(ClaimTypes.Email, user.Email ?? "")
         //};

//    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
//    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//    var token = new JwtSecurityToken(
//        issuer: _config["Jwt:Issuer"],
//        audience: _config["Jwt:Audience"],
//        claims: claims,
//        expires: DateTime.UtcNow.AddHours(3),
//        signingCredentials: creds
//    );

//    return new TokenResponse
//    {
//        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
//        RefreshToken = GenerateAndStoreRefreshToken
//    };
//}

//private async Task<string> GenerateAndStoreRefreshToken(ApplicationUser user)
//{
//    var randomBytes = new byte[64];
//    using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
//    rng.GetBytes(randomBytes);

//    var refreshToken = Convert.ToBase64String(randomBytes);

//    var token = new RefreshToken
//    {
//        Token = refreshToken,
//        UserId = user.Id,
//        CreatedAt = DateTime.UtcNow,
//        ExpiresAt = DateTime.UtcNow.AddDays(7),
//        IsRevoked = false
//    };

//    // ⚠️ Nếu bạn có DbContext riêng cho Identity thì lưu vào đó
//    // (giả sử bạn có _dbContext)
//    //_dbContext.RefreshTokens.Add(token);
//    //await _dbContext.SaveChangesAsync();

//    // Hoặc nếu chưa có DB, tạm thời bạn có thể lưu trong user claims
//    await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);

//    return refreshToken;
//}
