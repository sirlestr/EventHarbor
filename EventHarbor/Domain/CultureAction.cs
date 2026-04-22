using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHarbor.Domain;

public class CultureAction
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }

    public int Children { get; set; }
    public int Adults { get; set; }
    public int Seniors { get; set; }
    public int Disabled { get; set; }

    public CultureEventType Type { get; set; } = CultureEventType.Exhibition;
    public ExhibitionType Exhibition { get; set; } = ExhibitionType.General;
    public Organiser Organiser { get; set; } = Organiser.Museum;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    public bool IsFree { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public int TotalAttendance => Children + Adults + Seniors + Disabled;

    [NotMapped]
    public string DisplayId => $"EV-{CreatedAt:yyyy}-{Id:D3}";

    [NotMapped]
    public EventStatus Status
    {
        get
        {
            var now = DateTime.Now;
            if (EndAt.HasValue && EndAt.Value < now) return EventStatus.Ended;
            if (StartAt.HasValue && StartAt.Value <= now) return EventStatus.Running;
            return EventStatus.Planned;
        }
    }
}
