using DocumentAnalyzerAPI.Configurations;
using DocumentAnalyzerAPI.Contexts;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Mappers;
using DocumentAnalyzerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Contexts
builder.Services.AddSingleton<AzureFormRecognizerContext>();

// Services
builder.Services.AddScoped<IAzureFormRecognizer, AzureFormRecognizer>();
builder.Services.AddScoped<IDocumentAnalyzerService, DocumentAnalyzerService>();

// Mappers
builder.Services.AddScoped<IAPInvoiceDTOMapper, APInvoiceDTOMapper>();
builder.Services.AddScoped<IDocumentFieldsMapper, DocumentFieldsMapper<APInvoiceDTO>>();
builder.Services.AddScoped<IAPInvoiceLineDTOMapper, APInvoiceLineDTOMapper>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

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
