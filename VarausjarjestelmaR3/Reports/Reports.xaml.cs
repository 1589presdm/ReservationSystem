using System;
using System.Collections.Generic;
using System.Data;
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

namespace VarausjarjestelmaR3.Reports
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        public Reports()
        {
            InitializeComponent();
            repo = new Repository();
        }

        private Repository repo;

        private void GetInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    DataTable dataTable = repo.GetInvoicesForPrint();

        //    DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
        //    DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

        //    dataGridInvoices.ItemsSource = dataTable.DefaultView;
        //}
        {//untested, waiting for code fixes in customers.xaml
            // Fetch data from the repository
            DataTable dataTable = repo.GetInvoicesForPrint();

            // Get the selected start and end dates
            DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

            // Filter the data based on the date range
            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"varaus_alkaa >= '{startDate.ToString("yyyy-MM-dd")}' AND varaus_alkaa <= '{endDate.ToString("yyyy-MM-dd")}'";

            // Update the DataGrid with the filtered data
            dataGridInvoices.ItemsSource = dataView;
        }
        private void Exit(object sender, RoutedEventArgs e)  
        {
            DialogResult = true;
        }

    }
}
