﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EventHarbor.Class
{
    internal class StatisticManager
    {

        public  ObservableCollection<CultureAction> StatisticCollection;

        public StatisticManager(ObservableCollection<CultureAction> statisticCollection)
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

        public int TotalNumberOfParticipants1()
        {
            
            //StatisticCollection.Sum(x => x.NumberOfAdults + x.NumberOfChildren + x.NumberOfSeniors + x.NumberOfDisabled);
            return StatisticCollection.Sum(x => x.NumberOfAdults + x.NumberOfChildren + x.NumberOfSeniors + x.NumberOfDisabled); ;
        }




    }
}
