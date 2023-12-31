using Authentication.Middlewares;
using Authentication.Models;
using Authentication.Services.Auth0;
using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Serilog
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
  .MinimumLevel.Override("RabbitMQ.Client", LogEventLevel.Warning)
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Global error handling
builder.Services.AddTransient<GlobalErrorHandlingMiddleware>();
builder.Services.AddTransient<ApiErrorHandlingMiddleware>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Auth0 configuration and services
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0"));
builder.Services.AddHttpClient<IPublicTokenAPI, PublicTokenAPI>();
builder.Services.AddHttpClient<IManagementTokenManager, ManagementTokenManager>();
builder.Services.AddHttpClient<IUsersAPI, UsersAPI>();
builder.Services.AddHttpClient<IRolesAPI, RolesAPI>();
builder.Services.AddTransient<ManagementTokenManager>();
builder.Services.AddSingleton(provider =>
{
    var auth0Settings = provider.GetRequiredService<IOptions<Auth0Settings>>().Value;
    return auth0Settings;
});

builder.Services.AddHttpContextAccessor();

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

#if DEVELOPMENT || STAGE
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
});
#endif

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<ApiErrorHandlingMiddleware>();

app.MapControllers();

app.Run();