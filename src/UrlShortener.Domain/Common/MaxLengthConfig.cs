using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Common
{
    public static class MaxLengthConfig
    {
        public const int SHORT_CODE = 6;
        public const int ORIGINAL_URL = 2000;
        public const int USERNAME_MAXLENGTH = 200;
        public const int EMAIL_MAXLENGTH = 255;
        public const int PASSWORD_MAXLENGTH = 255;
        public const int PHONE_NUMBER_MAXLENGTH = 20;
    }
}
