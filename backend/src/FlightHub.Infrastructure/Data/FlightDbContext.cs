using FlightHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightHub.Infrastructure.Data;

// SRP (Single Responsibility Principle): 
// This DbContext is responsible for mapping Flight entities to the underlying database.
// DIP (Dependency Inversion Principle): 
// Higher layers depend on IFlightRepository rather than directly on this context.
public class FlightDbContext(DbContextOptions<FlightDbContext> options) : DbContext(options)
{
    public DbSet<Flight> Flights => Set<Flight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var flight = modelBuilder.Entity<Flight>();

        flight.HasKey(f => f.Id);

        flight.Property(f => f.FlightNumber)
            .IsRequired();

        flight.Property(f => f.Airline)
            .IsRequired();

        flight.Property(f => f.DepartureAirport)
            .IsRequired();

        flight.Property(f => f.ArrivalAirport)
            .IsRequired();
    }
}
