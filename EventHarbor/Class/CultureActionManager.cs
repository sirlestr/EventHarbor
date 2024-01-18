using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EventHarbor.Class
{
    internal class CultureActionManager
    {

        
       // internal ObservableCollection<CultureAction> CultureActions = new ObservableCollection<CultureAction>();
        private ObservableCollection<CultureAction> cultureActionsDbCollection;
        internal ObservableCollection<CultureAction> LocalCollection;

        public CultureActionManager(ObservableCollection<CultureAction> localCollection)
        {
            LocalCollection = localCollection;
            LocalCollection.CollectionChanged += LocalCollectionCollectionChanged;
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

      // public bool InsertDataToDb(ObservableCollection<CultureAction> item) { }

       

        //testing function for inster to collection, in future probably will be removed
        public bool AddCultureAction(string actionName, DateOnly? startDate, DateOnly? endDate,
                             int numberOfChildern, int numberOfAdult, int numberOfSenior,
                             CultureActionType cultureActionType, ExhibitionType exhibitionType,
                             float ticketPrice, Organiser oraganiser, string notes, bool isFree, int owner)
        {
            /*
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                CultureAction cultureActionTest = new CultureAction(actionName, startDate, endDate,
                                                            numberOfChildern, numberOfAdult, numberOfSenior,
                                                            cultureActionType, exhibitionType, ticketPrice,
                                                            oraganiser, notes, isFree, owner);
                context.CultureActionsDatabase.Add(cultureActionTest);
                context.SaveChanges();
                return true;
            }
            */
            
            CultureAction cultureAction = new CultureAction(actionName,startDate,endDate,
                                                            numberOfChildern,numberOfAdult,numberOfSenior,
                                                            cultureActionType,exhibitionType, ticketPrice,
                                                            oraganiser,notes,isFree,owner);

            
            return true;

        }

        public void AddAction(CultureAction cultureAction, ObservableCollection<CultureAction> localAction)
        {
            localAction.Add(cultureAction);
        }

        private void LocalCollectionCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                MessageBox.Show("Add");
            }
        }
    }
}
