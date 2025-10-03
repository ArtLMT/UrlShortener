using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Common.Enums;

namespace UrlShortener.Domain.Entities
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public int ClickCount { get; set; }
        public UrlStatus? Status { get; set; } = UrlStatus.ACTIVE;
        public User? User { get; set; }

        //public ShortUrl(int id, string originalUrl, string shortCode, int? userId, DateTime createdAt, DateTime? expiryDate, int clickCount, User? user)
        //{
        //    //if (string.IsNullOrWhiteSpace(originalUrl))
        //    //{
        //    //    throw new ArgumentException("Original URL is required.");
        //    //}
        //    //if (string.IsNullOrWhiteSpace(shortCode) || shortCode.Length < 6)
        //    //    throw new ArgumentException("Short code must be at least 6 characters.");


        //    Id = id;
        //    OriginalUrl = originalUrl;
        //    ShortCode = shortCode;
        //    UserId = userId;
        //    CreatedAt = createdAt;
        //    ExpiryDate = expiryDate;
        //    ClickCount = clickCount;
        //    User = user;
        //}
    }
}
