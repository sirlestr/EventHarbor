using EventHarbor.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EventHarbor.Screen
{
    /// <summary>
    /// Interaction logic for CultureActionDetail.xaml
    /// </summary>
   
    
    public partial class CultureActionDetail : Window
    {
        UserManager userManager;
        private CultureActionManager cultureActionManager = new CultureActionManager();
        int UserId;
        int LastId;


        public CultureActionDetail(UserManager manager)
        {
            //Initialize
            InitializeComponent();
            // for user ID and name of logged user
            userManager = manager;
            LastId = cultureActionManager.LastId;

            //assing user data to variables for display in view
            UserId = userManager.LoggedUserId;
            LoggedUserNameTextBlock.Text = userManager.LoggedUserName;
            
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as Border;
            var win = Window.GetWindow(move);
            win.DragMove();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
