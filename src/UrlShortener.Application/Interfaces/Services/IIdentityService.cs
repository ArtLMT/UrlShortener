using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;

namespace UrlShortener.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest req);
        Task<LoginResponse> LoginAsync(LoginRequest req);
    }
}
