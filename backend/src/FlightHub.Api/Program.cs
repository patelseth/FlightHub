using FlightHub.Application.Interfaces;
using FlightHub.Application.Services;
using FlightHub.Infrastructure.Data;
using FlightHub.Infrastructure.Repositories;
using FlightHub.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// EF Core InMemory DbContext
builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseInMemoryDatabase("FlightHub"));

builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightService, FlightService>();

builder.Services.AddScoped<IFlightRepository, FlightRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FlightHub API",
        Version = "v1",
        Description = "Flight Information API (Coding Challenge)"
    });
});

var app = builder.Build();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { }
