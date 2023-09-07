using ContactsAPI.Contexts;
using ContactsAPI.DTOs;
using ContactsAPI.Repositories;
using ContactsAPI.Services;
using ContactsAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using ContactsAPI.Middlewares;
using MassTransit;

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

// Validator
builder.Services.AddScoped<IValidator<ContactDTO>, ContactValidator>();
builder.Services.AddScoped<IValidator<CreateContactDto>, CreateContactDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateContactDTO>, UpdateContactDTOValidator>();

// Add services to the container.
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<IContactsRepository, ContactsRepository>();
builder.Services.AddScoped<IContactsService, ContactsService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
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

// Configure the HTTP request pipeline.
//if (app.Environment.IsProduction() == false)
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

#if DEVELOPMENT == false
app.UseHttpsRedirection();
#endif

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
