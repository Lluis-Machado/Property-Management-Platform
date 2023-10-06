using CoreAPI.Middlewares;
using CoreAPI.Services;
using MassTransit;
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

// Add services to the container.

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<ICoreService, CoreService>();
builder.Services.AddTransient<IPropertyServiceClient, PropertyServiceClient>();
builder.Services.AddTransient<ICompanyServiceClient, CompanyServiceClient>();
builder.Services.AddTransient<IDocumentsServiceClient, DocumentsServiceClient>();
builder.Services.AddTransient<IContactServiceClient, ContactServiceClient>();
builder.Services.AddTransient<IOwnershipServiceClient, OwnershipServiceClient>();

builder.Services.AddTransient<IBaseClientService, BaseClientService>();

builder.Services.AddSingleton<GlobalErrorHandlingMiddleware>();

//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers();

builder.Services.AddMvc(options =>
{
    options.InputFormatters.Insert(0, new RawJsonBodyInputFormatter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreAPI", Version = "v1" });
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

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("amqp://guest:guest@localhost:5672");

        // Configure retry policy
        cfg.UseMessageRetry(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
});

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();