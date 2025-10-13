using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urls")]
    public class UrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public UrlController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        [HttpGet]
        [Authorize]
        public string GetUrl()
        {
            return "test";
        }

        [HttpPost]
        public ActionResult<BaseResponse<ShortUrlResponse>> CreateShortUrl(string shortUrl)
        {
            var shortResponse = _shortUrlService.ToString();

            if (shortResponse == null)
            {
                Ok(new BaseResponse<ShortUrlResponse>(200, "Success"));
            }

            return Ok(new BaseResponse<ShortUrlResponse>(200, "Success", null));
        }


    }
}
