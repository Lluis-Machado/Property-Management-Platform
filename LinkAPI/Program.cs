using FluentValidation;
using LinkAPI.Contexts;
using LinkAPI.Dtos;
using LinkAPI.Middelwares;
using LinkAPI.Repositories;
using LinkAPI.Services;
using LinkAPI.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Security.Claims;

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

// Validator
builder.Services.AddScoped<IValidator<LinkDto>, LinkValidator>();

// Add services to the container.
builder.Services.AddSingleton<IMongoContext, MongoContext>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();

builder.Services.AddScoped<ILinkService, LinkService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Link API", Version = "v1" });
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth0:BaseUrl"];
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

var app = builder.Build();

#if DEVELOPMENT || STAGE
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
});
#endif

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
