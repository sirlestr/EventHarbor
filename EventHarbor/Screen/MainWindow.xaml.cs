using EventHarbor.Class;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EventHarbor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserManager userManager;
        private CultureActionManager cultureActionManager = new CultureActionManager();
        private int UserId;
        private string UserName;

        public MainWindow(UserManager manager)

        {
            InitializeComponent();
            // for user ID and name of logged user
            userManager = manager;




            //assing user data to variables for display in view
            UserId = userManager.LoggedUserId;
            UserName = userManager.LoggedUserName;
            LoggedUserNameTextBlock.Text = userManager.LoggedUserName;
            CultureActionDataGrid.ItemsSource = cultureActionManager.CultureActions;
            //load all culture actions from db to DataGrid
            if (cultureActionManager.GetAllCultureActionsFromDb(cultureActionManager, UserId))
            {
                MessageBox.Show("Načteno");
                CultureActionDataGrid.Items.Refresh();
            }


            
           

            //binding collection to datagrid



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

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            // cultureActionManager.AddCultureActionToDb(UserId);
            cultureActionManager.GetAllCultureActionsFromDb(cultureActionManager, UserId);

        }
    }
}