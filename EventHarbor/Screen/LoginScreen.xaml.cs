using EventHarbor.Class;
using EventHarbor.Screen;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EventHarbor
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            DatabaseContextManager dbManager = new DatabaseContextManager();
            
            InitializeComponent();

            //temporary for development
            /*
            if (dbManager.Database.EnsureDeleted())
            {
                MessageBox.Show("smazano");
            }
            if (dbManager.Database.EnsureCreated())
            {
                MessageBox.Show("Vytvořeno");
            }
            */
            
            

        }



        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
        //switch sreen to forgot window
        private void ForgotUri_Click(object sender, RoutedEventArgs e)
        {
            ForgotScreen screen = new ForgotScreen();
            this.Visibility = Visibility.Collapsed;
            screen.ShowDialog();
            this.Visibility = Visibility.Visible;
        }
        //switch screen to register window
        private void RegisterUri_Click(object sender, RoutedEventArgs e)
        {
            RegisterScreen register = new RegisterScreen();
            this.Visibility = Visibility.Collapsed;
            register.ShowDialog();
            this.Visibility = Visibility.Visible;
        }
        //move window function
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as Border;
            var win = Window.GetWindow(move);
            win.DragMove();
        }
        //login function
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {

            UserManager userManager = new UserManager();

            if (UserNameTextBox.Text != null && PwdBox.Password != null)
            {
                try
                {
                    switch (userManager.IsRegistered(UserNameTextBox.Text, PwdBox.Password))
                    {
                        case -1:
                            MessageBox.Show("Uživatel nenalezen");
                            break;
                        case 0:
                            MessageBox.Show("Uživatel nalezen ale heslo je zadané chybně, prosím opakuj.");
                            break;
                        case 1:
                            MessageBox.Show("Přihlášeno");
                            MainWindow mainWindow = new MainWindow(userManager);
                            this.Visibility = Visibility.Collapsed;
                            mainWindow.ShowDialog();
                            this.Visibility = Visibility.Visible;
                            break;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }


                /*
                if (dbManager.Database.EnsureDeleted())
                {
                    MessageBox.Show("smazano");
                }
                if (dbManager.Database.EnsureCreated())
                {
                    MessageBox.Show("Vytvořeno");
                }
                */

            }
        }
    }
}
