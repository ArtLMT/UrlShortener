using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.response
{
    public class BaseResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T? data { get; set; }

        public BaseResponse(int status, string message)
        {
            Status = status;
            Message = message;
        }

        public BaseResponse(int status, string message, T data) : this(status, message)
        {
            this.data = data;
        }
    }
}
