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

        // (404 behavior will be covered in a future microcycle).
        return Ok(flight);
    }

    // SRP (Single Responsibility Principle): 
    // This endpoint handles POST /api/flights and delegates creation to the service.
    [HttpPost]
    public async Task<ActionResult<Flight>> Create([FromBody] Flight flight)
    {
        var createdFlight = await flightService.CreateAsync(flight);

        // Minimal happy-path implementation: return 201 as well as the created flight.
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
        var updatedFlight = await flightService.UpdateAsync(id, flight);

        // Future microcycle: Add validation (id mismatch), not-found handling, and richer error responses here.
        return Ok(updatedFlight);
    }

    // SRP (Single Responsibility Principle): This endpoint handles DELETE /api/flights/{id} and delegates the deletion to the service.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await flightService.DeleteAsync(id);

        // Future microcycle: return 404 when the flight does not exist and add logging/validation.
        return NoContent();
    }
}
