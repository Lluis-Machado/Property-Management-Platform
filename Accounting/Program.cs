using Accounting.Context;
using Accounting.Middlewares;
using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Serilog;

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

builder.Services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
builder.Services.AddScoped<IValidator<BusinessPartner>, BusinessPartnerValidator>();

builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IValidator<Tenant>, AccountValidator>();

builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IValidator<Loan>, LoanValidator>();

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IValidator<Invoice>, InvoiceValidator>();

builder.Services.AddScoped<IInvoiceLineRepository, InvoiceLineRepository>();
builder.Services.AddScoped<IValidator<InvoiceLine>, InvoiceLineValidator>();

builder.Services.AddScoped<IExpenseTypeRepository, ExpenseTypeRepository>();
builder.Services.AddScoped<IValidator<ExpenseType>, ExpenseTypeValidator>();

builder.Services.AddScoped<IFixedAssetRepository, FixedAssetRepository>();
builder.Services.AddScoped<IValidator<FixedAsset>, FixedAssetValidator>();

builder.Services.AddScoped<IAmortizationRepository, AmortizationRepository>();
builder.Services.AddScoped<IValidator<Amortization>, AmortizationValidator>();

builder.Services.AddScoped<IAmortizationCongifRepository, AmortizationConfigRepository>();
builder.Services.AddScoped<IValidator<AmortizationConfig>, AmortizationConfigValidator>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
