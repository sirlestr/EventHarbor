using EventHarbor.Class;
using System.Windows;
using System.Windows.Input;

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
        //move window
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as System.Windows.Controls.Border;
            var win = Window.GetWindow(move);
            win.DragMove();

        }

        //Registration
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            UserManager userManager = new UserManager();

            if (UserNameTextBox.Text != null && PwdBox1.Password == PwdBox2.Password)
            {
                try
                {
                    if (userManager.AddUser(UserNameTextBox.Text, PwdBox1.Password))
                    {
                        MessageBox.Show("Registrace proběhla v pořádku");
                        this.Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Nepovedlo se vytvořit uživatele");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Zadejte prosím uživatelské jméno a heslo");
            }
        }


    }
}
