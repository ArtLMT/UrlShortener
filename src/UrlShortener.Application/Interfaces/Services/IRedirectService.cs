using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces.Services
{
    public interface IRedirectService
    {
        IRedirectService GetDirectService();
        Task<RedirectResponse> RedirectToOriginalUrl(RedirectRequest request);
    }
}
