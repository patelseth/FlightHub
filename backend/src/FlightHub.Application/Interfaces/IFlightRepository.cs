using FlightHub.Domain.Entities;

namespace FlightHub.Application.Interfaces;

// DIP (Dependency Inversion Principle):
// The application depends on this abstraction rather than a concrete repository,
// keeping it independent of any concrete data-access implementation.
// The Infrastructure layer will provide the real implementation.
public interface IFlightRepository
{
    Task<IReadOnlyList<Flight>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default);
}
