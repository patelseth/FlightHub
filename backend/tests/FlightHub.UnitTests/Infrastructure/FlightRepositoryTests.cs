using FlightHub.Domain.Entities;
using FlightHub.Infrastructure.Data;
using FlightHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightHub.UnitTests.Infrastructure;

public class FlightRepositoryTests
{
    #region Setup

    private static FlightDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FlightDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FlightDbContext(options);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlights()
    {
        // Arrange
        var context = CreateDbContext();
        context.Flights.AddRange(
            new Flight
            {
                Id = 1,
                FlightNumber = "FH100",
                Airline = "TestAir",
                DepartureAirport = "WLG",
                ArrivalAirport = "AKL"
            },
            new Flight
            {
                Id = 2,
                FlightNumber = "FH200",
                Airline = "SampleAir",
                DepartureAirport = "AKL",
                ArrivalAirport = "CHC"
            }
        );

        await context.SaveChangesAsync();

        var repo = new FlightRepository(context);

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ReturnsFlight_WhenExists()
    {
        // Arrange
        var context = CreateDbContext();
        var flight = new Flight
        {
            Id = 1,
            FlightNumber = "FH100",
            Airline = "TestAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "AKL"
        };

        context.Flights.Add(flight);
        await context.SaveChangesAsync();

        var repo = new FlightRepository(context);

        // Act
        var result = await repo.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FH100", result!.FlightNumber);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_PersistsFlight()
    {
        // Arrange
        var context = CreateDbContext();
        var repo = new FlightRepository(context);

        var flight = new Flight
        {
            FlightNumber = "FH300",
            Airline = "CreateAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "SYD"
        };

        // Act
        var created = await repo.AddAsync(flight);

        // Assert
        Assert.True(created.Id > 0);
        Assert.Equal("FH300", created.FlightNumber);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_UpdatesExistingFlight()
    {
        // Arrange
        var context = CreateDbContext();
        var existing = new Flight
        {
            Id = 1,
            FlightNumber = "OldAir",
            Airline = "Before",
            DepartureAirport = "WLG",
            ArrivalAirport = "AKL"
        };

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

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_RemovesExistingFlight()
    {
        // Arrange
        var context = CreateDbContext();
        var flight = new Flight
        {
            Id = 1,
            FlightNumber = "DeleteMe",
            Airline = "TestAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "AKL"
        };

        context.Flights.Add(flight);
        await context.SaveChangesAsync();

        var repo = new FlightRepository(context);

        // Act
        var deleted = await repo.DeleteAsync(1);

        // Assert
        Assert.True(deleted);
        Assert.Null(await context.Flights.FindAsync(1));
    }

    #endregion

    #region Logging

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_AndLogsWarning_WhenFlightNotFound()
    {
        // Arrange
        var context = CreateDbContext();
        var loggerMock = new Mock<ILogger<FlightRepository>>();
        var repo = new FlightRepository(context, loggerMock.Object);

        // Act
        var result = await repo.DeleteAsync(999);

        // Assert
        Assert.False(result);

        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("not found", StringComparison.OrdinalIgnoreCase)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion
}
