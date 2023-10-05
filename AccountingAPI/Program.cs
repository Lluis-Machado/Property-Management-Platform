using AccountingAPI.Configurations;
using AccountingAPI.Context;
using AccountingAPI.DTOs;
using AccountingAPI.Middlewares;
using AccountingAPI.Repositories;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Logs
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Middelwares
builder.Services.AddTransient<GlobalErrorHandlingMiddleware>();

// Contexts
builder.Services.AddSingleton<IDapperContext>(provider =>
{
    return new DapperContext(builder.Configuration.GetConnectionString("SqlConnection"));
});

// Repositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IPeriodRepository, PeriodRepository>();
builder.Services.AddScoped<IDepreciationRepository, DepreciationRepository>();
builder.Services.AddScoped<IFixedAssetRepository, FixedAssetRepository>();
builder.Services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
builder.Services.AddScoped<IAPInvoiceRepository, APInvoiceRepository>();
builder.Services.AddScoped<IARInvoiceRepository, ARInvoiceRepository>();
builder.Services.AddScoped<IAPInvoiceLineRepository, APInvoiceLinesRepository>();
builder.Services.AddScoped<IARInvoiceLineRepository, ARInvoiceLinesRepository>();
builder.Services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

// Service
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<IDepreciationService, DepreciationService>();
builder.Services.AddScoped<IFixedAssetService, FixedAssetService>();
builder.Services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
builder.Services.AddScoped<IAPInvoiceService, APInvoiceService>();
builder.Services.AddScoped<IARInvoiceService, ARInvoiceService>();
builder.Services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
builder.Services.AddScoped<ILoanService, LoanService>();

// Validators
builder.Services.AddScoped<IValidator<CreateTenantDTO>, CreateTenantDTOValidator>();
builder.Services.AddScoped<IValidator<CreateBusinessPartnerDTO>, CreateBusinessPartnerDTOValidator>();
builder.Services.AddScoped<IValidator<CreateAPInvoiceDTO>, CreateAPInvoiceDTOValidator>();
builder.Services.AddScoped<IValidator<CreateARInvoiceDTO>, CreateARInvoiceDTOValidator>();
builder.Services.AddScoped<IValidator<CreatePeriodDTO>, CreatePeriodDTOValidator>();
builder.Services.AddScoped<IValidator<CreateAPInvoiceLineDTO>, CreateAPInvoiceLineDTOValidator>();
builder.Services.AddScoped<IValidator<CreateARInvoiceLineDTO>, CreateARInvoiceLineDTOValidator>();
builder.Services.AddScoped<IValidator<CreateExpenseCategoryDTO>, CreateExpenseCategoryDTOValidator>();
builder.Services.AddScoped<IValidator<CreateLoanDTO>, CreateLoanDTOValidator>();
builder.Services.AddScoped<IValidator<CreateFixedAssetDTO>, CreateFixedAssetDTOValidator>();
builder.Services.AddScoped<IValidator<CreateDepreciationDTO>, CreateDepreciationDTOValidator>();
//
builder.Services.AddScoped<IValidator<UpdateTenantDTO>, UpdateTenantDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateBusinessPartnerDTO>, UpdateBusinessPartnerDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateAPInvoiceDTO>, UpdateAPInvoiceDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateARInvoiceDTO>, UpdateARInvoiceDTOValidator>();
builder.Services.AddScoped<IValidator<UpdatePeriodDTO>, UpdatePeriodDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateAPInvoiceLineDTO>, UpdateAPInvoiceLineDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateARInvoiceLineDTO>, UpdateARInvoiceLineDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateExpenseCategoryDTO>, UpdateExpenseCategoryDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateLoanDTO>, UpdateLoanDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateFixedAssetDTO>, UpdateFixedAssetDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateDepreciationDTO>, UpdateDepreciationDTOValidator>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Controllers
builder.Services.AddControllers();

// API Enpoints
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Accounting API", Version = "v1" });
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

// Authentication
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

// Middlewares
app.UseMiddleware<GlobalErrorHandlingMiddleware>();

// Validators
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

// Swagger
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
