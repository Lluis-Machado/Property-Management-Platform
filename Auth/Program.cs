using Auth.Models;
using Auth.Services.Auth0;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Auth0 configuration and services
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0"));
builder.Services.AddHttpClient<PublicTokenAPI>();
builder.Services.AddHttpClient<UsersAPI>();
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

app.MapControllers();

app.Run();