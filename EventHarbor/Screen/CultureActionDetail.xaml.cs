using EventHarbor.Class;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace EventHarbor.Screen
{
    /// <summary>
    /// Interaction logic for CultureActionDetail.xaml
    /// </summary>


    public partial class CultureActionDetail : Window
    {
        UserManager userManager;
        private CultureActionManager cultureActionManager;
        internal ObservableCollection<CultureAction> LocalAction;
        int UserId;
        int LastId;
        CultureAction SelectedAction;
        InputValidation inputValidation = new InputValidation();
        private readonly bool IsNew;

        /// <summary>
        /// Base constructor for detail view
        /// </summary>
        /// <param name="manager"> instance of UserMananger for logged user</param>
        /// <param name="localAction">instance of local colection for data manipulation</param>
        internal CultureActionDetail(UserManager manager, ObservableCollection<CultureAction> localAction, bool isNew)
        {
            //Initialize
            InitializeComponent();
            // for user ID and name of logged user
            userManager = manager;
            IsNew = isNew;
            LastIdTextBlock.Text = LastId.ToString();
            LocalAction = localAction;

            //assing user data to variables for display in view
            UserId = userManager.LoggedUserId;
            OwnerIDTextBlock.Text = UserId.ToString();
            LoggedUserNameTextBlock.Text = userManager.LoggedUserName;
            cultureActionManager = new CultureActionManager(LocalAction, manager.LoggedUserId);
            SetButtonContent();


        }



        //close button
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //window move function
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = sender as Border;
            var win = Window.GetWindow(move);
            win.DragMove();
        }
        //exit button
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Create action object for manipulation
        /// </summary>
        /// <returns>CultureAction Object</returns>
        //need improvement for data validation
        private CultureAction CreateActionObject()
        {
            try
            {
                string actionName = inputValidation.ValidateText(CultureActionNameTextBox.Text, "Název akce");
                DateOnly startDate = inputValidation.ValidateDateOnly(StartDatePicker.SelectedDate,"Začátek akce");
                DateOnly endDate = inputValidation.ValidateDateOnly(EndDatePicker.SelectedDate, "Konec akce");
                int childern = inputValidation.ValidateNumber(NumberOfChildrenTextBox.Text,"Počet dětí");
                int adult = inputValidation.ValidateNumber(NumberOfAdultsTextBox.Text, "Počet dospělých");
                int senior = inputValidation.ValidateNumber(NumberOfSeniorsTextBox.Text, "Počet seniorů");
                int disabled = inputValidation.ValidateNumber(NumberOfDisabledTextBox.Text,"Počet postižených");
                CultureActionType actionType = (CultureActionType)CultureActionTypeComboBox.SelectedIndex;
                ExhibitionType exhibitionType = (ExhibitionType)CultureExhibitionType.SelectedIndex;
                Organiser organiser = (Organiser)OrganiseComboBox.SelectedIndex;
                int actionPrice = inputValidation.ValidateNumber(ActionPriceTextBox.Text, "Náklady na akci");

                string notes = inputValidation.ValidateText(StringRichTextBox(NotesRichTextBox),"poznámka");

                bool isFree = IsFreeCheckBox.IsChecked.HasValue ? IsFreeCheckBox.IsChecked.Value : false;
                
                
                CultureAction action = new CultureAction(actionName, startDate, endDate, childern, adult, senior, disabled, actionType, exhibitionType, actionPrice, organiser, notes, isFree,  UserId);
                return action;
            } 
            catch (Exception e) 
            {
                MessageBox.Show(e.Message);
                return null;

            }

            /*
            string actionName = CultureActionNameTextBox.Text;
            DateOnly? start = StartDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value) : null;
            DateOnly? end = EndDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value) : null;
            int childern = NumberOfChildrenTextBox.Text != null ? int.Parse(NumberOfChildrenTextBox.Text.Trim()) : 0;
            int adult = NumberOfAdultsTextBox.Text != null ? int.Parse(NumberOfAdultsTextBox.Text.Trim()) : 0;
            int senior = NumberOfSeniorsTextBox.Text != null ? int.Parse(NumberOfSeniorsTextBox.Text.Trim()) : 0;
            CultureActionType actionType = (CultureActionType)CultureActionTypeComboBox.SelectedIndex;
            ExhibitionType exhibitionType = (ExhibitionType)CultureExhibitionType.SelectedIndex;
            Organiser organiser = (Organiser)OrganiseComboBox.SelectedIndex;
            int actionPrice = int.Parse(ActionPriceTextBox.Text.Trim());
            string notes = StringRichTextBox(NotesRichTextBox);
            bool isFree = IsFreeCheckBox.IsChecked.HasValue ? IsFreeCheckBox.IsChecked.Value : false;


            CultureAction action = new CultureAction(actionName, startDate, endDate, childern, adult, senior, actionType, exhibitionType, actionPrice, organiser,notes, isFree, UserId);
             return action;
            */
        }

        private void SetButtonContent()
        {
            SaveBtn.Content = IsNew ? "Vytvořit" : "Upravit";
        }

        //save button
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNew)
            {
                //add new action
                CultureAction action = CreateActionObject();
                if (action != null)
                {
                    cultureActionManager.AddAction(action, LocalAction);
                    this.Close();
                }
            }
            else
            {
                //edit action
                CultureAction editedAction = CreateActionObject();
                if (editedAction != null)
                {
                    cultureActionManager.EditAction(SelectedAction, editedAction, LocalAction);
                    this.Close();
                }
            }

        }

        //function for conver text from RichTextBox to string
        private string StringRichTextBox(RichTextBox richText)
        {
            //TextRange textRange = new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd);
            //return textRange.Text; 
            return new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd).Text;
        }

        /// <summary>
        /// Fill form with data
        /// </summary>
        /// <param name="action">Selected action</param>
        internal void FillFormData(CultureAction action)
        {
            SelectedAction = action;
            CultureActionNameTextBox.Text = action.CultureActionName;
            StartDatePicker.SelectedDate = action.ActionStartDate.Value.ToDateTime(TimeOnly.MinValue);
            EndDatePicker.SelectedDate = action.ActionEndDate.Value.ToDateTime(TimeOnly.MinValue);
            NumberOfChildrenTextBox.Text = action.NumberOfChildren.ToString();
            NumberOfAdultsTextBox.Text = action.NumberOfAdults.ToString();
            NumberOfSeniorsTextBox.Text = action.NumberOfSeniors.ToString();
            NumberOfDisabledTextBox.Text = action.NumberOfDisabled.ToString();
            CultureActionTypeComboBox.SelectedIndex = (int)action.CultureActionType;
            CultureExhibitionType.SelectedIndex = (int)action.ExhibitionType;
            OrganiseComboBox.SelectedIndex = (int)action.Organiser;
            ActionPriceTextBox.Text = action.ActionPrice.ToString();
            NotesRichTextBox.Document.Blocks.Clear();
            NotesRichTextBox.Document.Blocks.Add(new Paragraph(new Run(action.CultureActionNotes)));
            IsFreeCheckBox.IsChecked = action.IsFree;

        }

    }
}
