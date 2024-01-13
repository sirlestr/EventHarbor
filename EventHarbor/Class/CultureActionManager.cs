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

        public CultureActionManager() 
        { 
        }
    }
}
