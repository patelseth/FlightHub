using FlightHub.Application.Interfaces;
using FlightHub.Application.Services;
using FlightHub.Infrastructure.Data;
using FlightHub.Infrastructure.Repositories;
using FlightHub.Infrastructure.Seed;
using FlightHub.Api.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core InMemory DbContext
builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseInMemoryDatabase("FlightHub"));

// Application + Infrastructure services (DIP: these are wired at composition root)
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightService, FlightService>();

// Controllers + enums as strings in JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Swagger + Rate limiting via extensions
builder.Services.AddSwaggerDocumentation();
builder.Services.AddFlightHubRateLimiting();

var app = builder.Build();

// Seed CSV data into the in-memory database at startup.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FlightDbContext>();

    var csvPath = Path.Combine(
        app.Environment.ContentRootPath,
        "..",
        "FlightHub.Infrastructure",
        "Data",
        "FlightInformation.csv");

    csvPath = Path.GetFullPath(csvPath);

    await FlightDbSeeder.SeedFromCsvAsync(context, csvPath);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightHub API v1");
    });
}

app.UseHttpsRedirection();

// Global rate limiting (100 requests per minute per IP)
app.UseFlightHubRateLimiting();

app.MapControllers();

app.Run();

public partial class Program { }
