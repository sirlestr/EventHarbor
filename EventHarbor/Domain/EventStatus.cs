using System.ComponentModel;

namespace EventHarbor.Domain;

public enum EventStatus
{
    [Description("Plánováno")] Planned,
    [Description("Probíhá")] Running,
    [Description("Ukončeno")] Ended,
}
