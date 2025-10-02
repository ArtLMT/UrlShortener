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
    public class ShortUrlRepository : IShortUrlRepository
    {
        private readonly UrlShortenerDbContext _context;

        public async Task<ShortUrl> AddAsync(ShortUrl shortUrl)
        {
            // Track entity and add to DbContext
            await _context.ShortUrls.AddAsync(shortUrl);
            await _context.SaveChangesAsync();

            return shortUrl; // return the created entity with Id populated
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ShortUrls.FindAsync(id);
            if (entity != null)
            {
                _context.ShortUrls.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ShortUrl?> GetByCodeAsync(string shortCode)
        {
            return await _context.ShortUrls
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        }

        public async Task<ShortUrl?> GetByIdAsync(int id)
        {
            return await _context.ShortUrls
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
