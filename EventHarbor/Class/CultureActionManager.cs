using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHarbor.Class
{
    class CultureActionManager
    {
       
        public ObservableCollection<CultureAction> CultureActions = new ObservableCollection<CultureAction>();
        private ObservableCollection<CultureAction> cultureActionsDbCollection;
        public int LastId { get; private set; }

    public CultureActionManager() 
        {
           
            
        }


        /// <summary>
        /// Retrieves all culture actions from database to  ObesravableCollection
        /// </summary>
        /// <param name="localCultureAction">local instance of CultureAction Manager</param>
        /// <param name="ownerId">Id Logged user fo Db select based on ID</param>
        /// <returns></returns>
        //OwnerId = Logged user Id wait for implementation
        public bool GetAllCultureActionsFromDb(CultureActionManager localCultureAction, int ownerId)
        {
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                cultureActionsDbCollection = new ObservableCollection<CultureAction>(context.CultureActionsDatabase.Where(x => x.OwnerId == ownerId).ToList());
                IEnumerable <CultureAction> union = cultureActionsDbCollection.Union(localCultureAction.CultureActions);

                foreach (CultureAction cultureAction in union)
                {
                    CultureActions.Add(cultureAction);     
                }
                
                LastId = CultureActions.Last().CultureActionId;
                LastId++;

                return true;
            }

        }

       

        //testíing function for inster to db, in future probably will be removed
        public bool AddCultureActionToDb(int ownerId)
        {
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                CultureAction cultureActionTest = new CultureAction("neco", new DateOnly(1991, 1, 4), new DateOnly(1991, 3, 4), 15, 5, 5, CultureActionType.Default, ExhibitionType.Default, 15200, Organiser.Museum, "Aby tady taky něco bylo", ownerId);
                context.CultureActionsDatabase.Add(cultureActionTest);
                context.SaveChanges();
                return true;
            }
        }
    }
}
