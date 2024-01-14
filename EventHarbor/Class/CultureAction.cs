﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHarbor.Class
{
    
    public enum CultureActionType { Default, Theatre, CastleEvent, OpeningCeremony, Fair, CastleTour }
    public enum ExhibitionType { Default, Hystorical, Technical, Haptic, Creative }
    public enum Organiser { Museum, School, City }
    class CultureAction
    {
        [Key]
        public int CultureActionId { get; set; }
        public string CultureActionName { get; set; }
        public DateOnly ActionStartDate { get; set; }
        public DateOnly ActionEndDate { get; set; }
        public int NumberOfChildren { get; set; } = 0;
        public int NumberOfAdults { get; set; } = 0;
        public int NumberOfSeniors { get; set; } = 0;
        public CultureActionType CultureActionType { get; set; } = CultureActionType.Default;
        public ExhibitionType ExhibitionType { get; set; } = ExhibitionType.Default;
        public float FinalPrice { get; set; }
        public Organiser Organiser { get; set; } = Organiser.Museum;
        public string CultureActionNotes { get; set; }
        public int OwnerId { get; set; }

        [NotMapped]
        private int LastId 
        {
            get {
                int lastId = 0;
                return lastId; }
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
        /// <param name="finalPrice">Finla budget</param>
        /// <param name="oraganiser">Who organise this event</param>
        /// <param name="notes">notes</param>
        /// <param name="owner">Id logged user who create this record</param>
        public CultureAction(string actionName, DateOnly startDate, DateOnly endDate,
                             int numberOfChildern, int numberOfAdult, int numberOfSenior,
                             CultureActionType cultureActionType, ExhibitionType exhibitionType,
                             float finalPrice, Organiser oraganiser, string notes, int owner)
        {
            //CultureActionId = LastId + 1;
            CultureActionType = cultureActionType;
            CultureActionName = actionName;
            ActionStartDate = startDate;
            ActionEndDate = endDate;
            NumberOfChildren = numberOfChildern;
            NumberOfAdults = numberOfAdult;
            NumberOfSeniors = numberOfSenior;
            ExhibitionType = exhibitionType;
            FinalPrice = finalPrice;
            Organiser = oraganiser;
            CultureActionNotes = notes;
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
