using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urls")]
    public class UrlController : BaseApiController
    {
        private readonly IShortUrlService _shortUrlService;

        public UrlController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<BaseResponse<ShortUrlResponse>>> GetByShortCode(string shortCode)
        {
            var foundUrlEntity = await _shortUrlService.GetOriginalUrl(shortCode);

            return Success<ShortUrlResponse>(foundUrlEntity, "Success");

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<ShortUrlResponse>>> CreateShortUrl(ShortUrlRequest request)
        {
            
            var shortResponse = await _shortUrlService.ShortenUrl(request, User);

            if (shortResponse == null)
                return Fail<ShortUrlResponse>("Fail to create", 400);

            return Success<ShortUrlResponse>(shortResponse, "Success");
 
        }

        [Authorize]
        [HttpGet("/Lists")] 
        public async Task<ActionResult<BaseResponse<List<ShortUrlResponse>>>> GetShortUrls() 
        {
            var shortUrls = await _shortUrlService.GetShortUrls(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (shortUrls == null)
                return Fail<List<ShortUrlResponse>>("Fail to get", 400);

            return Success<List<ShortUrlResponse>>(shortUrls, "Sucess");
        }


    }
}
