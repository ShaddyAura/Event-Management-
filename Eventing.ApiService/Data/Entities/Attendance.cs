using System.ComponentModel.DataAnnotations.Schema;

namespace Eventing.ApiService.Data.Entities;

public sealed class Attendance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))]
    public Event Event { get; set; } = null!;
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public Profile User { get; set; } = null!;
    public bool Attended { get; set; } = false;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
