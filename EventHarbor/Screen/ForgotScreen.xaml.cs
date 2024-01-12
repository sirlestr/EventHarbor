using EventHarbor.Class;
using System.Windows;
using System.Windows.Input;

namespace EventHarbor.Screen
{
    /// <summary>
    /// Interaction logic for ForgotScreen.xaml
    /// </summary>
    public partial class ForgotScreen : Window
    {
        public ForgotScreen()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as System.Windows.Controls.Border;
            var win = Window.GetWindow(move);
            win.DragMove();

        }


        //Reset user password
        private void ResetPwdBtn_Click(object sender, RoutedEventArgs e)
        {
            UserManager userManager = new UserManager();

            if (UserNameTextBox.Text != null && PwdBox.Password != null)
            {
                try
                {
                    if (userManager.ResetPassword(UserNameTextBox.Text, PwdBox.Password))
                    {
                        MessageBox.Show("Heslo bylo resetováno");
                        this.Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Nepovedlo se resetovat heslo, uživatel nenalezen");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }



        }
    }
}
