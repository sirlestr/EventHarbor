using System.ComponentModel;

namespace EventHarbor.Domain;

public enum CultureEventType
{
    [Description("Výstava")] Exhibition,
    [Description("Koncert")] Concert,
    [Description("Divadlo")] Theatre,
    [Description("Přednáška")] Lecture,
    [Description("Workshop")] Workshop,
    [Description("Trh")] Fair,
    [Description("Speciální akce")] SpecialEvent,
}
