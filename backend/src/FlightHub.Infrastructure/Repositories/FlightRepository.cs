using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;
using FlightHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightHub.Infrastructure.Repositories;

// SRP (Single Responsibility Principle): This repository encapsulates data access for flights.
// DIP (Dependency Inversion Principle): 
// It implements the IFlightRepository abstraction defined in the Application layer.
public class FlightRepository(FlightDbContext context, ILogger<FlightRepository>? logger = null) : IFlightRepository
{
    public async Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Flights
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Flights
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        context.Flights.Add(flight);
        await context.SaveChangesAsync(cancellationToken);
        return flight;
    }

    public async Task<Flight?> UpdateAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        var existing = await context.Flights.FindAsync([flight.Id], cancellationToken);
        if (existing is null)
        {
            return null;
        }

        context.Entry(existing).CurrentValues.SetValues(flight);
        await context.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await context.Flights.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            // Log a warning when a delete is attempted for a non-existent flight.
            logger?.LogWarning(
            "Flight with id {Id} not found when attempting to delete.",
            id);
            return false;
        }

        context.Flights.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
