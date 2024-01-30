using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
                             int numberOfChildern, int numberOfAdult, int numberOfSenior,
                             CultureActionType cultureActionType, ExhibitionType exhibitionType,
                             float ticketPrice, Organiser oraganiser, string notes, bool isFree, int owner)
        {

            CultureAction cultureAction = new CultureAction(actionName, startDate, endDate,
                                                            numberOfChildern, numberOfAdult, numberOfSenior,
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

        public void EditAction(CultureAction selectedAction, CultureAction editedAction, ObservableCollection<CultureAction> localColection )
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
                actionToModify.CultureActionType = editedAction.CultureActionType;
                actionToModify.ExhibitionType = editedAction.ExhibitionType;
                actionToModify.TicketPrice = editedAction.TicketPrice;
                actionToModify.Organiser = editedAction.Organiser;
                actionToModify.CultureActionNotes = editedAction.CultureActionNotes;
                actionToModify.IsFree = editedAction.IsFree;
                
                //for development purpose only; will be removed in future
                MessageBox.Show("Edited");
            }

        }

        public void RemoveItemFromCollection(ObservableCollection<CultureAction> localAction, CultureAction cultureAction)
        {
            CultureAction actionToModify = localAction.FirstOrDefault(x => x.CultureActionId == cultureAction.CultureActionId);
            if (actionToModify != null)
            {
                localAction.Remove(actionToModify);
                //for development purpose only; will be removed in future
                MessageBox.Show("Removed");
            }
        }


        /*
        private bool MergeDataToDb()
        {

            using (DatabaseContextManager context = new())
            {

                cultureActionsDbCollection = new ObservableCollection<CultureAction>(context.CultureActionsDatabase.Where(x => x.OwnerId == OwnerId).ToList());
                if (cultureActionsDbCollection.Count < LocalCollection.Count)
                {
                    foreach (CultureAction item in LocalCollection)
                    {
                        if (!cultureActionsDbCollection.Contains(item))
                        {
                            context.CultureActionsDatabase.Add(item);
                        }

                    }

                    context.SaveChanges();
                    return true;
                }
                /*
                else if (cultureActionsDbCollection.Count > LocalCollection.Count)
                {
                    foreach (CultureAction item in cultureActionsDbCollection)
                    {
                        if (!LocalCollection.Contains(item))
                        {
                            cultureActionsDbCollection.Remove(item);
                        }
                    }
                    context.SaveChanges();
                    return true;
                }
                

            }
            return false;
        }
        */
        public void ForceMerge(ObservableCollection<CultureAction> localCollection)
        {
            DbManager.MergeDataWithDb(localCollection, OwnerId);
        }

        private void LocalCollectionCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (DbManager.MergeDataWithDb(LocalCollection,OwnerId))
                {
                    MessageBox.Show("Added to Db");
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (DbManager.MergeDataWithDb(LocalCollection, OwnerId))
                {
                    MessageBox.Show("Removed from Db");
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (DbManager.MergeDataWithDb(LocalCollection, OwnerId))
                {
                    MessageBox.Show("Replaced in Db");
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (DbManager.MergeDataWithDb(LocalCollection, OwnerId))
                {
                    MessageBox.Show("Moved in Db");
                }
            }

        }
    }
}
