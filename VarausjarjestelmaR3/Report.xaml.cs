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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VarausjarjestelmaR3
    {
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
        {
        Repository repo;


        public Report ()
            {
                {
                InitializeComponent();
                repo = new Repository();
                }

            }
        private void GetInfo_Click (object sender, RoutedEventArgs e)
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

        private void PrintButton_Click (object sender, RoutedEventArgs e)
            {
                

                // Luodaan PrintDialog-olio
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                    {
                    // Luo RenderTargetBitmap-olio ja piirrä DataGrid sen päälle
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)dataGridInvoices.ActualWidth, (int)dataGridInvoices.ActualHeight, 96, 96, PixelFormats.Default);
                    renderTargetBitmap.Render(dataGridInvoices);

                    // Luo Image-olio ja aseta sille renderöity kuva
                    Image image = new Image();
                    image.Source = renderTargetBitmap;

                    // Käynnistä tulostus
                    printDialog.PrintVisual(image, "Tulosta DataGrid");
                    }
            }
        }
    }