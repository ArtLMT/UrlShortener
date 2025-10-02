using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urls")]
    public class UrlController : ControllerBase
    {
        [HttpGet]
        public string GetUrl()
        {
            return "test";
        }
    }
}
