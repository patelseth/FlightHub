using FlightHub.Domain.Entities;
using FlightHub.Domain.Enums;
using FlightHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightHub.Infrastructure.Seed;

// SRP (Single Responsibility Principle): Seed the database with initial flight data (from CSV) when empty.
public static class FlightDbSeeder
{
    public static async Task SeedFromCsvAsync(
        FlightDbContext context,
        string csvPath,
        CancellationToken cancellationToken = default)
    {
        if (await context.Flights.AnyAsync(cancellationToken))
        {
            return;
        }

        if (!File.Exists(csvPath))
        {
            return;
        }

        var lines = await File.ReadAllLinesAsync(csvPath, cancellationToken);
        if (lines.Length <= 1)
        {
            return;
        }

        // Skip header
        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = line.Split(',', StringSplitOptions.TrimEntries);

            if (columns.Length < 8)
            {
                continue;
            }

            var flight = new Flight
            {
                Id = int.Parse(columns[0]),
                FlightNumber = columns[1],
                Airline = columns[2],
                DepartureAirport = columns[3],
                ArrivalAirport = columns[4],
                DepartureTime = DateTime.Parse(columns[5]),
                ArrivalTime = DateTime.Parse(columns[6]),
                Status = Enum.Parse<FlightStatus>(columns[7])
            };

            context.Flights.Add(flight);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
