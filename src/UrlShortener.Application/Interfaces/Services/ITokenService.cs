using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Infrastructure.Identity.Entities;

namespace UrlShortener.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateToken(ApplicationUser user);
        
    }
}
