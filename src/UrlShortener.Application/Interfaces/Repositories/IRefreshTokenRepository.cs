using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        void Add(RefreshToken token);
        void Update(RefreshToken token);
        void Delete(RefreshToken token);
        Task SaveChangesAsync();
        Task RevokeAllForUserAsync(string userId);
        Task<RefreshToken?> GetActiveTokenByUserIdAsync(string userId);
    }
}
