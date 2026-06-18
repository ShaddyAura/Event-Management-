namespace Eventing.Web.Features.Events.Models;

public sealed record EventDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartTime,
    DateTime EndTime,
    string LocationType,
    string Location,
    string Status,
    Guid CreatedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public sealed record RegistrationDto(
    Guid AttendeeId,
    Guid EventId,
    Guid UserId,
    string EventTitle,
    DateTime StartTime,
    DateTime EndTime,
    string Location,
    string LocationType,
    string RsvpResponse,
    DateTime? RespondedAt,
    bool IsOrganizer,
    string EventStatus,   // Upcoming / Live / Ended / Cancelled from API
    bool Attended         // true = admin verified present
);
