using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;

namespace FlightHub.Application.Services;

// SRP (Single Responsibility Principle): This service handle flight-related operations at the application layer. 
// DIP (Dependency Inversion Principle): It depends on IFlightRepository, keeping it independent of any
// concrete data-access implementation.
public class FlightService(IFlightRepository flightRepository) : IFlightService
{
    // SRP (Single Responsibility Principle): 
    // This method retrieves flights by delegating the data access to the repository.
    public Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Minimal implementation - simply delegate to the repository. 
        // Additional rules (filtering, sorting, and validation) can be added here later..
        return flightRepository.GetAllAsync(cancellationToken);
    }
}
