using Carter;
using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using RestApi.Services;

namespace RestApi.Modules
{
    public class TransactionModule : ICarterModule
    {
        private readonly AccountService _accountService;

        public TransactionModule(AccountService accountService)
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
            return Results.Ok(_accountService.GetBalance(accountId));
        }

        public IResult HandleEvent(
            NewEvent request
        )
        {
            return Results.Created("balance", _accountService.MakeOperation(request));
        }

        public IResult HandleReset()
        {
            _accountService.Reset();
            return Results.Content("OK");
        }
    }
}
