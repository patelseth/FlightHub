using System.ComponentModel.DataAnnotations;
using FlightHub.Domain.Enums;

namespace FlightHub.Domain.Entities;

// SRP (Single Responsibility Principle): This class represents a flight in the domain model.
// It only describes flight data and does not handle persistence or validation.
public class Flight
{
    /// <summary>
    /// Unique identifier of the flight.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The official flight number (e.g., FH100).
    /// </summary>
    public required string FlightNumber { get; set; }

    /// <summary>
    /// The airline operating the flight.
    /// </summary>
    public required string Airline { get; set; }

    /// <summary>
    /// Departure airport code (e.g., WLG).
    /// </summary>
    public required string DepartureAirport { get; set; }

    /// <summary>
    /// Arrival airport code (e.g., AKL).
    /// </summary>
    public required string ArrivalAirport { get; set; }

    /// <summary>
    /// UTC departure timestamp.
    /// </summary>
    public DateTime DepartureTime { get; set; }

    /// <summary>
    /// UTC arrival timestamp.
    /// </summary>
    public DateTime ArrivalTime { get; set; }

    /// <summary>
    /// Current flight status (Scheduled, InAir, Delayed, etc.)
    /// </summary>
    public FlightStatus Status { get; set; }

}
