using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Interfaces.Repositories;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Identity.Entities;
using UrlShortener.Infrastructure.Repositories;

namespace UrlShortener.Infrastructure.Services
{
    public class RefreshTokenServiceImpl : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repo;
        private readonly ILogger<RefreshTokenServiceImpl> _logger;

        public RefreshTokenServiceImpl(IRefreshTokenRepository repository, ILogger<RefreshTokenServiceImpl> logger)
        {
            _repo = repository;
            _logger = logger;
        }

        public async Task<string> GenerateAndStoreAsync(ApplicationUser user)
        {
            var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var existingToken = await _repo.GetActiveTokenByUserIdAsync(user.Id);

            if (existingToken != null)
            {
                // Bước 2a: Nếu CÓ -> Cập nhật token cũ
                existingToken.Token = tokenValue;
                existingToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
                existingToken.IsRevoked = false;
                existingToken.CreatedAt = DateTime.UtcNow; // Cập nhật cả thời gian tạo

                // (Nếu dùng EF Core, bạn không cần gọi _repo.UpdateAsync() 
                //  vì entity đã được theo dõi)
                _repo.Update(existingToken);
            }
            else
            {
                // Bước 2b: Nếu KHÔNG CÓ -> Tạo mới
                var refresh = new RefreshToken
                {
                    Token = tokenValue,
                    UserId = user.Id, // Chỉ cần set UserId (Foreign Key)
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    IsRevoked = false,
                    //User = user
                    // Bỏ: User = user // Không nên gán cả đối tượng User vào đây
                    // có thể gây lỗi tracking của EF Core. 
                    // Chỉ cần gán UserId.
                };
                _repo.Add(refresh);
            }

            await _repo.SaveChangesAsync();
            return tokenValue;
        }

        public async Task<bool> ValidateAsync(string refreshToken)
        {
            var token = await _repo.GetByTokenAsync(refreshToken);
            if (token == null) return false;

            return !token.IsRevoked && token.ExpiresAt > DateTime.UtcNow;
        }

        public async Task RevokeAsync(string refreshToken)
        {
            var token = await _repo.GetByTokenAsync(refreshToken);
            if (token == null) return;

            token.IsRevoked = true;
            _repo.Update(token);
            await _repo.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetByTokenAsync(string refreshToken)
        {
            var token = await _repo.GetByTokenAsync(refreshToken);

            if (token != null) return token;

            return null;
            //if (token.ExpiresAt > DateTime.UtcNow)
            //{
            //    token.ExpiresAt = DateTime.UtcNow;
            //    token.IsRevoked = true;
            //    await _repo.UpdateAsync(token);
            //}
        }
    }
}
