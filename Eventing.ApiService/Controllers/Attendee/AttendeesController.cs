using Eventing.ApiService.Data;
using Eventing.ApiService.Data.Entities;
using Eventing.ApiService.Data.Enums;
using Eventing.ApiService.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventing.ApiService.Controllers.Attendee;

[Authorize]
public class AttendeesController(
    EventingDbContext dbContext,
    CurrentUserService currentUserService) : ApiBaseController
{
    /// <summary>Register the current user for an event.</summary>
    [HttpPost("events/{eventId:guid}/register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AttendeeRegistrationResponse>> RegisterAsync(
        [FromRoute] Guid eventId, CancellationToken ct)
    {
        var ev = await dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId, ct);
        if (ev is null) return NotFound("Event not found.");

        var userId = currentUserService.UserId;

        var existing = await dbContext.Attendees
            .FirstOrDefaultAsync(a => a.EventId == eventId && a.ResponderId == userId, ct);

        if (existing is not null)
            return Conflict(BuildResponse(existing, ev, userId));

        var attendee = new Data.Entities.Attendee
        {
            EventId      = eventId,
            ResponderId  = userId,
            IsOrganizer  = false,
            RsvpResponse = RsvpResponse.Pending,
            RespondedAt  = DateTime.UtcNow
        };

        dbContext.Attendees.Add(attendee);
        await dbContext.SaveChangesAsync(ct);

        return Created(string.Empty, BuildResponse(attendee, ev, userId));
    }

    /// <summary>Check if current user is registered for an event.</summary>
    [HttpGet("events/{eventId:guid}/my-registration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttendeeRegistrationResponse>> GetMyRegistrationAsync(
        [FromRoute] Guid eventId, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var attendee = await dbContext.Attendees
            .Include(a => a.Event)
            .FirstOrDefaultAsync(a => a.EventId == eventId && a.ResponderId == userId, ct);

        if (attendee is null) return NotFound();
        return Ok(BuildResponse(attendee, attendee.Event, userId));
    }

    /// <summary>Get all events the current user is registered for — includes live event status.</summary>
    [HttpGet("my-events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AttendeeRegistrationResponse>>> GetMyEventsAsync(
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        var attendees = await dbContext.Attendees
            .Include(a => a.Event)
            .Where(a => a.ResponderId == userId)
            .OrderByDescending(a => a.Event.StartTime)
            .ToListAsync(ct);

        var result = attendees.Select(a => BuildResponse(a, a.Event, userId)).ToList();
        return Ok(result);
    }

    /// <summary>Cancel registration for an event.</summary>
    [HttpDelete("events/{eventId:guid}/register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelRegistrationAsync(
        [FromRoute] Guid eventId, CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var attendee = await dbContext.Attendees
            .FirstOrDefaultAsync(a => a.EventId == eventId && a.ResponderId == userId, ct);

        if (attendee is null) return NotFound();

        dbContext.Attendees.Remove(attendee);
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    // ── Helpers ────────────────────────────────────────────────

    private static AttendeeRegistrationResponse BuildResponse(
        Data.Entities.Attendee attendee, Data.Entities.Event ev, Guid userId) =>
        new(attendee.Id, ev.Id, userId,
            ev.Title, ev.StartTime, ev.EndTime,
            ev.Location, ev.LocationType.ToString(),
            attendee.RsvpResponse.ToString(),
            attendee.RespondedAt, attendee.IsOrganizer,
            ComputeEventStatus(ev),
            attendee.Attended);

    private static string ComputeEventStatus(Data.Entities.Event ev)
    {
        if (ev.Status == EventStatus.Cancelled) return "Cancelled";
        var now = DateTime.UtcNow;
        if (ev.EndTime   <  now) return "Ended";
        if (ev.StartTime <= now) return "Live";
        return "Upcoming";
    }
}

public sealed record AttendeeRegistrationResponse(
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
    string EventStatus,   // computed at read-time: Upcoming / Live / Ended / Cancelled
    bool Attended         // true = admin verified present
);
