using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.response
{
    public class RedirectResponse
    {
        [Required]
        [Url]
        public string originalUrl { get; set; } = string.Empty;
    }
}
