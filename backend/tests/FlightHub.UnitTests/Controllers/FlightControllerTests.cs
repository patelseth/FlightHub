using FlightHub.Api.Controllers;
using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;
using FlightHub.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlightHub.UnitTests.Controllers;

public class FlightsControllerTests
{
    private readonly Mock<IFlightService> _flightServiceMock;

    // DIP (Dependency Inversion Principle): The controller will depend on the IFlightService abstraction, which we mock here.
    public FlightsControllerTests()
    {
        _flightServiceMock = new Mock<IFlightService>();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithFlightsFromService()
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

        // Arrange the service mock to return our test flights when GetAllAsync is called.
        _flightServiceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // Confirm the response body contains a list of Flight objects.
        var returnedFlights = Assert.IsType<IReadOnlyList<Flight>>(okResult.Value, exactMatch: false);

        Assert.Equal(flights.Count, returnedFlights.Count);
        Assert.Equal(flights[0].Id, returnedFlights[0].Id);
        Assert.Equal(flights[1].FlightNumber, returnedFlights[1].FlightNumber);

        // Confirm the controller calls the service to retrieve flights.
        _flightServiceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsOkWithFlight_WhenFlightExists()
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

        // Configure the mock service to return the flight when requested by id.
        _flightServiceMock
            .Setup(s => s.GetByIdAsync(flight.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flight);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.GetById(flight.Id);

        // Assert
        var okResponse = Assert.IsType<OkObjectResult>(result.Result);

        // Confirm the response body contains the expected flight.
        var returnedFlight = Assert.IsType<Flight>(okResponse.Value);

        Assert.Equal(flight.Id, returnedFlight.Id);
        Assert.Equal(flight.FlightNumber, returnedFlight.FlightNumber);

        // Confirm the controller queries the service for the specific flight id.
        _flightServiceMock.Verify(s => s.GetByIdAsync(flight.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithCreatedFlight()
    {
        // Arrange
        var newFlight = new Flight
        {
            Id = 0,
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
            Id = 10,
            FlightNumber = newFlight.FlightNumber,
            Airline = newFlight.Airline,
            DepartureAirport = newFlight.DepartureAirport,
            ArrivalAirport = newFlight.ArrivalAirport,
            DepartureTime = newFlight.DepartureTime,
            ArrivalTime = newFlight.ArrivalTime,
            Status = newFlight.Status
        };

        // Arrange the service mock to return the created flight 
        _flightServiceMock
            .Setup(s => s.CreateAsync(newFlight, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlight);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.Create(newFlight);

        // Assert
        var createdAtAction = Assert.IsType<CreatedAtActionResult>(result.Result);

        // Confirm the response body contains the created flight.
        var returnedFlight = Assert.IsType<Flight>(createdAtAction.Value);
        Assert.Equal(createdFlight.Id, returnedFlight.Id);
        Assert.Equal(createdFlight.FlightNumber, returnedFlight.FlightNumber);

        // Confirm the controller calls the service with the new flight exactly once.
        _flightServiceMock.Verify(
            s => s.CreateAsync(newFlight, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
