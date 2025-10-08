using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs.response;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult<BaseResponse<T>> Success<T>(
            T data,
            string message = "Success",
            int statusCode = 200)
        {
            var response = new BaseResponse<T>(statusCode, message, data);
            return StatusCode(statusCode, response);
        }

        protected ActionResult<BaseResponse<T>> Fail<T>(
            string message,
            int statusCode = 400)
        {
            var response = new BaseResponse<T>(statusCode, message);
            return StatusCode(statusCode, response);
        }
    }
}
