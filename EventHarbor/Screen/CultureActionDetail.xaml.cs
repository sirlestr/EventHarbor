using EventHarbor.Class;
using EventHarbor.Screen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
       // CultureActionManager cultureActionManager = MainWindow.cultureActionManager;
        int UserId;
        int LastId;


        public CultureActionDetail(UserManager manager)
        {
            //Initialize
            InitializeComponent();
            // for user ID and name of logged user
            userManager = manager;
            //cultureActionManager = actionManager;
            LastId = cultureActionManager.GetLasIdFromDb();
            LastIdTextBlock.Text = LastId.ToString();

            //assing user data to variables for display in view
             UserId = userManager.LoggedUserId;
            OwnerIDTextBlock.Text = UserId.ToString();
            LoggedUserNameTextBlock.Text =   userManager.LoggedUserName;
            
            
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

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string actionName = CultureActionNameTextBox.Text;
            DateOnly? start = StartDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value) : null;
            DateOnly? end = EndDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value): null;
            int childern = NumberOfChildrenTextBox.Text != null ? int.Parse(NumberOfChildrenTextBox.Text.Trim()) : 0;
            int adult = NumberOfAdultsTextBox.Text != null ? int.Parse(NumberOfAdultsTextBox.Text.Trim()) : 0;
            int senior = NumberOfSeniorsTextBox.Text != null ? int.Parse(NumberOfSeniorsTextBox.Text.Trim()) : 0;
            CultureActionType actionType = (CultureActionType)CultureActionTypeComboBox.SelectedIndex;
            ExhibitionType exhibitionType = (ExhibitionType)CultureExhibitionType.SelectedIndex;
            Organiser organiser = (Organiser)OrganiseComboBox.SelectedIndex;
            int ticketPrice = int.Parse(TicketPriceTextBox.Text.Trim());
            string notes = StringRichTextBox(NotesRichTextBox);
            bool isFree = IsFreeCheckBox.IsChecked.HasValue ? IsFreeCheckBox.IsChecked.Value : false;


            cultureActionManager.AddCultureAction(actionName,start,end,childern,adult,senior,actionType,exhibitionType,ticketPrice,organiser,notes,isFree,UserId);
            



            this.Close();


        }

        private string StringRichTextBox(RichTextBox richText)
        {
            TextRange textRange = new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd);
            return textRange.Text;
        }
    }
}
