using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Exceptions;
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
            // Note: Thieu exepction =))

            // Nay test thôi, chưa có validation với logic shortCode từ chatGPT
            var shortCode = Guid.NewGuid().ToString().Substring(0, 6);

            ShortUrl existed = await _repo.GetByCodeAsync(shortCode);
            while (existed != null) // Có cần validate thêm duplicate short code kh nhỉ
            {
                shortCode = Guid.NewGuid().ToString().Substring(0, 6);
                existed = await _repo.GetByCodeAsync(shortCode);
            }

            // Note: cần validate xem có trùng originalUrl kh
            var RequestOriginalUrl = request.originalUrl;

            var originalUrlAlreadyHadShort = await _repo.GetByOriginalUrlAsync(RequestOriginalUrl);
            if (originalUrlAlreadyHadShort != null)
            {
                throw new DuplicateShortCodeException(originalUrlAlreadyHadShort.ShortCode); // Note: này ví dụ thôi
            }

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
                shortUrl = shortUrl, // note: cần trả về ShortUrl Enitity
                originalUrl = ShortUrlEntity.OriginalUrl
            };


            return response;
        }

        // Note: đang không biết nên dùng hay cần thiết dùng Task.FromResult hay kh
        public Task<ShortUrlResponse> createResponse(ShortUrl ShortUrlEntity)
        {
            var response = new ShortUrlResponse
            {
                id = ShortUrlEntity.Id,
                shortUrl = ShortUrlEntity, // Note: cần trả về shortUrl dạng URL hoàn chỉnh hay shortCode thoi
                originalUrl = ShortUrlEntity.OriginalUrl // Note: Cai nay co can tra ve kh, luu vo db la xong r ma nhi..
            };
            return Task.FromResult(response);
        }

        //public ShortUrl createEnity(int id, string originalUrl, string shortcode, int userId, User user)
        //{
        //    return new ShortUrl
        //    {
        //        Id = id,
        //        OriginalUrl = originalUrl,
        //        ShortCode = shortcode,
        //        UserId = userId,
        //        CreatedAt = DateTime.UtcNow,
        //        User = user,
        //        ClickCount = 0
        //    };
        //}
    }
}
