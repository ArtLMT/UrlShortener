using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Interfaces.Repositories;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UrlShortenerDbContext _context;

        public RefreshTokenRepository(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public void Add(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
        }

        // SỬA LẠI: Không SaveChanges, và không cần async
        public void Update(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
        }

        // SỬA LẠI: Không SaveChanges, và không cần async
        public void Delete(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
        }

        // HÀM MỚI: Tìm token đang active của user
        // (Đây là logic bạn cần để sửa lỗi "duplicate key")
        public async Task<RefreshToken?> GetActiveTokenByUserIdAsync(string userId)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsRevoked);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
            => await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(t => t.Token == token);

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId)
            => await _context.RefreshTokens.Where(t => t.UserId == userId).ToListAsync();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
        public async Task RevokeAllForUserAsync(string userId)
        {
            var tokens = await _context.RefreshTokens.Where(r => r.UserId == userId && !r.IsRevoked).ToListAsync();
            tokens.ForEach(r => r.IsRevoked = true);
            _context.RefreshTokens.UpdateRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}
