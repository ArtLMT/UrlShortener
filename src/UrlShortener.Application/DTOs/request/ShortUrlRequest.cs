using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.request
{
    public class ShortUrlRequest
    {
        [Required]
        [Url]
        public string originalUrl { get; set; } = string.Empty;

        //public int userId { get; set; }
    }
}
