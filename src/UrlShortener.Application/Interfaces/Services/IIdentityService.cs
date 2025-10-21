using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;

namespace UrlShortener.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterRequestDTO req);
        Task<TokenResponse> LoginAsync(LoginRequestDTO req);
        Task LogoutAsync(string userId);
    }
}
