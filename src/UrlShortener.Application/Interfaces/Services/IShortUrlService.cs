using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;

namespace UrlShortener.Application.Interfaces.Services
{

    public interface IShortUrlService
    {
        Task<ShortUrlResponse> ShortenUrl(ShortUrlRequest request, ClaimsPrincipal user);
        Task<ShortUrlResponse> GetOriginalUrl(string shortCode);

        Task<List<ShortUrlResponse>> GetShortUrls(string userId);

        Task<List<ShortUrlResponse>> GetShortUrlsV2();


    }
}
