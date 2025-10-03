using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Interfaces.Repositories;
using UrlShortener.Domain.Common.Enums;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UrlShortenerDbContext _context;
        public UserRepository(UrlShortenerDbContext context) => _context = context;

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.Status != UserStatus.BANNED);
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username && u.Status != UserStatus.BANNED);

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Status == UserStatus.ACTIVE)
                .ToListAsync();
        }

        public async Task BanUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Status = UserStatus.BANNED;
                await _context.SaveChangesAsync();
            }
        }
    }
}
