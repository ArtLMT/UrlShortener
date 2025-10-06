using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.DTOs.request;

namespace UrlShortener.Application.Interfaces.Services
{
    public interface IShortUrlService
    { 
        Task<ShortUrlResponse> ShortenUrl(ShortUrlRequest request);

    }
}
