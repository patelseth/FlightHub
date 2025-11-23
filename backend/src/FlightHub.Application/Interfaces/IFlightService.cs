using FlightHub.Domain.Entities;

namespace FlightHub.Application.Interfaces;

// ISP (Interface Segregation Principle):
// This service interface exposes only the operations needed by higher layers.
// Keeping it small and focused makes the application easier to test and maintain.
public interface IFlightService
{
    Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
