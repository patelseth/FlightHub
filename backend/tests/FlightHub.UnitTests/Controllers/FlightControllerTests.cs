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
        var returnedFlights = Assert.IsAssignableFrom<IReadOnlyList<Flight>>(okResult.Value);

        Assert.Equal(flights.Count, returnedFlights.Count);
        Assert.Equal(flights[0].Id, returnedFlights[0].Id);
        Assert.Equal(flights[1].FlightNumber, returnedFlights[1].FlightNumber);

        // Confirm the controller calls the service to retrieve flights.
        _flightServiceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
