using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.request;

namespace UrlShortener.Application.Validators
{
    public class ShortUrlRequestValidator : AbstractValidator<ShortUrlRequest>
    {
        public ShortUrlRequestValidator()
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("Original URL is required.");
                //.Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("Invalid URL format.");
        }
    }
}
