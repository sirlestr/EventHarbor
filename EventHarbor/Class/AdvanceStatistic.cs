using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EventHarbor.Class
{
    internal class AdvanceStatistic
    {
        private ObservableCollection<CultureAction> FilteredCollection;
        private ObservableCollection<CultureAction> LocalCollection { get; set; }
        private BasicStatistic basicStatistic;

        public AdvanceStatistic(ObservableCollection<CultureAction> localCollection)
        {
            LocalCollection = localCollection;
        }

        public void CreateFilteredCollection(DateOnly startDate, DateOnly endDate, Organiser organiser, CultureActionType ActionType, ExhibitionType exhibitionType, bool isFree) 
        {

            FilteredCollection = new ObservableCollection<CultureAction>(LocalCollection.Where(x => x.ActionStartDate >= startDate && x.ActionEndDate <= endDate && x.Organiser == organiser && x.CultureActionType == ActionType && x.ExhibitionType == exhibitionType && x.IsFree == isFree).ToList());
            
        }

        public ObservableCollection<CultureAction> CreateFilteredCollectionBasedOnDate(DateOnly startDate, DateOnly endDate)
        {
            return FilteredCollection = new ObservableCollection<CultureAction>(LocalCollection.Where(x => x.ActionStartDate >= startDate && x.ActionEndDate <= endDate));
        }

        public string GenerateStatistic(ObservableCollection<CultureAction> filteredCollection)
        {
            basicStatistic = new BasicStatistic(filteredCollection);
            return $"Celkový počet účastníků: {basicStatistic.TotalNumberOfParticipants()}\nCelkový počet dětí: {basicStatistic.TotalNumberOfChildren()}\nCelkový počet dospělých: {basicStatistic.TotalNumberOfAdults()}\nCelkový počet seniorů: {basicStatistic.TotalNumberOfSeniors()}";

        }




    }
}
