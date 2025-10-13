using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.DTOs.response
{
    public class ShortUrlResponse
    {
        [Required]
        public ShortUrl shortUrl { get; set; }

        [Required]
        [Url]
        public string originalUrl { get; set; } = string.Empty;

        [Required]
        public int id { get; set; }
    }
}
