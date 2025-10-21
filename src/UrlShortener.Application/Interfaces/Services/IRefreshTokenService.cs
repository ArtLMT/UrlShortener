using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Identity.Entities;

namespace UrlShortener.Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateAndStoreAsync(ApplicationUser user);
        Task<bool> ValidateAsync(string refreshToken);
        Task RevokeAsync(string refreshToken);
        Task<RefreshToken> GetByTokenAsync(string refreshToken);

    }
}
