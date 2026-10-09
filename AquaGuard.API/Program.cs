using AquaGuard.API.Data;
using AquaGuard.API.Extensions;
using AquaGuard.API.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddFluentValidationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddHealthChecksConfigured();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<GlobalExceptionMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
