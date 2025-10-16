using Azure;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.Exceptions;
using UrlShortener.Application.Interfaces.Repositories;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Services;

namespace UrlShortener.Tests.Infrastructure
{
    public class ShortUrlServiceTests
    {
        private readonly Mock<IShortUrlRepository> _mockRepo;
        private readonly ShortUrlServiceImpl _service;
        private readonly ClaimsPrincipal _mockUser;

        public ShortUrlServiceTests()
        {
            _mockRepo = new Mock<IShortUrlRepository>();
            _mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "user123")
            }, "mock"));
            _service = new ShortUrlServiceImpl(_mockRepo.Object);
        }

        [Fact]
        public async Task ShortenUrl_NewUrl_ReturnsShortCode()
        {
            // Arrange
            var request = new ShortUrlRequest { OriginalUrl = "https://google.com" };
            _mockRepo.Setup(r => r.GetByOriginalUrlAsync(It.IsAny<string>())).ReturnsAsync((ShortUrl)null!);
            _mockRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync((ShortUrl)null!);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ShortUrl>())).ReturnsAsync((ShortUrl s) => s);

            // Act
            var response = await _service.ShortenUrl(request, _mockUser, "http//test/");

            // Assert
            Assert.NotNull(response);
            Assert.Equal("user123", response.UserId);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<ShortUrl>()), Times.Once);
        }

        [Fact]
        public async Task GetOriginalUrl_Found_ReturnsResponse()
        {
            // Arrange
            var shortUrlEntity = new ShortUrl
            {
                Id = 1,
                ShortCode = "abc123",
                OriginalUrl = "https://example.com",
                UserId = "user123"
            };

            _mockRepo.Setup(r => r.GetByCodeAsync("abc123"))
                     .ReturnsAsync(shortUrlEntity);

            // Act
            var response = await _service.GetOriginalUrl("abc123", "http//test/");

            // Assert
            Assert.NotNull(response);
            Assert.Equal("abc123", response.ShortCode);
            Assert.Equal("https://example.com", response.OriginalUrl);
            Assert.Equal("user123", response.UserId);
            _mockRepo.Verify(r => r.GetByCodeAsync("abc123"), Times.Once);
        }

        [Fact]
        public async Task GetOriginalUrl_NotFound_ThrowsException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByCodeAsync("abc123"))
                     .ReturnsAsync((ShortUrl)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetOriginalUrl("abc123", "http//test/"));

            _mockRepo.Verify(r => r.GetByCodeAsync("abc123"), Times.Once);
        }

        [Fact]
        public async Task GetOriginalUrl_BadRequest_ThrowsException()
        {
            var invalidShortCode = "aaaaa"; // 5 ký tự, giả sử service yêu cầu 6

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetOriginalUrl(invalidShortCode, "http//test/")
            );
        }

        [Fact]
        public async Task ShortenUrl_DuplicateOriginalUrl_ReturnsExistingShortUrl()
        {
            // Arrange
            var existingShortUrl = new ShortUrl
            {
                ShortCode = "abc123",
                OriginalUrl = "https://google.com",
                UserId = "user123"
            };

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user123")
            }, "mock"));

            _mockRepo.Setup(r => r.GetByOriginalUrlAsync("https://google.com"))
                     .ReturnsAsync(existingShortUrl);

            var request = new ShortUrlRequest { OriginalUrl = "https://google.com" };

            // Act
            var response = await _service.ShortenUrl(request, mockUser, "http//test/");

            // Assert
            Assert.NotNull(response);
            Assert.Equal("abc123", response.ShortCode);
            Assert.Equal("https://google.com", response.OriginalUrl);
            Assert.Equal("user123", response.UserId);

            // Verify that no new record was created
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<ShortUrl>()), Times.Never);
        }

    }

}