using EventHarbor.Class;
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
    /// Interaction logic for AdvancedStatisticScreen.xaml
    /// </summary>
    public partial class AdvancedStatisticScreen : Window
    {
        InputValidation inputValidation = new InputValidation();
        internal ObservableCollection<CultureAction> LocalCollection { get; set; }
        
        internal AdvancedStatisticScreen(ObservableCollection<CultureAction> localCollection)
        {
            LocalCollection = localCollection;
            InitializeComponent();
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

        private void GenerateByDateBtn_Click(object sender, RoutedEventArgs e)
        {
            AdvanceStatistic advanceStatistic = new AdvanceStatistic(LocalCollection);
            try
            {
                if (UseDate.IsChecked == true)
                {
                    DateOnly startDate = inputValidation.ValidateDateOnly(StartDatePicker.SelectedDate, "Začátek akce");
                    DateOnly endDate = inputValidation.ValidateDateOnly(EndDatePicker.SelectedDate, "Konec akce");

                    string statistic = advanceStatistic.GenerateStatistic(advanceStatistic.CreateFilteredCollectionBasedOnDate(startDate, endDate));
                    MessageBox.Show(statistic);
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           

        }

       
    }
}
