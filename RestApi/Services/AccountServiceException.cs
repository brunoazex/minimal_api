using System.Net;

namespace RestApi.Services
{
    public class AccountServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public AccountServiceException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
