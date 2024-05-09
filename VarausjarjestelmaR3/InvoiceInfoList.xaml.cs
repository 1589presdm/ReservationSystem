using System;
using System.Collections.Generic;
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
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for InvoiceInfoList.xaml
    /// </summary>
    public partial class InvoiceInfoList : Window
    {
        public InvoiceInfoList(Invoice invoice)
        {
            InitializeComponent();
            DataContext = invoice;
            invoiceDetailsListView.ItemsSource = new[] { invoice };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
