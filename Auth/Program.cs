using Authentication.Middlewares;
using Authentication.Models;
using Authentication.Services.Auth0;
using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Global error handling
builder.Services.AddTransient<GlobalErrorHandlingMiddleware>();
builder.Services.AddTransient<ApiErrorHandlingMiddleware>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Auth0 configuration and services
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0"));
builder.Services.AddHttpClient<IPublicTokenAPI, PublicTokenAPI>();
builder.Services.AddHttpClient<IUsersAPI, UsersAPI>();
builder.Services.AddHttpClient<IRolesAPI, RolesAPI>();
builder.Services.AddSingleton(provider =>
{
    var auth0Settings = provider.GetRequiredService<IOptions<Auth0Settings>>().Value;
    return auth0Settings;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<ApiErrorHandlingMiddleware>();

app.MapControllers();

app.Run();