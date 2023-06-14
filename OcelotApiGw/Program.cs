using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Get Auth0 conf values
string authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
string audience = builder.Configuration["Auth0:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = authority;
    options.Audience = audience;
});

// For the caching implementation
builder.Services.AddOcelot().AddCacheManager(x =>
{
    x.WithDictionaryHandle();
});

// Add the CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
    File.Copy("ocelot.Development.json", "ocelot.json", true);

app.UseOcelot().Wait();

app.UseAuthentication();

app.Run();