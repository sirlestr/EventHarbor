using EventHarbor.Class;
using EventHarbor.Screen;
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

namespace EventHarbor
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

       

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            
        }


        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ForgotUri_Click(object sender, RoutedEventArgs e)
        {
            ForgotScreen screen = new ForgotScreen();
            this.Visibility = Visibility.Collapsed;
            screen.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void RegisterUri_Click(object sender, RoutedEventArgs e)
        {
            RegisterScreen register = new RegisterScreen();
            this.Visibility = Visibility.Collapsed;
            register.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as Border;
            var win = Window.GetWindow(move);
            win.DragMove();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {

            using (DatabaseContextManager dbManager = new DatabaseContextManager())
            {
                if (dbManager.Database.EnsureDeleted())
                {
                    MessageBox.Show("smazano");
                }
                if (dbManager.Database.EnsureCreated())
                {
                    MessageBox.Show("Vytvořeno");
                }
            }
        }
    }
}
