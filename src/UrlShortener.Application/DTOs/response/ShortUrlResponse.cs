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
        public string? ShortCode { get; set; }

        [Url]
        public string? FullUrl {  get; set; }

        [Url]
        public string OriginalUrl { get; set; } = string.Empty;


        public int Id { get; set; }
    
        public string? UserId { get; set; } 
    }
}
