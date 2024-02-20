using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Controls;

namespace EventHarbor.Class
{

    public enum CultureActionType
    {
        [Description("Nezvoleno")] Default,
        [Description("Divadelní představení")] Theatre,
        [Description("Zámecká akce")] CastleEvent,
        [Description("Vernisáž")] OpeningCeremony,
        [Description("Trh")] Fair,
        [Description("Prohlídka Zámku")] CastleTour
    }

    public enum ExhibitionType
    {
        [Description("Všeobecné")] Default,
        [Description("Historická")] Historical,
        [Description("Technická")] Technical,
        [Description("Interaktivní")] Interactive,       
        [Description("Umělecká")] Artistic,
        [Description("Přírodovědná")] Nature

    }
    public enum Organiser 
    { 
        [Description("Muzeum")] Museum,
        [Description("Ostatní")] Other,
        [Description("Město")] City }
    internal class CultureAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int CultureActionId { get; set; }
        public string CultureActionName { get; set; }
        public DateOnly? ActionStartDate { get; set; }
        public DateOnly? ActionEndDate { get; set; }
        public int NumberOfChildren { get; set; } = 0;
        public int NumberOfAdults { get; set; } = 0;
        public int NumberOfSeniors { get; set; } = 0;
        public int NumberOfDisabled { get; set; } = 0;
        public CultureActionType CultureActionType { get; set; } = CultureActionType.Default;
        public ExhibitionType ExhibitionType { get; set; } = ExhibitionType.Default;
        public float ActionPrice { get; set; }
        public Organiser Organiser { get; set; } = Organiser.Museum;
        public string CultureActionNotes { get; set; }
        public int OwnerId { get; set; }
        public bool IsFree { get; set; }

        [NotMapped]
        private int LastId { get; set; }
    
        

        //empty constructor for DataContext
        internal CultureAction() { }


        /// <summary>
        /// Constructor for CultureAction
        /// </summary>
        /// <param name="actionName">Culture action name</param>
        /// <param name="startDate">Start of the event</param>
        /// <param name="endDate">End of the event</param>
        /// <param name="numberOfChildern">Number of young participant</param>
        /// <param name="numberOfAdult">Number of adult participant</param>
        /// <param name="numberOfSenior">Number of seniors participant </param>
        /// <param name="numberOfDisabled">Number of disabled participant </param>
        /// <param name="cultureActionType">Type of Action</param>
        /// <param name="exhibitionType">Type of exhibition</param>
        /// <param name="actionPrice">action price</param>
        /// <param name="oraganiser">Who organizes this event</param>
        /// <param name="notes">notes</param>
        /// <param name="owner">Id logged user who create this record</param>
        internal CultureAction( string actionName, DateOnly? startDate, DateOnly? endDate,
                             int numberOfChildern, int numberOfAdult, int numberOfSenior,int numberOfDisabled,
                             CultureActionType cultureActionType, ExhibitionType exhibitionType,
                             float actionPrice, Organiser oraganiser, string notes,bool isFree, int owner)
        {
            LastId = GetLastIdFromDb();
            CultureActionId = LastId;
            CultureActionType = cultureActionType;
            CultureActionName = actionName;
            ActionStartDate = startDate;
            ActionEndDate = endDate;
            NumberOfChildren = numberOfChildern;
            NumberOfAdults = numberOfAdult;
            NumberOfSeniors = numberOfSenior;
            NumberOfDisabled = numberOfDisabled;
            ExhibitionType = exhibitionType;
            ActionPrice = actionPrice;
            Organiser = oraganiser;
            CultureActionNotes = notes;
            IsFree = isFree;
            OwnerId = owner;


        }



        //WIP - Get last id from database , probably will be removed
        public int GetLastIdFromDb()
        {
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                if (context.CultureActionsDatabase.Count() > 0)
                {
                    LastId = context.CultureActionsDatabase.Max(x => x.CultureActionId);
                    return LastId + 1;
                }
                return LastId = 0;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return CultureActionId == ((CultureAction)obj).CultureActionId;
        }

        public override int GetHashCode()
        {
            return CultureActionId.GetHashCode();
        }

        public static bool operator ==(CultureAction left, CultureAction right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            // Comparison of all class parameters
            return left.CultureActionId == right.CultureActionId
                && left.CultureActionName == right.CultureActionName
                && left.ActionStartDate == right.ActionStartDate
                && left.ActionEndDate == right.ActionEndDate
                && left.NumberOfChildren == right.NumberOfChildren
                && left.NumberOfAdults == right.NumberOfAdults
                &&left.NumberOfSeniors == right.NumberOfSeniors
                && left.NumberOfDisabled == right.NumberOfDisabled
                && left.CultureActionType == right.CultureActionType
                &&left.ExhibitionType == right.ExhibitionType
                &&left.ActionPrice == right.ActionPrice
                &&left.Organiser == right.Organiser
                &&left.CultureActionNotes == right.CultureActionNotes
                &&left.IsFree == right.IsFree
                ;
        }


        public static bool operator !=(CultureAction left, CultureAction right)
        {
            return !(left == right);
        }






    }
}
