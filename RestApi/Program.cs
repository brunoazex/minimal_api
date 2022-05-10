using Microsoft.AspNetCore.Diagnostics;
using RestApi.Services;
using RestApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AccountService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is AccountServiceException opException)
        {
            context.Response.StatusCode = (int)opException.StatusCode;
            await context.Response.WriteAsync(opException.Message);
        }

        await context.Response.CompleteAsync();
    });
});

app.MapGet("/balance", ([FromQuery(Name = "account_id")] string accountId, AccountService service) => Results.Ok(service.GetBalance(accountId)));
app.MapPost("/event", (NewEvent request, AccountService service) => Results.Created("balance", service.MakeOperation(request)));
app.MapPost("/reset", (AccountService service) =>
{
    service.Reset();
    return Results.Content("OK");
});

app.Run();