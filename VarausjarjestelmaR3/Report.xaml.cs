using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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


        public Report()
        {
            {
                InitializeComponent();
                repo = new Repository();
            }

        }
        private void GetInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    DataTable dataTable = repo.GetInvoicesForPrint();

        //    DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
        //    DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

        //    dataGridInvoices.ItemsSource = dataTable.DefaultView;
        //}

        {
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


        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {


            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this);

            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();                                            //Create screenshot as PNG
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            MemoryStream memoryStream = new MemoryStream();
            pngEncoder.Save(memoryStream);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);              //Save to desktop
            string fileName = $"UI_Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filePath = System.IO.Path.Combine(desktopPath, fileName);
            File.WriteAllBytes(filePath, memoryStream.ToArray());

            MessageBox.Show($"Kuva tallennettu työpöydälle nimellä: {fileName}", "Tallennus onnistui", MessageBoxButton.OK, MessageBoxImage.Information);  //Confirmation box
        }
        
    }
}