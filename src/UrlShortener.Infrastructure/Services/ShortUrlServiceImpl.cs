using Microsoft.AspNetCore.Identity;
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
using UrlShortener.Infrastructure.Identity.Entities;
using UrlShortener.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;



namespace UrlShortener.Infrastructure.Services
{
    public class ShortUrlServiceImpl : IShortUrlService
    {
        private readonly IShortUrlRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;



        public ShortUrlServiceImpl(IShortUrlRepository repo,
                                    UserManager<ApplicationUser> user)
        {
            _repo = repo; 
            _userManager = user;
        }

        public async Task<ShortUrlResponse> ShortenUrl(ShortUrlRequest request, ClaimsPrincipal user)
        {

            var shortCode = Guid.NewGuid().ToString().Substring(0, 6);

            ShortUrl? existed = await _repo.GetByCodeAsync(shortCode);
            while (existed != null)
            {
                shortCode = Guid.NewGuid().ToString().Substring(0, 6);
                existed = await _repo.GetByCodeAsync(shortCode);
            }

            var RequestOriginalUrl = request.originalUrl;

            var originalUrlAlreadyHadShort = await _repo.GetByOriginalUrlAsync(RequestOriginalUrl);
            if (originalUrlAlreadyHadShort != null)
            {
                throw new DuplicateShortCodeException(originalUrlAlreadyHadShort.ShortCode);
            }

            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
            {
                throw new UnauthorizedException("User not found");
            }

            var shortUrl = new ShortUrl
            {
                OriginalUrl = request.originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0,
                UserId = id

            };

            var ShortUrlEntity = await _repo.AddAsync(shortUrl);

            return CreateResponse(ShortUrlEntity);
        }

        public ShortUrlResponse CreateResponse(ShortUrl ShortUrlEntity)
        {
            var response = new ShortUrlResponse
            {
                Id = ShortUrlEntity.Id,
                ShortCode = ShortUrlEntity.ShortCode,
                OriginalUrl = ShortUrlEntity.OriginalUrl,
                UserId = ShortUrlEntity.UserId
            };
            return response;
        }

        public async Task<ShortUrlResponse> GetOriginalUrl(string shortCode)
        {
            var FoundShortUrl = await _repo.GetByCodeAsync(shortCode);
            if (FoundShortUrl == null)
            {
                throw new NotFoundException("Short URL not found");
            }

            return CreateResponse(FoundShortUrl);
        }

        public async Task<List<ShortUrlResponse>> GetShortUrls(string userId)
        {
            var shortUrls = await _repo.GetByUserIdAsync(userId);
            if (shortUrls == null || !shortUrls.Any())
            {
                throw new NotFoundException("No short URLs found for the user");
            }
            var responseList = shortUrls.Select(su => CreateResponse(su)
                ).ToList();

            return responseList;

        }
    }
}
