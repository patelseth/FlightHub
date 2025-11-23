using FlightHub.Application.Interfaces;
using FlightHub.Application.Services;
using FlightHub.Domain.Entities;
using FlightHub.Domain.Enums;
using Moq;

namespace FlightHub.UnitTests.Services;

public class FlightServiceTests
{
    private readonly Mock<IFlightRepository> _repositoryMock;
    private readonly IFlightService _flightService;

    public FlightServiceTests()
    {
        // DIP (Dependency Inversion Principle): FlightService depends on an abstraction (IFlightRepository),
        // which we replace here with a mock to keep this test focused only on service behavior.
        _repositoryMock = new Mock<IFlightRepository>();
        _flightService = new FlightService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlightsFromRepository()
    {
        // Arrange
        var flights = new List<Flight>
        {
            new()
            {
                Id = 1,
                FlightNumber = "FH100",
                Airline = "TestAir",
                DepartureAirport = "WLG",
                ArrivalAirport = "AKL",
                DepartureTime = new DateTime(2025, 11, 26, 9, 0, 0, DateTimeKind.Utc),
                ArrivalTime = new DateTime(2025, 11, 26, 10, 0, 0, DateTimeKind.Utc),
                Status = FlightStatus.Scheduled
            },
            new()
            {
                Id = 2,
                FlightNumber = "FH200",
                Airline = "SampleAir",
                DepartureAirport = "AKL",
                ArrivalAirport = "CHC",
                DepartureTime = new DateTime(2025, 11, 26, 12, 0, 0, DateTimeKind.Utc),
                ArrivalTime = new DateTime(2025, 11, 26, 13, 30, 0, DateTimeKind.Utc),
                Status = FlightStatus.InAir
            }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _flightService.GetAllAsync();

        // Assert
        Assert.Equal(flights.Count, result.Count);
        Assert.Equal(flights[0].Id, result[0].Id);
        Assert.Equal(flights[1].FlightNumber, result[1].FlightNumber);

        // Confirm the service retrieves flights by calling the repository
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
