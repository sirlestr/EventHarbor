using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EventHarbor.Class
{
    internal class BasicStatistic
    {

        private  ObservableCollection<CultureAction> StatisticCollection;

        public BasicStatistic(ObservableCollection<CultureAction> statisticCollection)
        {
          StatisticCollection = statisticCollection;
        }



        public int TotalNumberOfParticipants()
        {
            int total = 0;
            foreach (CultureAction item in StatisticCollection)
            {
                total += item.NumberOfAdults + item.NumberOfChildren + item.NumberOfSeniors + item.NumberOfDisabled;
            }
            return total;
        }

        public int TotalNumberOfChildren()
        {
            int total = 0;
            foreach (CultureAction item in StatisticCollection)
            {
                total += item.NumberOfChildren;
            }
            return total;
        }

        public int TotalNumberOfAdults()
        {
            int total = 0;
            foreach (CultureAction item in StatisticCollection)
            {
                total += item.NumberOfAdults;
            }
            return total;
        }

        public int TotalNumberOfSeniors()
        {
            int total = 0;
            foreach (CultureAction item in StatisticCollection)
            {
                total += item.NumberOfSeniors;
            }
            return total;
        }

        public int TotalNumberOfDisabled()
        {
            int total = 0;
            foreach (CultureAction item in StatisticCollection)
            {
                total += item.NumberOfDisabled;
            }
            return total;
        }

        public float TotalActionPrice()
        {
            float total = 0;
            foreach (CultureAction item in StatisticCollection)
            {
                total += item.ActionPrice;
            }
            return total;
        }

       

       public string MostProfitAction()
        {
            float max = 0;
            string name = "";
            foreach (CultureAction item in StatisticCollection)
            {
                if (item.ActionPrice > max)
                {
                    max = item.ActionPrice;
                    name = item.CultureActionName;
                }
            }
            return name;
            
        }


        public string MostVisitedAction()
        {
            int max = 0;
            string name = "";
            foreach (CultureAction item in StatisticCollection)
            {
                if (item.NumberOfAdults + item.NumberOfChildren + item.NumberOfSeniors + item.NumberOfDisabled > max)
                {
                    max = item.NumberOfAdults + item.NumberOfChildren + item.NumberOfSeniors + item.NumberOfDisabled;
                    name = item.CultureActionName;
                }
            }
            return name;
        }




    }
}
