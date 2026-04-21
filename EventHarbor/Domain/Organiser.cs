using System.ComponentModel;

namespace EventHarbor.Domain;

public enum Organiser
{
    [Description("Muzeum")] Museum,
    [Description("Město")] City,
    [Description("Ostatní")] Other,
}
