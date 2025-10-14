using FluentAssertions;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Repositories;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Services;
using Xunit;

namespace UrlShortener.Tests.Services
{
    public class ShortUrlServiceTests
    {
        private readonly Mock<IShortUrlRepository> _repoMock;
        private readonly IShortUrlService _service;

        public ShortUrlServiceTests()
        {
            _repoMock = new Mock<IShortUrlRepository>();
            _service = new ShortUrlServiceImpl(_repoMock.Object);
        }

        [Fact]
        public async Task ShortenUrl_ShouldGenerateUniqueShortCode_WhenNoExistingConflict()
        {
            // Arrange
            var request = new ShortUrlRequest
            {
                originalUrl = "https://openai.com/"
            };

            // Repository should return null (no conflict)
            _repoMock.Setup(r => r.GetByCodeAsync(It.IsAny<string>()))
                     .ReturnsAsync((ShortUrl?)null);

            _repoMock.Setup(r => r.GetByOriginalUrlAsync(request.originalUrl))
                     .ReturnsAsync((ShortUrl?)null);

            _repoMock.Setup(r => r.AddAsync(It.IsAny<ShortUrl>()))
                     .ReturnsAsync((ShortUrl shortUrl) =>
                     {
                         shortUrl.Id = 1; // simulate database auto-ID
                         return shortUrl;
                     });

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Name, "testuser@example.com")
            };
            var identity = new ClaimsIdentity(claims, "mock");
            var user = new ClaimsPrincipal(identity);

            // Act
            var result = await _service.ShortenUrl(request, user);

            // Assert
            result.Should().NotBeNull();
            result.ShortCode.Should().NotBeNullOrEmpty();
            result.ShortCode.Length.Should().Be(6);
            result.OriginalUrl.Should().Be(request.originalUrl);

            // Verify AddAsync was called once
            _repoMock.Verify(r => r.AddAsync(It.IsAny<ShortUrl>()), Times.Once);
        }
    }
}
