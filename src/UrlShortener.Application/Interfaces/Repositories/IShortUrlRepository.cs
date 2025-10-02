using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces.Repositories
{
    public interface IShortUrlRepository
    {
        Task<ShortUrl> AddAsync(ShortUrl shortUrl);
        Task DeleteAsync(int id);
        Task<ShortUrl?> GetByCodeAsync(string shortCode);
        Task<ShortUrl?> GetByIdAsync(int id);
    }
}
