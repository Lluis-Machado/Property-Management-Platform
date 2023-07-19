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
builder.Services.AddScoped<IDocumentFieldsMapper, DocumentFieldsMapper>();
builder.Services.AddScoped<IAPInvoiceDTOMapper, APInvoiceDTOMapper>();
builder.Services.AddScoped<IARInvoiceDTOMapper, ARInvoiceDTOMapper>();
builder.Services.AddScoped<IAPInvoiceLineDTOMapper, APInvoiceLineDTOMapper>();
builder.Services.AddScoped<IARInvoiceLineDTOMapper, ARInvoiceLineDTOMapper>();

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
