using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;

namespace FlightHub.Application.Services;

// SRP (Single Responsibility Principle): This service handle flight-related operations at the application layer. 
// DIP (Dependency Inversion Principle): It depends on IFlightRepository, keeping it independent of any
// concrete data-access implementation.
public class FlightService(IFlightRepository flightRepository) : IFlightService
{
    // SRP (Single Responsibility Principle): Retrieve flights.
    public Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Additional rules (filtering, sorting, and validation) can be added here later.
        return flightRepository.GetAllAsync(cancellationToken);
    }

    // SRP (Single Responsibility Principle): Retrieve a single flight by id.
    public Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Additional rules (such as not-found handling) can be added here later.
        return flightRepository.GetByIdAsync(id, cancellationToken);
    }

    // SRP (Single Responsibility Principle): Create a new flight.
    public Task<Flight> CreateAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        if (flight.ArrivalTime < flight.DepartureTime)
        {
            throw new ArgumentException("ArrivalTime cannot be before DepartureTime.", nameof(flight));
        }
        
        // Additional rules (validation, defaults, etc.) can be added here later.
        return flightRepository.AddAsync(flight, cancellationToken);
    }

    // SRP (Single Responsibility Principle): Update an existing flight.
    // The repository is responsible for applying the update.
    public Task<Flight?> UpdateAsync(int id, Flight flight, CancellationToken cancellationToken = default)
    {
        return flightRepository.UpdateAsync(flight, cancellationToken);
    }

    // SRP (Single Responsibility Principle): Delete an existing flight by delegating to the repository.
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // Future microcycle: handle not-found behaviour and domain rules around deletion.
        return flightRepository.DeleteAsync(id, cancellationToken);
    }

    // SRP (Single Responsibility Principle): 
    // Apply search filtering rules at the application layer while delegating data access to the repository.
    public async Task<IReadOnlyList<Flight>> SearchAsync(
        string? airline,
        string? departureAirport,
        string? arrivalAirport,
        DateTime? departureFrom,
        DateTime? departureTo,
        CancellationToken cancellationToken = default)
    {
        var flights = await flightRepository.GetAllAsync(cancellationToken);

        var query = flights.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(airline))
        {
            query = query.Where(f => f.Airline == airline);
        }

        if (!string.IsNullOrWhiteSpace(departureAirport))
        {
            query = query.Where(f => f.DepartureAirport == departureAirport);
        }

        if (!string.IsNullOrWhiteSpace(arrivalAirport))
        {
            query = query.Where(f => f.ArrivalAirport == arrivalAirport);
        }

        if (departureFrom.HasValue)
        {
            query = query.Where(f => f.DepartureTime >= departureFrom.Value);
        }

        if (departureTo.HasValue)
        {
            query = query.Where(f => f.DepartureTime <= departureTo.Value);
        }

        return [.. query];
    }
}
