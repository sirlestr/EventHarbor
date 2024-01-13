using System.ComponentModel.DataAnnotations;

namespace EventHarbor.Class
{
    public enum CultureActionType { Default, Theatre, CastleEvent, OpeningCeremony, Fair, CastleTour }
    public enum ExhibitionType { Default, Hystorical, Technical, Haptic, Creative }
    public enum Organiser { Museum,School,City}
    class CultureAction
    {
        [Key]
        public int CultureActionId { get; set; }
        public string CultureActionName { get; set; }
        public DateOnly ActionStartDate { get; set; }
        public DateOnly ActionEndDate { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfSeniors { get; set; }
        public CultureActionType CultureActionType { get; set; } = CultureActionType.Default;
        public ExhibitionType ExhibitionType { get; set; } = ExhibitionType.Default;
        public double FinalPrice { get; set; }
        public Organiser Organiser { get; set; } = Organiser.Museum;
        public string CultureActionNotes { get; set; }

        public CultureAction()
        {
        }






    }
}
