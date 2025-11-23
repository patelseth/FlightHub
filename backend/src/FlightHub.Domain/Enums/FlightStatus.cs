namespace FlightHub.Domain.Enums;

// SRP (Single Responsibility Principle):
// This enum’s only job is to define the possible statuses a flight can have.
// It stays in the Domain layer because status values are part of the core business model.
public enum FlightStatus
{
    Scheduled,
    Delayed,
    Cancelled,
    InAir,
    Landed
}
