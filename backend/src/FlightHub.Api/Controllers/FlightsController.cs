using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FlightHub.Api.Controllers;

// DIP (Dependency Inversion Principle): methods/endpoints rely on the IFlightService abstraction, so the controller
// stays decoupled from any specific data-access implementation.

[ApiController]
[Route("api/[controller]")]
public class FlightsController(IFlightService flightService) : ControllerBase
{
    // SRP (Single Responsibility Principle): This endpoint handles the GET /api/flights request and delegates
    // flight retrieval to the service.
    public async Task<ActionResult<IReadOnlyList<Flight>>> GetAll()
    {
        var flights = await flightService.GetAllAsync();
        return Ok(flights);
    }

    // SRP (Single Responsibility Principle): 
    // This endpoint handles GET /api/flights/{id} and delegates retrieval to the service.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Flight>> GetById(int id)
    {
        var flight = await flightService.GetByIdAsync(id);

        if (flight is null)
        {
            return NotFound();
        } 
        return Ok(flight);
    }

    // SRP (Single Responsibility Principle): 
    // This endpoint handles POST /api/flights and delegates creation to the service.
    [HttpPost]
    public async Task<ActionResult<Flight>> Create([FromBody] Flight flight)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdFlight = await flightService.CreateAsync(flight);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdFlight.Id },
            createdFlight);
    }

    // SRP (Single Responsibility Principle): This endpoint handles PUT /api/flights/{id} 
    // and delegates update operations to the service.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Flight>> Update(int id, [FromBody] Flight flight)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedFlight = await flightService.UpdateAsync(id, flight);

        if (updatedFlight is null)
        {
            return NotFound();
        }

        // Future microcycle: add id/body consistency checks, additional validation, logging.
        return Ok(updatedFlight);
    }

    // SRP (Single Responsibility Principle): 
    // This endpoint handles DELETE /api/flights/{id} and delegates the deletion to the service.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await flightService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // SRP (Single Responsibility Principle): 
    // This endpoint handles GET /api/flights/search and delegates search to the service.
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

        return Ok(flights);
    }
}
