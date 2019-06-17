using System.Net;

namespace qu.nuget.infrastructure.Models
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public T Result { get; set; }

        public ApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            Result = default(T);
            ErrorMessage = string.Empty;
        }

        public ApiResponse(HttpStatusCode statusCode, T result = default(T), string errorMessage = "")
        {
            StatusCode = statusCode;
            Result = result;
            ErrorMessage = errorMessage;
        }
    }
}