using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FlightHub.Api.Controllers;

// DIP (Dependency Inversion Principle): methods/endpoints rely on the IFlightService abstraction, so the controller
// stays decoupled from any specific data-access implementation.

[ApiController]
[Route("api/[controller]")]
public class FlightsController(IFlightService flightService, ILogger<FlightsController> logger) : ControllerBase
{
    /// <summary>
    /// Returns all flights from the system.
    /// </summary>
    /// <returns>A list of flights.</returns>
    /// <response code="200">Returns all flights.</response>
    /// <remarks>
    /// SRP (Single Responsibility Principle): This endpoint handles the GET /api/flights request and delegates
    /// flight retrieval to the service.
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Flight>>> GetAll()
    {
        var flights = await flightService.GetAllAsync();

        logger.LogInformation("Returning {Count} flights", flights.Count);

        return Ok(flights);
    }

    /// <summary>
    /// Returns a single flight by id.
    /// </summary>
    /// <param name="id">The flight identifier.</param>
    /// <returns>The matching flight if found.</returns>
    /// <response code="200">Flight found.</response>
    /// <response code="404">Flight not found.</response>
    /// <remarks>
    /// SRP (Single Responsibility Principle): 
    /// This endpoint handles GET /api/flights/{id} and delegates retrieval to the service.
    /// </remarks>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Flight>> GetById(int id)
    {
        var flight = await flightService.GetByIdAsync(id);

        if (flight is null)
        {
            logger.LogWarning("Flight with id {Id} not found", id);
            return NotFound();
        }

        logger.LogInformation("Flight with id {Id} retrieved", id);
        return Ok(flight);
    }

    /// <summary>
    /// Creates a new flight.
    /// </summary>
    /// <param name="flight">The flight to create.</param>
    /// <returns>The created flight.</returns>
    /// <response code="201">Flight created successfully.</response>
    /// <response code="400">Invalid request body.</response>
    /// <remarks>
    /// SRP (Single Responsibility Principle): 
    /// This endpoint handles POST /api/flights and delegates creation to the service.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Flight>> Create([FromBody] Flight flight)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid flight model submitted for creation");
            return BadRequest(ModelState);
        }

        var createdFlight = await flightService.CreateAsync(flight);

        logger.LogInformation("Created flight with id {Id}", createdFlight.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdFlight.Id },
            createdFlight);
    }

    /// <summary>
    /// Updates an existing flight.
    /// </summary>
    /// <param name="id">The id of the flight being updated.</param>
    /// <param name="flight">The updated flight model.</param>
    /// <returns>The updated flight.</returns>
    /// <response code="200">Successfully updated.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">Flight not found.</response>
    /// <remarks>
    /// SRP (Single Responsibility Principle): This endpoint handles PUT /api/flights/{id} 
    /// and delegates update operations to the service.
    /// </remarks>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Flight>> Update(int id, [FromBody] Flight flight)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid flight model submitted for update for id {Id}", id);
            return BadRequest(ModelState);
        }

        var updatedFlight = await flightService.UpdateAsync(id, flight);

        if (updatedFlight is null)
        {
            logger.LogWarning("Flight with id {Id} not found for update", id);
            return NotFound();
        }

        logger.LogInformation("Updated flight with id {Id}", id);

        // Future microcycle: add id/body consistency checks, additional validation, logging.
        return Ok(updatedFlight);
    }

    /// <summary>
    /// Deletes a flight.
    /// </summary>
    /// <param name="id">The flight id.</param>
    /// <response code="204">Successfully deleted.</response>
    /// <response code="404">Flight not found.</response>
    /// <remarks>
    /// SRP (Single Responsibility Principle): 
    /// This endpoint handles DELETE /api/flights/{id} and delegates the deletion to the service.
    /// </remarks>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await flightService.DeleteAsync(id);

        if (!deleted)
        {
            logger.LogWarning("Flight with id {Id} not found for deletion", id);
            return NotFound();
        }

        logger.LogInformation("Deleted flight with id {Id}", id);

        return NoContent();
    }

    /// <summary>
    /// Searches flights using optional filters.
    /// </summary>
    /// <param name="airline">Filter by airline.</param>
    /// <param name="departureAirport">Filter by departure airport.</param>
    /// <param name="arrivalAirport">Filter by arrival airport.</param>
    /// <param name="departureFrom">Earliest departure time.</param>
    /// <param name="departureTo">Latest departure time.</param>
    /// <returns>A filtered list of flights.</returns>
    /// <response code="200">Flights returned.</response>
    /// <remarks>
    /// SRP (Single Responsibility Principle): 
    /// This endpoint handles GET /api/flights/search and delegates search to the service.
    /// </remarks>
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<Flight>>> Search(
        [FromQuery] string? airline,
        [FromQuery] string? departureAirport,
        [FromQuery] string? arrivalAirport,
        [FromQuery] DateTime? departureFrom,
        [FromQuery] DateTime? departureTo)
    {
        var flights = await flightService.SearchAsync(
            airline,
            departureAirport,
            arrivalAirport,
            departureFrom,
            departureTo);

        logger.LogInformation(
            "Search query: airline={Airline}, departureAirport={DepartureAirport}, arrivalAirport={ArrivalAirport}, departureFrom={DepartureFrom}, departureTo={DepartureTo}. Returned {Count} results.",
            airline, departureAirport, arrivalAirport, departureFrom, departureTo, flights.Count);

        return Ok(flights);
    }
}
