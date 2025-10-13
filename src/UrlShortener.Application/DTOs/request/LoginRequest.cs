using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.request
{
    public class LoginRequest
    {
        /// <example>123@gmail.com</example>
        public string Email { get; set; }

        /// <example>Thanh1@</example>
        public string Password { get; set; }
    }
}
