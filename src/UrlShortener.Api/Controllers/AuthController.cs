using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Services;

namespace UrlShortener.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IIdentityService _identityService;
        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> Register(RegisterRequest request)
        {
            var result = await _identityService.RegisterAsync(request);
            return Success(result, "User registered successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> Login(LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request);
            return Success(result, "Login successful");
        }
    }
}
