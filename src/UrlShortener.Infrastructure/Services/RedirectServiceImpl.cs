using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;
using UrlShortener.Application.DTOs.response;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Services
{
    public class RedirectServiceImpl : IRedirectService
    {
        public IRedirectService GetDirectService()
        {
            throw new NotImplementedException();
        }

        public Task<RedirectResponse> RedirectToOriginalUrl(RedirectRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
