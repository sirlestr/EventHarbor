using EventHarbor.Class;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
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
            string path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EventHarbor");

            DatabaseContextManager manager = new DatabaseContextManager();
            //check if folder exist on startup, if not, then  create
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //check if db exist on startup, if not, then  create
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
