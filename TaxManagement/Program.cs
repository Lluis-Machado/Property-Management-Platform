using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using TaxManagement.Context;
using TaxManagement.Middelwares;
using TaxManagement.Repositories;
using TaxManagement.Validators;
using TaxManagementAPI.DTOs;
using TaxManagementAPI.Services;

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

// Add services to the container.


builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IDeclarationRepository, DeclarationRepository>();
builder.Services.AddScoped<IDeclarantRepository, DeclarantRepository>();

builder.Services.AddScoped<IDeclarantService, DeclarantService>();
builder.Services.AddScoped<IDeclarationService, DeclarationService>();

builder.Services.AddScoped<IValidator<DeclarantDTO>, DeclarantValidator>();
builder.Services.AddScoped<IValidator<CreateDeclarantDTO>, CreateDeclarantValidator>();
builder.Services.AddScoped<IValidator<UpdateDeclarantDTO>, UpdateDeclarantValidator>();
builder.Services.AddScoped<IValidator<DeclarationDTO>, DeclarationValidator>();
builder.Services.AddScoped<IValidator<CreateDeclarationDTO>, CreateDeclarationValidator>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.EnableAnnotations();
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

// Configure the HTTP request pipeline.
#if DEVELOPMENT || STAGE
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}
#endif

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
