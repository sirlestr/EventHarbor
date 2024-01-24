using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace EventHarbor.Class
{
    internal class CultureActionManager
    {

        
       
        private ObservableCollection<CultureAction> cultureActionsDbCollection;
        internal ObservableCollection<CultureAction> LocalCollection;
        private int OwnerId { get; set; }

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

      // public bool InsertDataToDb(ObservableCollection<CultureAction> item) { }

       

        //testing function for inster to collection, in future probably will be removed
        public bool AddCultureAction(string actionName, DateOnly? startDate, DateOnly? endDate,
                             int numberOfChildern, int numberOfAdult, int numberOfSenior,
                             CultureActionType cultureActionType, ExhibitionType exhibitionType,
                             float ticketPrice, Organiser oraganiser, string notes, bool isFree, int owner)
        {
           
            
            CultureAction cultureAction = new CultureAction(actionName,startDate,endDate,
                                                            numberOfChildern,numberOfAdult,numberOfSenior,
                                                            cultureActionType,exhibitionType, ticketPrice,
                                                            oraganiser,notes,isFree,owner);

           LocalCollection.Add(cultureAction);




            return true;

        }

        public void AddAction(CultureAction cultureAction, ObservableCollection<CultureAction> localAction)
        {
            localAction.Add(cultureAction);
        }

        private bool MergeDataToDb()
        {

            using(DatabaseContextManager context = new())
            {
                
                cultureActionsDbCollection = new ObservableCollection<CultureAction>(context.CultureActionsDatabase.Where(x => x.OwnerId == OwnerId).ToList());
                if (cultureActionsDbCollection.Count < LocalCollection.Count)
                {
                    foreach (CultureAction item in LocalCollection)
                    {
                        if (!cultureActionsDbCollection.Contains(item))
                        {
                            
                            context.CultureActionsDatabase.Add(item);
                            //cultureActionsDbCollection.Add(item);
                            
                        }

                    }
                    
                    //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT  CultureActionsDatabase ON");
                    context.SaveChanges();
                    
                    return true;
                }


            }
                return false;
        }

        private void LocalCollectionCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                if (MergeDataToDb())
                {
                    MessageBox.Show("Added");
                }
            }
        }
    }
}
