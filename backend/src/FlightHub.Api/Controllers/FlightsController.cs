using FlightHub.Application.Interfaces;
using FlightHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FlightHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController(IFlightService flightService) : ControllerBase
{
    // SRP (Single Responsibility Principle): This endpoint handles the GET /api/flights request and delegates
    // flight retrieval to the application layer.
    // DIP (Dependency Inversion Principle): It relies on the IFlightService abstraction, so the controller
    // stays decoupled from any specific implementation.
    public async Task<ActionResult<IReadOnlyList<Flight>>> GetAll()
    {
        var flights = await flightService.GetAllAsync();

        return Ok(flights);
    }

    // SRP (Single Responsibility Principle): This endpoint handles GET /api/flights/{id} and delegates retrieval to the application layer.
    // DIP (Dependency Inversion Principle): It relies on the IFlightService abstraction, so the controller
    // stays decoupled from any specific implementation.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Flight>> GetById(int id)
    {
        var flight = await flightService.GetByIdAsync(id);

        // (404 behavior will be covered in a future microcycle).
        return Ok(flight);
    }
}
