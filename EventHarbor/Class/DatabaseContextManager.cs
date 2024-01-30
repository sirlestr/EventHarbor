using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Common;
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
                ObservableCollection<CultureAction>  DbCollection = new ObservableCollection<CultureAction>(context.CultureActionsDatabase.Where(x => x.OwnerId == ownerId).ToList());

                if (DbCollection != localCollection && localCollection.Count > DbCollection.Count)
                {
                    foreach (CultureAction localItem in localCollection)
                    {
                        CultureAction dbItem = DbCollection.FirstOrDefault(x => x.CultureActionId == localItem.CultureActionId);
                        if (dbItem != null)
                        {
                            if(localItem != dbItem)
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
                        else
                        {
                            context.CultureActionsDatabase.Add(localItem);
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Data merged");
                }
                else if (DbCollection != localCollection && DbCollection.Count > localCollection.Count)
                {
                    foreach (CultureAction item in DbCollection)
                    {
                        if (!localCollection.Contains(item))
                        {
                            context.CultureActionsDatabase.Remove(item);
                        }
                    }
                    context.SaveChanges();
                    MessageBox.Show("Data removed");

                }
               




                /*
                if (DbCollection.Count < localCollection.Count)
                {
                    foreach (CultureAction item in localCollection)
                    {
                        if (!DbCollection.Contains(item))
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
                */

            }
            return false;
        }

        


    }
}
