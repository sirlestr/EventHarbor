using EventHarbor.Class;
using EventHarbor.Screen;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private ObservableCollection<CultureAction> localCollection = new ObservableCollection<CultureAction>();
        internal ObservableCollection<CultureAction> LocalCollection
        {
            get { return localCollection; }
            set { localCollection = value; }
        }

        internal CultureActionManager cultureActionManager = new CultureActionManager();


        private int UserId;
        private string UserName;


        public MainWindow(UserManager manager)

        {

            // for user ID and name of logged user
            userManager = manager;

            //assing user data to variables for display in view
            UserId = userManager.LoggedUserId;
            UserName = userManager.LoggedUserName;

            InitializeComponent();

            CultureActionDataGrid.ItemsSource = LocalCollection;
            LoggedUserNameTextBlock.Text = userManager.LoggedUserName;

            cultureActionManager.GetCultureActionsFromDb(LocalCollection, UserId);
            CultureActionDataGrid.Items.Refresh();







        }



        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            cultureActionManager.ForceMerge(LocalCollection, UserId);
            Application.Current.Shutdown();


        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as Border;
            var win = Window.GetWindow(move);
            win.DragMove();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {


            CultureActionDetail AddActionWindow = new CultureActionDetail(userManager, LocalCollection, true);
            AddActionWindow.ShowDialog();



        }



        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (cultureActionManager.GetCultureActionsFromDb(LocalCollection, UserId))
            {
                MessageBox.Show("Načteno");
                CultureActionDataGrid.Items.Refresh();
            }

        }

        private void ChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CultureActionDataGrid.SelectedItem != null)
            {
                try
                {
                    CultureAction action = (CultureAction)CultureActionDataGrid.SelectedItem;
                    if (action != null)
                    {
                        CultureActionDetail EditWindow = new CultureActionDetail(userManager, LocalCollection, false);
                        EditWindow.FillFormData(action);
                        EditWindow.ShowDialog();
                    }

                    CultureActionDataGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CultureActionDataGrid.SelectedItem != null)
            {
                cultureActionManager.RemoveItemFromCollection(LocalCollection, (CultureAction)CultureActionDataGrid.SelectedItem);
                // Removed successfully
                Debug.WriteLine("Removed");
            }
            else
            {
                //no item is selected 
                Debug.WriteLine("no item is selected");
            }


        }
    }

}