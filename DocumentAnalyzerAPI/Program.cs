using DocumentAnalyzerAPI.Contexts;
using DocumentAnalyzerAPI.Mappers;
using DocumentAnalyzerAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Serilog
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
  .MinimumLevel.Override("RabbitMQ.Client", LogEventLevel.Warning)
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Contexts
builder.Services.AddSingleton<AzureFormRecognizerContext>();

// Services
builder.Services.AddScoped<IAzureFormRecognizer, AzureFormRecognizer>();
builder.Services.AddScoped<IDocumentAnalyzerService, DocumentAnalyzerService>();

// Mappers
builder.Services.AddScoped<IDocumentFieldsMapper, DocumentFieldsMapper>();
builder.Services.AddScoped<IAPInvoiceDTOMapper, APInvoiceDTOMapper>();
builder.Services.AddScoped<IARInvoiceDTOMapper, ARInvoiceDTOMapper>();
builder.Services.AddScoped<IAPInvoiceLineDTOMapper, APInvoiceLineDTOMapper>();
builder.Services.AddScoped<IARInvoiceLineDTOMapper, ARInvoiceLineDTOMapper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "DocAnalyzerAPI", Version = "v1" });
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
    opt.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Check if the controller belongs to the "Accounting" namespace
        if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            if (controllerActionDescriptor.ControllerTypeInfo.Namespace.StartsWith("Accounting"))
            {
                return false; // Exclude from Swagger
            }
        }
        return true; // Include in Swagger
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
