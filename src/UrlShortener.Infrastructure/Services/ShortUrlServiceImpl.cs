using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        //private string baseUrl;

        public ShortUrlServiceImpl(IShortUrlRepository repo)
        {
            _repo = repo;
        }

        public ShortUrlServiceImpl(IShortUrlRepository repo,
                                    UserManager<ApplicationUser> user)
        {
            _repo = repo; 
            _userManager = user;
        }

      

        public async Task<ShortUrlResponse> ShortenUrl(ShortUrlRequest request, ClaimsPrincipal user, string baseURl)
        {

            var shortCode = Guid.NewGuid().ToString().Substring(0, 6);

            ShortUrl? existed = await _repo.GetByCodeAsync(shortCode);
            while (existed != null)
            {
                shortCode = Guid.NewGuid().ToString().Substring(0, 6);
                existed = await _repo.GetByCodeAsync(shortCode);
            }

            var RequestOriginalUrl = request.OriginalUrl;

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
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                UserId = id

            };





            var ShortUrlEntity = await _repo.AddAsync(shortUrl);

            return CreateResponse(ShortUrlEntity, baseURl);
        }

        public ShortUrlResponse CreateResponse(ShortUrl ShortUrlEntity, string baseUrl = "")
        {
            var response = new ShortUrlResponse
            {
                Id = ShortUrlEntity.Id,
                ShortCode = ShortUrlEntity.ShortCode,
                OriginalUrl = ShortUrlEntity.OriginalUrl,
                UserId = ShortUrlEntity.UserId,
                FullUrl = baseUrl + "/" + ShortUrlEntity.ShortCode
            };
            return response;
        }

        public async Task<ShortUrlResponse> GetOriginalUrl(string shortCode, string baseURl)
        {
            if (string.IsNullOrWhiteSpace(shortCode) || shortCode.Length != 6)
            {
                throw new BadRequestException("Short code is invalid");
            }
            var FoundShortUrl = await _repo.GetByCodeAsync(shortCode);
            if (FoundShortUrl == null)
            {
                throw new NotFoundException("Short URL not found");
            }

            return CreateResponse(FoundShortUrl, baseURl);
        }

        public async Task<List<ShortUrlResponse>> GetShortUrls(string userId, string baseURl)
        {
            var shortUrls = await _repo.GetByUserIdAsync(userId);
            if (shortUrls == null || !shortUrls.Any())
            {
                throw new NotFoundException("No short URLs found for the user");
            }
            var responseList = shortUrls.Select(su => CreateResponse(su, baseURl)
                ).ToList();

            return responseList;

        }

        public async Task<List<ShortUrlResponse>> GetShortUrlsV2()
        {
            var shortUrls = await _repo.GetShortUrlsAsync();
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
