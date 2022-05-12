using Microsoft.AspNetCore.Diagnostics;
using RestApi.Services;
using Carter;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddSingleton<AccountService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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

app.MapCarter();
app.Run();