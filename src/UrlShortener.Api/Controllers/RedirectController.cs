using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Services;

namespace UrlShortener.Api.Controllers
{
    [Route("")]

    public class RedirectController : ControllerBase
    {
        private readonly IShortUrlService _urlService;

        public RedirectController(IShortUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            var result = await _urlService.GetOriginalUrl(code);
            if (result == null)
                return NotFound("Failed to fetch original url from service");

            var originalUrl = result.OriginalUrl;
            if (string.IsNullOrEmpty(originalUrl))
                return NotFound("Original url not found");

            return Redirect(originalUrl);
        }
    }
}
