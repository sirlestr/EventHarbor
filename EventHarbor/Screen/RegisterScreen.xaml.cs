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
    /// Interaction logic for RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : Window
    {
        public RegisterScreen()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as System.Windows.Controls.Border;
            var win = Window.GetWindow(move);
            win.DragMove();

        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            UserManager userManager = new UserManager();
           
                if (UserNameTextBox.Text != null && PwdBox1.Password == PwdBox2.Password)
                {
                    if(userManager.AddUser(UserNameTextBox.Text, PwdBox1.Password))
                    {
                        MessageBox.Show("Registrace proběhla");
                    }
                    else
                    {
                        {
                            MessageBox.Show("Nepovedlo se vytvořit uživatele");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Zadejte prosím uživatelské jméno a heslo");
                }
          

        }
    }
}
