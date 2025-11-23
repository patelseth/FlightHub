using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;

namespace FlightHub.Application.Services;

// SRP (Single Responsibility Principle): This service handle flight-related operations at the application layer. 
// DIP (Dependency Inversion Principle): It depends on IFlightRepository, keeping it independent of any
// concrete data-access implementation.
public class FlightService(IFlightRepository flightRepository) : IFlightService
{
    // SRP (Single Responsibility Principle): Retrieve flights
    public Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Additional rules (filtering, sorting, and validation) can be added here later.
        return flightRepository.GetAllAsync(cancellationToken);
    }

    // SRP (Single Responsibility Principle): Retrieve a single flight by id 
    public Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Additional rules (such as not-found handling) can be added here later.
        return flightRepository.GetByIdAsync(id, cancellationToken);
    }

    // SRP (Single Responsibility Principle): Create a new flight
    public Task<Flight> CreateAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        // Additional rules (validation, defaults, etc.) can be added here later.
        return flightRepository.AddAsync(flight, cancellationToken);
    }
}
