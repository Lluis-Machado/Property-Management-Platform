using FluentValidation;
using TaxManagement.Context;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagement.Validators;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IDeclarationRepository, DeclarationRepository>();
builder.Services.AddScoped<IDeclarantRepository, DeclarantRepository>();
builder.Services.AddScoped<IValidator<Declarant>, DeclarantValidator>();
builder.Services.AddScoped<IValidator<Declaration>, DeclarationValidator>();
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
