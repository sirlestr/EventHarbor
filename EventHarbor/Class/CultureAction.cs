using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHarbor.Class
{

    public enum CultureActionType
    {
        [Description("Nezvoleno")] Default,
        [Description("Divadelní představení")] Theatre,
        [Description("Zámeká akce")] CastleEvent,
        [Description("Vernisáž")] OpeningCeremony,
        [Description("Trh")] Fair,
        [Description("Porhlídka Zámku")] CastleTour
    }

    public enum ExhibitionType
    {
        [Description("Všeobecné")] Default,
        [Description("Hystorická")] Hystorical,
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
    class CultureAction
    {
        [Key]
        public int CultureActionId { get; set; }
        public string CultureActionName { get; set; }
        public DateOnly? ActionStartDate { get; set; }
        public DateOnly? ActionEndDate { get; set; }
        public int NumberOfChildren { get; set; } = 0;
        public int NumberOfAdults { get; set; } = 0;
        public int NumberOfSeniors { get; set; } = 0;
        public CultureActionType CultureActionType { get; set; } = CultureActionType.Default;
        public ExhibitionType ExhibitionType { get; set; } = ExhibitionType.Default;
        public float TicketPrice { get; set; }
        public Organiser Organiser { get; set; } = Organiser.Museum;
        public string CultureActionNotes { get; set; }
        public int OwnerId { get; set; }
        public bool IsFree { get; set; }

        [NotMapped]
        private int LastId
        {
            get
            {
                int lastId = 0;
                return lastId;
            }
            set { GetLastIdFromDb(); }

        }

        //empty constructor for DataContext
        public CultureAction() { }


        /// <summary>
        /// Constructor for CultureAction
        /// </summary>
        /// <param name="actionName">Culture action name</param>
        /// <param name="startDate">Start of the event</param>
        /// <param name="endDate">End of the event</param>
        /// <param name="numberOfChildern">Number of young participant</param>
        /// <param name="numberOfAdult">Number of adult participant</param>
        /// <param name="numberOfSenior">Number of seniors participant </param>
        /// <param name="cultureActionType">Type of Action</param>
        /// <param name="exhibitionType">Type of exhibition</param>
        /// <param name="ticketPrice">ticker price</param>
        /// <param name="oraganiser">Who organise this event</param>
        /// <param name="notes">notes</param>
        /// <param name="owner">Id logged user who create this record</param>
        public CultureAction(string actionName, DateOnly? startDate, DateOnly? endDate,
                             int numberOfChildern, int numberOfAdult, int numberOfSenior,
                             CultureActionType cultureActionType, ExhibitionType exhibitionType,
                             float ticketPrice, Organiser oraganiser, string notes,bool isFree, int owner)
        {
            
            CultureActionType = cultureActionType;
            CultureActionName = actionName;
            ActionStartDate = startDate;
            ActionEndDate = endDate;
            NumberOfChildren = numberOfChildern;
            NumberOfAdults = numberOfAdult;
            NumberOfSeniors = numberOfSenior;
            ExhibitionType = exhibitionType;
            TicketPrice = ticketPrice;
            Organiser = oraganiser;
            CultureActionNotes = notes;
            IsFree = isFree;
            OwnerId = owner;


        }







        //WIP - Get last id from database , probably will be removed
        private void GetLastIdFromDb()
        {
            using (var context = new DatabaseContextManager())
            {
                var lastRecord = context.CultureActionsDatabase.OrderByDescending(x => x.CultureActionId).LastOrDefault();
                if (lastRecord != null)
                {
                    LastId = lastRecord.CultureActionId;
                }
                else { LastId = 0; }
            }
        }








    }
}
