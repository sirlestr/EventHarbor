﻿using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EventHarbor.Class
{
    /// <summary>
    /// Db context manager
    /// </summary>
    internal class DatabaseContextManager : DbContext
    {
        /// <summary>
        /// Gets or sets the users DbSet
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the culture actions DbSet
        /// </summary>
        public DbSet<CultureAction> CultureActionsDatabase { get; set; }




        /// <summary>
        /// Configures the database options
        /// </summary>
        /// <param name="optionsBuilder">The options builder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=EventHarbor;Trusted_Connection=True;");
            optionsBuilder.LogTo(text => Debug.WriteLine(text), LogLevel.Information);

        }


        /// <summary>
        /// Merge data with the database
        /// </summary>
        /// <param name="localCollection">The local culture action collection</param>
        /// <param name="ownerId">The owner ID</param>
        /// <returns>True if the merge was successful, otherwise false</returns>
        internal bool MergeDataWithDb(ObservableCollection<CultureAction> localCollection, int ownerId)
        {

            using (DatabaseContextManager context = new())
            {
                ObservableCollection<CultureAction> dbCollection = new ObservableCollection<CultureAction>(context.CultureActionsDatabase.Where(x => x.OwnerId == ownerId).ToList());
                if (!dbCollection.Equals(localCollection) && (dbCollection.Count == localCollection.Count))
                {
                    foreach (CultureAction localItem in localCollection)
                    {
                        CultureAction? dbItem = dbCollection.FirstOrDefault(x => x.CultureActionId == localItem.CultureActionId);
                        if ((dbItem != null) && (localItem != dbItem))
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
                    context.SaveChanges();
                    Debug.WriteLine("Data updated");
                }

                if (!dbCollection.Equals(localCollection) && (dbCollection.Count < localCollection.Count))
                {
                    foreach (CultureAction localItem in localCollection)
                    {
                        CultureAction? dbItem = dbCollection.FirstOrDefault(x => x.CultureActionId == localItem.CultureActionId);

                        if ((dbItem == null) )
                        {
                            context.CultureActionsDatabase.Add(localItem);
                        }

                    }

                    context.SaveChanges();
                    Debug.WriteLine("Data Added");
                }

                if (!dbCollection.Equals(localCollection) && dbCollection.Count > localCollection.Count)
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
