using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EventHarbor.Class
{
    internal class DatabaseContextManager : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CultureAction> CultureActionsDatabase { get; set; }

        
        


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=EventHarbor;Trusted_Connection=True;");
            
        }

        internal bool MergeDataWithDb(ObservableCollection<CultureAction> localCollection, int ownerId)
        {

            using (DatabaseContextManager context = new())
            {
                ObservableCollection<CultureAction>  dbCollection = new ObservableCollection<CultureAction>(context.CultureActionsDatabase.Where(x => x.OwnerId == ownerId).ToList());
                if(!dbCollection.Equals(localCollection) && (dbCollection.Count == localCollection.Count))
                {
                    foreach (CultureAction localItem in localCollection)
                    {
                        CultureAction dbItem = dbCollection.FirstOrDefault(x => x.CultureActionId == localItem.CultureActionId);
                        if (dbItem != null)
                        {
                            if (localItem != dbItem)
                            {
                                dbItem.CultureActionName = localItem.CultureActionName;
                                dbItem.ActionStartDate = localItem.ActionStartDate;
                                dbItem.ActionEndDate = localItem.ActionEndDate;
                                dbItem.NumberOfChildren = localItem.NumberOfChildren;
                                dbItem.NumberOfAdults = localItem.NumberOfAdults;
                                dbItem.NumberOfSeniors = localItem.NumberOfSeniors;
                                dbItem.CultureActionType = localItem.CultureActionType;
                                dbItem.ExhibitionType = localItem.ExhibitionType;
                                dbItem.TicketPrice = localItem.TicketPrice;
                                dbItem.Organiser = localItem.Organiser;
                                dbItem.CultureActionNotes = localItem.CultureActionNotes;
                                dbItem.IsFree = localItem.IsFree;

                            }
                        }                        
                    }
                    context.SaveChanges();
                    Debug.WriteLine("Data updated");
                }
                if (!dbCollection.Equals(localCollection) && (dbCollection.Count< localCollection.Count) )
                {
                    foreach (CultureAction localItem in localCollection)
                    {
                        CultureAction dbItem = dbCollection.FirstOrDefault(x => x.CultureActionId == localItem.CultureActionId);
                        if (dbItem != null)
                        {
                            context.CultureActionsDatabase.Add(localItem);
                        }
                       
                    }

                    context.SaveChanges();
                    Debug.WriteLine("Data Added");
                }
                
                else if (!dbCollection.Equals(localCollection) && dbCollection.Count > localCollection.Count)
                {
                    foreach (CultureAction item in dbCollection)
                    {
                        if (!localCollection.Contains(item))
                        {
                            context.CultureActionsDatabase.Remove(item);
                        }
                    }
                    context.SaveChanges();
                    Debug.WriteLine("Data removed");

                }
                
            }
            return false;
        }

       
        


    }
}
