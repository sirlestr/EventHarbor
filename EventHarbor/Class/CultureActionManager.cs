using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;


namespace EventHarbor.Class
{
    internal class CultureActionManager
    {



        private ObservableCollection<CultureAction> cultureActionsDbCollection;
        internal ObservableCollection<CultureAction> LocalCollection;
        private int OwnerId { get; set; }
        private DatabaseContextManager DbManager = new DatabaseContextManager();

        public CultureActionManager(ObservableCollection<CultureAction> localCollection, int owner)
        {
            LocalCollection = localCollection;
            LocalCollection.CollectionChanged += LocalCollectionCollectionChanged;
            OwnerId = owner;
        }
        public CultureActionManager()
        {

        }


        /// <summary>
        /// Retrieves all culture actions from database to  ObesravableCollection
        /// </summary>
        /// <param name="localCultureAction">local instance of CultureAction Manager</param>
        /// <param name="ownerId">Id Logged user for Db select </param>
        /// <returns></returns>

        public bool GetCultureActionsFromDb(ObservableCollection<CultureAction> localCollection, int ownerId)
        {

            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                cultureActionsDbCollection = new ObservableCollection<CultureAction>(context.CultureActionsDatabase.Where(x => x.OwnerId == ownerId).ToList());

                foreach (CultureAction item in cultureActionsDbCollection)
                {
                    if (!localCollection.Contains(item))
                    {
                        localCollection.Add(item);
                    }

                }

                return true;
            }

        }

        //testing function for inster to collection, in future probably will be removed
        public bool AddCultureAction(string actionName, DateOnly? startDate, DateOnly? endDate,
                             int numberOfChildern, int numberOfAdult, int numberOfSenior, int numberOfDisabled,
                             CultureActionType cultureActionType, ExhibitionType exhibitionType,
                             float ticketPrice, Organiser oraganiser, string notes, bool isFree, int owner)
        {

            CultureAction cultureAction = new CultureAction(actionName, startDate, endDate,
                                                            numberOfChildern, numberOfAdult, numberOfSenior, numberOfDisabled,
                                                            cultureActionType, exhibitionType, ticketPrice,
                                                            oraganiser, notes, isFree, owner);

            LocalCollection.Add(cultureAction);

            return true;

        }
        /// <summary>
        /// Add item to collection
        /// </summary>
        /// <param name="cultureAction">Item for adding to the collection</param>
        /// <param name="localAction">Local collection</param>
        public void AddAction(CultureAction cultureAction, ObservableCollection<CultureAction> localAction)
        {
            localAction.Add(cultureAction);
        }

        public void EditAction(CultureAction selectedAction, CultureAction editedAction, ObservableCollection<CultureAction> localColection)
        {
            CultureAction actionToModify = localColection.FirstOrDefault(x => x.CultureActionId == selectedAction.CultureActionId);
            if (actionToModify != null)
            {
                actionToModify.CultureActionName = editedAction.CultureActionName;
                actionToModify.ActionStartDate = editedAction.ActionStartDate;
                actionToModify.ActionEndDate = editedAction.ActionEndDate;
                actionToModify.NumberOfChildren = editedAction.NumberOfChildren;
                actionToModify.NumberOfAdults = editedAction.NumberOfAdults;
                actionToModify.NumberOfSeniors = editedAction.NumberOfSeniors;
                actionToModify.NumberOfDisabled = editedAction.NumberOfDisabled;
                actionToModify.CultureActionType = editedAction.CultureActionType;
                actionToModify.ExhibitionType = editedAction.ExhibitionType;
                actionToModify.ActionPrice = editedAction.ActionPrice;
                actionToModify.Organiser = editedAction.Organiser;
                actionToModify.CultureActionNotes = editedAction.CultureActionNotes;
                actionToModify.IsFree = editedAction.IsFree;

                //for development purpose only; will be removed in future
                Debug.WriteLine("Edited");
            }

        }

        public void RemoveItemFromCollection(ObservableCollection<CultureAction> localAction, CultureAction cultureAction)
        {
            CultureAction actionToModify = localAction.FirstOrDefault(x => x.CultureActionId == cultureAction.CultureActionId);

            if (actionToModify != null)
            {
                localAction.Remove(actionToModify);
            }
            else
            {
                Debug.WriteLine("no action to modify");
            }

        }




        public void ForceMerge(ObservableCollection<CultureAction> localCollection, int userId)
        {
            DbManager.MergeDataWithDb(localCollection, userId);
        }
       
        private void LocalCollectionCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var action = e.Action;
            var mergeSuccessful = DbManager.MergeDataWithDb(LocalCollection, OwnerId);

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (mergeSuccessful) MessageBox.Show("Added to Db");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (mergeSuccessful) MessageBox.Show("Removed from Db");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (mergeSuccessful) MessageBox.Show("Replaced in Db");
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (mergeSuccessful) MessageBox.Show("Moved in Db");
                    break;
            }
        }
    }
}
