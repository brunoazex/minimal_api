using Carter;
using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using RestApi.Services;

namespace RestApi.Modules
{
    public class TransactionModule : ICarterModule
    {
        private readonly IAccountService _accountService;

        public TransactionModule(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/balance", HandleBalance);
            app.MapPost("/event", HandleEvent);
            app.MapPost("/reset", HandleReset);
        }


        public IResult HandleBalance(
            [FromQuery(Name = "account_id")]
            string accountId)
        {
            var result = _accountService.GetBalance(accountId);
            return Results.Json(result.Data, statusCode: (int)result.StatusCode);
        }

        public IResult HandleEvent(NewEvent request)
        {
            var result = _accountService.MakeOperation(request);
            return Results.Json(result.Data, statusCode: (int)result.StatusCode);
        }

        public IResult HandleReset()
        {
            var result = _accountService.Reset();
            return Results.StatusCode((int)result.StatusCode);
        }
    }
}
