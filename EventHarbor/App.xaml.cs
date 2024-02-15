using EventHarbor.Class;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Windows;

namespace EventHarbor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
           DatabaseContextManager manager = new DatabaseContextManager();
            if (manager.Database.EnsureCreated())
            {
                Debug.WriteLine("****************************");
                Debug.WriteLine("Created");
                Debug.WriteLine("****************************");
            }
            else
            {
                Debug.WriteLine("****************************");
                Debug.WriteLine("Db Exist");
                Debug.WriteLine("****************************");
            }

        }
    }

}
