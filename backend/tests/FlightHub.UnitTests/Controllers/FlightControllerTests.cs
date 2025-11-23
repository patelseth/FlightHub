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

    [Fact]
    public async Task Update_ReturnsOkWithUpdatedFlight_WhenFlightExists()
    {
        // Arrange
        var existingId = 5;

        var updateRequest = new Flight
        {
            Id = existingId,
            FlightNumber = "FH555",
            Airline = "UpdateAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "MEL",
            DepartureTime = new DateTime(2025, 11, 28, 8, 0, 0, DateTimeKind.Utc),
            ArrivalTime = new DateTime(2025, 11, 28, 10, 0, 0, DateTimeKind.Utc),
            Status = FlightStatus.Scheduled
        };

        var updatedFlight = new Flight
        {
            Id = existingId,
            FlightNumber = updateRequest.FlightNumber,
            Airline = updateRequest.Airline,
            DepartureAirport = updateRequest.DepartureAirport,
            ArrivalAirport = updateRequest.ArrivalAirport,
            DepartureTime = updateRequest.DepartureTime,
            ArrivalTime = updateRequest.ArrivalTime,
            Status = updateRequest.Status
        };

        // Arrange the service mock to return the updated flight when UpdateAsync is called.
        _flightServiceMock
            .Setup(s => s.UpdateAsync(existingId, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedFlight);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.Update(existingId, updateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // Confirm the response body contains the updated flight.
        var returnedFlight = Assert.IsType<Flight>(okResult.Value);
        Assert.Equal(updatedFlight.Id, returnedFlight.Id);
        Assert.Equal(updatedFlight.FlightNumber, returnedFlight.FlightNumber);

        // Confirm the controller calls the service to update the flight.
        _flightServiceMock.Verify(
            s => s.UpdateAsync(existingId, updateRequest, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenFlightIsDeleted()
    {
        // Arrange
        var existingId = 5;

        // Arrange the service mock to complete successfully when DeleteAsync is called.
        _flightServiceMock
            .Setup(s => s.DeleteAsync(existingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.Delete(existingId);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);

        // Confirm the controller calls the service to delete the flight.
        _flightServiceMock.Verify(
            s => s.DeleteAsync(existingId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Search_ReturnsOkWithFlightsFromService()
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
                Airline = "TestAir",
                DepartureAirport = "WLG",
                ArrivalAirport = "CHC",
                DepartureTime = new DateTime(2025, 11, 27, 9, 0, 0, DateTimeKind.Utc),
                ArrivalTime = new DateTime(2025, 11, 27, 11, 0, 0, DateTimeKind.Utc),
                Status = FlightStatus.InAir
            }
        };

        var airline = "TestAir";
        var departureAirport = "WLG";
        var arrivalAirport = "AKL";
        var departureFrom = new DateTime(2025, 11, 25, 0, 0, 0, DateTimeKind.Utc);
        var departureTo = new DateTime(2025, 11, 27, 23, 59, 59, DateTimeKind.Utc);

        // Arrange the service mock to return the matching flights when SearchAsync is called.
        _flightServiceMock
            .Setup(s => s.SearchAsync(
                airline,
                departureAirport,
                arrivalAirport,
                departureFrom,
                departureTo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.Search(
            airline,
            departureAirport,
            arrivalAirport,
            departureFrom,
            departureTo);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // Confirm the response body contains a list of Flight objects.
        var returnedFlights = Assert.IsType<IReadOnlyList<Flight>>(okResult.Value, exactMatch: false);

        Assert.Equal(flights.Count, returnedFlights.Count);
        Assert.Equal(flights[0].Id, returnedFlights[0].Id);
        Assert.Equal(flights[1].FlightNumber, returnedFlights[1].FlightNumber);

        // Confirm the controller calls the service to search for flights.
        _flightServiceMock.Verify(
            s => s.SearchAsync(
                airline,
                departureAirport,
                arrivalAirport,
                departureFrom,
                departureTo,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenFlightDoesNotExist()
    {
        // Arrange
        var id = 999;

        _flightServiceMock
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Flight?)null);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.GetById(id);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _flightServiceMock.Verify(
            s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var invalidFlight = new Flight(); // missing required fields

        var controller = new FlightsController(_flightServiceMock.Object);
        controller.ModelState.AddModelError("FlightNumber", "Required");

        // Act
        var result = await controller.Create(invalidFlight);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(badRequest.Value);

        _flightServiceMock.Verify(
            s => s.CreateAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var id = 5;
        var invalidFlight = new Flight { Id = id };

        var controller = new FlightsController(_flightServiceMock.Object);
        controller.ModelState.AddModelError("FlightNumber", "Required");

        // Act
        var result = await controller.Update(id, invalidFlight);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(badRequest.Value);

        _flightServiceMock.Verify(
            s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<Flight>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenFlightDoesNotExist()
    {
        // Arrange
        var id = 5;
        var updateRequest = new Flight { Id = id };

        _flightServiceMock
            .Setup(s => s.UpdateAsync(id, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Flight?)null);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.Update(id, updateRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _flightServiceMock.Verify(
            s => s.UpdateAsync(id, updateRequest, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Delete_ReturnsNotFound_WhenFlightDoesNotExist()
    {
        // Arrange
        var id = 10;

        _flightServiceMock
            .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var controller = new FlightsController(_flightServiceMock.Object);

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        _flightServiceMock.Verify(
            s => s.DeleteAsync(id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
