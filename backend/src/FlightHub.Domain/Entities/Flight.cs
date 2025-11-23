using System.ComponentModel.DataAnnotations;
using FlightHub.Domain.Enums;

namespace FlightHub.Domain.Entities;

// SRP (Single Responsibility Principle): This class represents a flight in the domain model.
// It only describes flight data and does not handle persistence or validation.
public class Flight
{
    public int Id { get; set; }

    [Required]
    public string FlightNumber { get; set; } = string.Empty;

    [Required]
    public string Airline { get; set; } = string.Empty;

    [Required]
    public string DepartureAirport { get; set; } = string.Empty;

    [Required]
    public string ArrivalAirport { get; set; } = string.Empty;

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public FlightStatus Status { get; set; }
}
