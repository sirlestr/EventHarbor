using EventHarbor.Class;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EventHarbor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserManager userManager;
        private CultureActionManager cultureActionManager = new CultureActionManager();

        public MainWindow(UserManager manager)
        
        {
             userManager = manager;

            InitializeComponent();
            CultureActionDataGrid.ItemsSource = cultureActionManager.CultureActions;

            int UserId = userManager.LoggedUserId;
            string UserName = userManager.LoggedUserName;
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
    }
}