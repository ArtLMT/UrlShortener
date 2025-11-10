using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Common.Enums;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public UserStatus? Status { get; set; } = UserStatus.ACTIVE;
        public ICollection<ShortUrl> ShortUrls { get; set; } = new List<ShortUrl>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
