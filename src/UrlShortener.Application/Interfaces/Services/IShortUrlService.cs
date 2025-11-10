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
        Task<ShortUrlResponse> ShortenUrl(ShortUrlRequest request, ClaimsPrincipal user, string baseURL);
        Task<ShortUrlResponse> GetOriginalUrl(string shortCode, string baseURl = "");

        Task<List<ShortUrlResponse>> GetShortUrls(string userId, string baseURl);

        Task<List<ShortUrlResponse>> GetShortUrlsV2(string baseUrl);


    }
}