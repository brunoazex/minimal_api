using System.Net;

namespace RestApi.Services
{
    public class ServiceResult
    {
        public HttpStatusCode StatusCode { get; }
        public object? Data { get; }

        public ServiceResult(HttpStatusCode statusCode, object? data)
            => (StatusCode, Data) = (statusCode, data);

        public static ServiceResult Success(HttpStatusCode statusCode = HttpStatusCode.OK, object? data = null)
            => new(statusCode, data);

        public static ServiceResult Error(HttpStatusCode statusCode = HttpStatusCode.InternalServerError, object? data = null)
            => new(statusCode, data);
    }
}
