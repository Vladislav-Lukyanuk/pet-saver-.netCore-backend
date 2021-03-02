using animalFinder.Enum;
using System.Net;

namespace animalFinder.Exception
{
    public class ApiException : System.Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Error { get; set; }
        public ApiException(HttpStatusCode statusCode, ApiError error) : base(error.ToString())
        {
            StatusCode = statusCode;
            Error = error.ToString();
        }

        public ApiException(HttpStatusCode statusCode, string error) : base(error)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public int GetStatusCode()
        {
            return (int)StatusCode;
        }

        public string GetErrorMessage()
        {
            return Error;
        }
    }
}
