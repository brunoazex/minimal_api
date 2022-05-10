using System.Net;

namespace RestApi.Services
{
    public class AccountServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public AccountServiceException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public AccountServiceException(HttpStatusCode statusCode)
            : this("0", statusCode) { }

        public AccountServiceException(string message)
            : this(message, HttpStatusCode.UnprocessableEntity) { }

        public AccountServiceException()
            : this("0", HttpStatusCode.UnprocessableEntity) { }
    }
}
