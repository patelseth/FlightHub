using FlightHub.Api.Controllers;
using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;
using FlightHub.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightHub.UnitTests.Controllers;

public class FlightsControllerTests
{
    private readonly Mock<IFlightService> _flightServiceMock;
    private readonly Mock<ILogger<FlightsController>> _loggerMock;

    // DIP (Dependency Inversion Principle): The controller depends on IFlightService, which we mock.
    public FlightsControllerTests()
    {
        _flightServiceMock = new Mock<IFlightService>();
        _loggerMock = new Mock<ILogger<FlightsController>>();
    }

    private FlightsController CreateController()
        => new FlightsController(_flightServiceMock.Object, _loggerMock.Object);

    // -------------------------------------------------------
    #region GET /api/flights (GetAll)
    // -------------------------------------------------------

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

        _flightServiceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights);

        var controller = CreateController();

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedFlights = Assert.IsType<IReadOnlyList<Flight>>(okResult.Value, exactMatch: false);

        Assert.Equal(flights.Count, returnedFlights.Count);
        Assert.Equal(flights[0].Id, returnedFlights[0].Id);

        _flightServiceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    // -------------------------------------------------------
    #region GET /api/flights/{id}
    // -------------------------------------------------------

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

        _flightServiceMock
            .Setup(s => s.GetByIdAsync(flight.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flight);

        var controller = CreateController();

        // Act
        var result = await controller.GetById(flight.Id);

        // Assert
        var okResponse = Assert.IsType<OkObjectResult>(result.Result);
        var returnedFlight = Assert.IsType<Flight>(okResponse.Value);

        Assert.Equal(flight.Id, returnedFlight.Id);

        _flightServiceMock.Verify(s => s.GetByIdAsync(flight.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenFlightDoesNotExist()
    {
        // Arrange
        var id = 999;

        _flightServiceMock
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Flight?)null);

        var controller = CreateController();

        // Act
        var result = await controller.GetById(id);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _flightServiceMock.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    // -------------------------------------------------------
    #region POST /api/flights
    // -------------------------------------------------------

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

        _flightServiceMock
            .Setup(s => s.CreateAsync(newFlight, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlight);

        var controller = CreateController();

        // Act
        var result = await controller.Create(newFlight);

        // Assert
        var createdAtAction = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedFlight = Assert.IsType<Flight>(createdAtAction.Value);

        Assert.Equal(createdFlight.Id, returnedFlight.Id);

        _flightServiceMock.Verify(
            s => s.CreateAsync(newFlight, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var invalidFlight = new Flight
        {
            FlightNumber = "FH000",
            Airline = "InvalidAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "AKL"
        };

        var controller = CreateController();
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

    #endregion

    // -------------------------------------------------------
    #region PUT /api/flights/{id}
    // -------------------------------------------------------

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

        _flightServiceMock
            .Setup(s => s.UpdateAsync(existingId, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedFlight);

        var controller = CreateController();

        // Act
        var result = await controller.Update(existingId, updateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedFlight = Assert.IsType<Flight>(okResult.Value);

        Assert.Equal(updatedFlight.Id, returnedFlight.Id);

        _flightServiceMock.Verify(
            s => s.UpdateAsync(existingId, updateRequest, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var id = 5;
        var invalidFlight = new Flight
        {
            Id = id,
            FlightNumber = "FH000",
            Airline = "InvalidAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "AKL"
        };

        var controller = CreateController();
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
        var updateRequest = new Flight
        {
            Id = id,
            FlightNumber = "FH000",
            Airline = "UpdateAir",
            DepartureAirport = "WLG",
            ArrivalAirport = "AKL"
        };

        _flightServiceMock
            .Setup(s => s.UpdateAsync(id, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Flight?)null);

        var controller = CreateController();

        // Act
        var result = await controller.Update(id, updateRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _flightServiceMock.Verify(
            s => s.UpdateAsync(id, updateRequest, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    // -------------------------------------------------------
    #region DELETE /api/flights/{id}
    // -------------------------------------------------------

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenFlightIsDeleted()
    {
        // Arrange
        var existingId = 5;

        _flightServiceMock
            .Setup(s => s.DeleteAsync(existingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var controller = CreateController();

        // Act
        var result = await controller.Delete(existingId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _flightServiceMock.Verify(
            s => s.DeleteAsync(existingId, It.IsAny<CancellationToken>()),
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

        var controller = CreateController();

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        _flightServiceMock.Verify(
            s => s.DeleteAsync(id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    // -------------------------------------------------------
    #region SEARCH
    // -------------------------------------------------------

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

        _flightServiceMock
            .Setup(s => s.SearchAsync(
                airline,
                departureAirport,
                arrivalAirport,
                departureFrom,
                departureTo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights);

        var controller = CreateController();

        // Act
        var result = await controller.Search(
            airline,
            departureAirport,
            arrivalAirport,
            departureFrom,
            departureTo);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedFlights = Assert.IsType<IReadOnlyList<Flight>>(okResult.Value, exactMatch: false);

        Assert.Equal(flights.Count, returnedFlights.Count);

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

    #endregion

    // -------------------------------------------------------
    #region LOGGING TESTS
    // -------------------------------------------------------

    [Fact]
    public async Task GetById_ReturnsNotFound_AndLogsWarning_WhenFlightDoesNotExist()
    {
        // Arrange
        const int missingId = 999;

        _flightServiceMock
            .Setup(s => s.GetByIdAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Flight?)null);

        var controller = CreateController();

        // Act
        var result = await controller.GetById(missingId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("not found", StringComparison.OrdinalIgnoreCase)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_AndLogsInformation_OnSuccess()
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

        _flightServiceMock
            .Setup(s => s.CreateAsync(newFlight, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlight);

        var controller = CreateController();

        // Act
        var result = await controller.Create(newFlight);

        // Assert
        var createdAtAction = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedFlight = Assert.IsType<Flight>(createdAtAction.Value);

        Assert.Equal(createdFlight.Id, returnedFlight.Id);

        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("created", StringComparison.OrdinalIgnoreCase)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion
}
