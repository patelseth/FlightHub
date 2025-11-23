using FlightHub.Domain.Entities;
using FlightHub.Infrastructure.Data;
using FlightHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlightHub.UnitTests.Infrastructure;

public class FlightRepositoryTests
{
    private FlightDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FlightDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FlightDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlights()
    {
        // Arrange
        var context = CreateDbContext();
        context.Flights.AddRange(
            new Flight { Id = 1, FlightNumber = "FH100", Airline = "TestAir" },
            new Flight { Id = 2, FlightNumber = "FH200", Airline = "SampleAir" }
        );

        await context.SaveChangesAsync();

        var repo = new FlightRepository(context);

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFlight_WhenExists()
    {
        // Arrange
        var context = CreateDbContext();
        var flight = new Flight { Id = 1, FlightNumber = "FH100", Airline = "TestAir" };

        context.Flights.Add(flight);
        await context.SaveChangesAsync();

        var repo = new FlightRepository(context);

        // Act
        var result = await repo.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FH100", result!.FlightNumber);
    }

    [Fact]
    public async Task AddAsync_PersistsFlight()
    {
        // Arrange
        var context = CreateDbContext();
        var repo = new FlightRepository(context);

        var flight = new Flight
        {
            FlightNumber = "FH300",
            Airline = "CreateAir"
        };

        // Act
        var created = await repo.AddAsync(flight);

        // Assert
        Assert.True(created.Id > 0);
        Assert.Equal("FH300", created.FlightNumber);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingFlight()
    {
        // Arrange
        var context = CreateDbContext();
        var existing = new Flight { Id = 1, FlightNumber = "OldAir", Airline = "Before" };

        context.Flights.Add(existing);
        await context.SaveChangesAsync();

        var repo = new FlightRepository(context);
        existing.FlightNumber = "NewAir";

        // Act
        var updated = await repo.UpdateAsync(existing);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("NewAir", updated!.FlightNumber);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingFlight()
    {
        // Arrange
        var context = CreateDbContext();
        var flight = new Flight { Id = 1, FlightNumber = "DeleteMe" };

        context.Flights.Add(flight);
        await context.SaveChangesAsync();

        var repo = new FlightRepository(context);

        // Act
        var deleted = await repo.DeleteAsync(1);

        // Assert
        Assert.True(deleted);
        Assert.Null(await context.Flights.FindAsync(1));
    }
}
