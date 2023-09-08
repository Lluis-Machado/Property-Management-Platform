using DocumentsAPI.Services;
using DocumentsAPI.Middlewares;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using DocumentsAPI.Services.AzureBlobStorage;
using DocumentsAPI.Validators;
using DocumentsAPI.Contexts;
using DocumentsAPI.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using MassTransit;
using DocumentsAPI.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Serilog
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Global error handling
builder.Services.AddTransient<AzureErrorHandlingMiddleware>();
builder.Services.AddTransient<GlobalErrorHandlingMiddleware>();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<AzureBlobStorageContext>();
builder.Services.AddScoped<IBlobMetadataRepository, BlobMetadataRepository>();
builder.Services.AddScoped<IDocumentRepository, AzureBlobStorage>();
builder.Services.AddScoped<IDocumentsService, DocumentsService>();
builder.Services.AddScoped<IArchiveRepository, AzureBlobStorage>();
builder.Services.AddScoped<IArchivesService, ArchivesService>();
builder.Services.AddScoped<IFoldersService, FoldersService>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorageConnectionString:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureStorageConnectionString:queue"], preferMsi: true);
});

builder.Configuration.AddUserSecrets<Program>();

// Validators
builder.Services.AddScoped<IValidator<Archive>, ArchiveValidator>();

// Swagger
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Documents API", Version = "v1" });
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

builder.Services.AddMassTransit(config => {

    config.AddConsumer<ArchiveConsumer>();

    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host("amqp://guest:guest@localhost:5672");

        cfg.ReceiveEndpoint("archives", c => {
            c.ConfigureConsumer<ArchiveConsumer>(ctx);
        });
    });
});

// Other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<AzureErrorHandlingMiddleware>();


app.MapControllers();

app.Run();





