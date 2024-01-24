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
        private CultureActionManager cultureActionManager;// = new CultureActionManager();
        internal ObservableCollection<CultureAction> LocalAction;
        int UserId;
        int LastId;


        internal CultureActionDetail(UserManager manager, ObservableCollection<CultureAction> localAction)
        {
            //Initialize
            InitializeComponent();
            // for user ID and name of logged user
            userManager = manager;
         
            LastIdTextBlock.Text = LastId.ToString();
            LocalAction = localAction;

            //assing user data to variables for display in view
             UserId = userManager.LoggedUserId;
            OwnerIDTextBlock.Text = UserId.ToString();
            LoggedUserNameTextBlock.Text =   userManager.LoggedUserName;
            cultureActionManager = new CultureActionManager(LocalAction, manager.LoggedUserId);



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


            CultureAction action = new CultureAction(actionName, start, end, childern, adult, senior, actionType, exhibitionType, ticketPrice, organiser, notes, isFree, UserId);

           // cultureActionManager.AddAction(action, LocalAction);
            cultureActionManager.AddCultureAction(actionName, start, end, childern, adult, senior, actionType, exhibitionType, ticketPrice, organiser, notes, isFree, UserId);
            this.Close();


        }

        private string StringRichTextBox(RichTextBox richText)
        {
            TextRange textRange = new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd);
            return textRange.Text;
        }

        internal void FillFormData(CultureAction action)
        {
            CultureActionNameTextBox.Text = action.CultureActionName;
            StartDatePicker.SelectedDate =action.ActionStartDate.Value.ToDateTime(TimeOnly.MinValue);
            EndDatePicker.SelectedDate = action.ActionEndDate.Value.ToDateTime(TimeOnly.MinValue);
            NumberOfChildrenTextBox.Text = action.NumberOfChildren.ToString();
            NumberOfAdultsTextBox.Text = action.NumberOfAdults.ToString();
            NumberOfSeniorsTextBox.Text = action.NumberOfSeniors.ToString();
            CultureActionTypeComboBox.SelectedIndex = (int)action.CultureActionType;
            CultureExhibitionType.SelectedIndex = (int)action.ExhibitionType;
            OrganiseComboBox.SelectedIndex = (int)action.Organiser;
            TicketPriceTextBox.Text = action.TicketPrice.ToString();
            NotesRichTextBox.Document.Blocks.Clear();
            NotesRichTextBox.Document.Blocks.Add(new Paragraph(new Run(action.CultureActionNotes)));
            IsFreeCheckBox.IsChecked = action.IsFree;

        }

       
    }
}
