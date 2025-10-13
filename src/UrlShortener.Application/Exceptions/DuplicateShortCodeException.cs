using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.Exceptions
{
    public class DuplicateShortCodeException : Exception
    {
        public DuplicateShortCodeException(string shortCode)
            : base($"Short code '{shortCode}' already exists.")
        {
        }
    }
}
