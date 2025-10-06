using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Repositories;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Repositories;


namespace UrlShortener.Infrastructure.Services
{
    public class ShortUrlServiceImpl : IShortUrlService
    {

        private readonly IShortUrlRepository _repo;

        public ShortUrlServiceImpl(IShortUrlRepository repo)
        {
            _repo = repo; // tiem chich injection
        }

        public async Task<ShortUrlResponse> ShortenUrl(ShortUrlRequest request)
        {
            // Nay test thôi, chưa có validation với logic shortCode từ chatGPT
            var shortCode = Guid.NewGuid().ToString().Substring(0, 6);
            var shortUrl = new ShortUrl
            {
                OriginalUrl = request.originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            var ShortUrlEntity = await _repo.AddAsync(shortUrl);

            var response = new ShortUrlResponse
            {
                id = ShortUrlEntity.Id,
                shortCode = ShortUrlEntity.ShortCode, // cần trả về shortUrl dạng URL hoàn chỉnh
                originalUrl = ShortUrlEntity.OriginalUrl
            };


            return response;
        }

        public Task<ShortUrlResponse> createResponse(ShortUrl ShortUrlEntity)
        {
            var response = new ShortUrlResponse
            {
                id = ShortUrlEntity.Id,
                shortCode = ShortUrlEntity.ShortCode, // Note: cần trả về shortUrl dạng URL hoàn chỉnh hay shortCode thoi
                originalUrl = ShortUrlEntity.OriginalUrl // Note: Cai nay co can tra ve kh, luu vo db la xong r ma nhi..
            };
            return Task.FromResult(response);
        }
    }
}
