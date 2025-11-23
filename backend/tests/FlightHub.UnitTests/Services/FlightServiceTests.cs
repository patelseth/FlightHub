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
    public async Task GetAllAsync_ReturnsAllFlights()
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

    [Fact]
    public async Task GetByIdAsync_ReturnsFlight_WhenItExists()
    {
        // Arrange
        var flight = new Flight
        {
            Id = 1,
            FlightNumber = "FH100",
            Airline = "TestAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "AKL",
            DepartureTime = new DateTime(2025, 11, 26, 9, 0, 0, DateTimeKind.Utc),
            ArrivalTime = new DateTime(2025, 11, 26, 10, 0, 0, DateTimeKind.Utc),
            Status = FlightStatus.Scheduled
        };

        // Set up the repository to return this flight when queried by id.
        _repositoryMock
            .Setup(r => r.GetByIdAsync(flight.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flight);

        // Act
        var result = await _flightService.GetByIdAsync(flight.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(flight.Id, result!.Id);
        Assert.Equal(flight.FlightNumber, result.FlightNumber);

        // Confirm the service asks the repository for the specific flight id.
        _repositoryMock.Verify(r => r.GetByIdAsync(flight.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedFlight()
    {
        // Arrange
        var newFlight = new Flight
        {
            Id = 0, // new entity - id assigned by repository or data store
            FlightNumber = "FH300",
            Airline = "CreateAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "SYD",
            DepartureTime = new DateTime(2025, 11, 27, 9, 0, 0, DateTimeKind.Utc),
            ArrivalTime = new DateTime(2025, 11, 27, 11, 0, 0, DateTimeKind.Utc),
            Status = FlightStatus.Scheduled
        };

        var createdFlight = new Flight
        {
            Id = 10, // simulate a generated id from the data store
            FlightNumber = newFlight.FlightNumber,
            Airline = newFlight.Airline,
            DepartureAirport = newFlight.DepartureAirport,
            ArrivalAirport = newFlight.ArrivalAirport,
            DepartureTime = newFlight.DepartureTime,
            ArrivalTime = newFlight.ArrivalTime,
            Status = newFlight.Status
        };

        // Arrange the repository mock to return the created flight when adding a new one.
        _repositoryMock
            .Setup(r => r.AddAsync(newFlight, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlight);

        // Act
        var result = await _flightService.CreateAsync(newFlight);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdFlight.Id, result!.Id);
        Assert.Equal(createdFlight.FlightNumber, result.FlightNumber);

        // Confirm the service passes the new flight to the repository once.
        _repositoryMock.Verify(r => r.AddAsync(newFlight, It.IsAny<CancellationToken>()), Times.Once);
    }

}
