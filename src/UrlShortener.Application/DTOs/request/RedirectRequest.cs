using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.request
{
    public class RedirectRequest
    {
        public string shortCode { get; set; } = string.Empty;
    }
}
