using AccountingAPI.Context;
using AccountingAPI.Middlewares;
using AccountingAPI.DTOs;
using AccountingAPI.Repositories;
using AccountingAPI.Validators;
using AccountingAPI.Configurations;
using AccountingAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;

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
builder.Services.AddSingleton<IDapperContext>(provider =>
{
    return new DapperContext(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IValidator<CreateTenantDTO>, CreateTenantDTOValidator>();

builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<IPeriodRepository, PeriodRepository>();

builder.Services.AddScoped<IDepreciationService, DepreciationService>();
builder.Services.AddScoped<IDepreciationRepository, DepreciationRepository>();

builder.Services.AddScoped<IFixedAssetService, FixedAssetService>();
builder.Services.AddScoped<IFixedAssetRepository, FixedAssetRepository>();

builder.Services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
builder.Services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
builder.Services.AddScoped<IValidator<CreateBusinessPartnerDTO>, CreateBusinessPartnerDTOValidator>();

builder.Services.AddScoped<IAPInvoiceService, APInvoiceService>();
builder.Services.AddScoped<IAPInvoiceRepository, APInvoiceRepository>();
builder.Services.AddScoped<IValidator<CreateAPInvoiceDTO>, CreateAPInvoiceDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateAPInvoiceDTO>, UpdateAPInvoiceDTOValidator>();

builder.Services.AddScoped<IARInvoiceService, ARInvoiceService>();
builder.Services.AddScoped<IARInvoiceRepository, ARInvoiceRepository>();
builder.Services.AddScoped<IValidator<CreateARInvoiceDTO>, CreateARInvoiceDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateARInvoiceDTO>, UpdateARInvoiceDTOValidator>();

builder.Services.AddScoped<IAPInvoiceLineRepository, APInvoiceLineRepository>();
builder.Services.AddScoped<IAPInvoiceLineService, APInvoiceLineService>();
builder.Services.AddScoped<IValidator<CreateAPInvoiceLineDTO>, CreateAPInvoiceLineDTOValidator>();

builder.Services.AddScoped<IARInvoiceLineRepository, ARInvoiceLineRepository>();
builder.Services.AddScoped<IARInvoiceLineService, ARInvoiceLineService>();
builder.Services.AddScoped<IValidator<CreateARInvoiceLineDTO>, CreateARInvoiceLineDTOValidator>();

builder.Services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
builder.Services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
builder.Services.AddScoped<IValidator<CreateExpenseCategoryDTO>, CreateExpenseCategoryDTOValidator>();

builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IValidator<CreateLoanDTO>, CreateLoanDTOValidator>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddControllers();

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

var app = builder.Build();

// Global FluentValidation CascadeMode
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
});
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
