using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Infrastructure.Services;

namespace UrlShortener.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IIdentityService _identityService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        private const string RefreshTokenCookieName = "refreshToken";
        public AuthController(IIdentityService identityService, IRefreshTokenService refreshTokenService, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _identityService = identityService;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<ActionResult<BaseResponse<string>>> Register(RegisterRequestDTO request)
        {
            var result = await _identityService.RegisterAsync(request);
            return Success(result.Data, "User registered successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<TokenResponse>>> Login(LoginRequestDTO request)
        {
            _logger.LogInformation("Login start");
            var result = await _identityService.LoginAsync(request);
            _logger.LogInformation("Login after service call");

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                _logger.LogInformation("Setting cookie...");
                SetRefreshTokenCookie(result.RefreshToken);
                result.RefreshToken = string.Empty;
            }

            return Success(result, "Login successful");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync() // 1. Đổi kiểu trả về sang IActionResult
        {
            // 2. ĐỌC TOKEN TỪ COOKIE (thay vì DTO)
            if (!HttpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken)
                || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token is missing." });
            }

            // --- Phần logic nghiệp vụ giữ nguyên ---
            var isValid = await _refreshTokenService.ValidateAsync(refreshToken);
            if (!isValid)
            {
                // Nếu token không hợp lệ -> Xóa cookie ở phía client
                ClearRefreshTokenCookie();
                return Unauthorized(new { message = "Invalid refresh token." });
            }

            var storedToken = await _refreshTokenService.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.User == null)
            {
                ClearRefreshTokenCookie();
                return Unauthorized(new { message = "Invalid token details." });
            }

            var user = storedToken.User;

            // (Tùy chính sách — có thể revoke luôn token cũ)
            await _refreshTokenService.RevokeAsync(refreshToken);

            // --- Tạo token mới (giống hệt code cũ) ---
            var accessTokenResponse = await _tokenService.GenerateToken(user); // Cái này chứa AccessToken
            var newRefreshToken = await _refreshTokenService.GenerateAndStoreAsync(user); // Cái này là chuỗi RefreshToken mới

            // 3. GHI REFRESH TOKEN MỚI VÀO COOKIE (thay vì trả về body)
            SetRefreshTokenCookie(newRefreshToken);

            // 4. Chỉ trả về AccessToken trong body
            return Ok(new
            {
                accessToken = accessTokenResponse.AccessToken
            });
        }
        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Quan trọng: Ngăn JavaScript truy cập
                Expires = DateTime.UtcNow.AddDays(7), // Nên khớp với hạn dùng của refresh token
                Secure = true,   // BẮT BUỘC: Chỉ gửi qua HTTPS
                SameSite = SameSiteMode.None, // Tốt nhất: Chống tấn công CSRF
                Path = "/api/v1/Auth" // Tùy chọn: Giới hạn cookie chỉ cho các endpoint xác thực
            };
            HttpContext.Response.Cookies.Append(RefreshTokenCookieName, token, cookieOptions);
        }

        private void ClearRefreshTokenCookie()
        {
            // Xóa cookie bằng cách ghi đè nó với thời gian hết hạn trong quá khứ
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(-1),
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/v1/Auth"
            };
            HttpContext.Response.Cookies.Append(RefreshTokenCookieName, string.Empty, cookieOptions);
        }
    }
}
