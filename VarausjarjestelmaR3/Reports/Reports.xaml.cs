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

        private void HaeTiedot_Click(object sender, RoutedEventArgs e)  //Get laskunumbers click
        {
            DataTable dataTable = repo.GetInvoicesForPrint();
            dataGridInvoices.ItemsSource = dataTable.DefaultView;
        }
        private void Exit(object sender, RoutedEventArgs e)  //close window
        {
            DialogResult = true;
        }

    }
}
