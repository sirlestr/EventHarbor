using System.ComponentModel;

namespace EventHarbor.Domain;

public enum ExhibitionType
{
    [Description("Všeobecné")] General,
    [Description("Umění")] Art,
    [Description("Historie")] History,
    [Description("Klasika")] Classic,
    [Description("Rodinné")] Family,
    [Description("Vzdělávací")] Educational,
}
