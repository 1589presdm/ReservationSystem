using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        Repository repo;
        ObservableCollection<Reservation> kaikkiVaraukset;
        ObservableCollection<Reservation> valitutVaraukset;
        ObservableCollection<Reservation> jarjestysVaraukset;
        DateTime startDate;
        DateTime endDate;

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
            kaikkiVaraukset = repo.GetAllReservations();
            jarjestysVaraukset = new ObservableCollection<Reservation>(kaikkiVaraukset.OrderBy(v => v.VarausAlkaa));  //Varaukset aikajärjestykseen.

            valitutVaraukset = new ObservableCollection<Reservation>();
            lvVaraukset.ItemsSource = valitutVaraukset;

            //// Fetch data from the repository
            //DataTable dataTable = repo.GetInvoicesForPrint();

            // Get the selected start and end dates
            startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
            endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

            FilterReservations(jarjestysVaraukset, startDate, endDate);

            //// Filter the data based on the date range
            //DataView dataView = new DataView(dataTable);
            //dataView.RowFilter = $"varaus_alkaa >= '{startDate.ToString("yyyy-MM-dd")}' AND varaus_alkaa <= '{endDate.ToString("yyyy-MM-dd")}'";

            //// Update the DataGrid with the filtered data
            //dataGridInvoices.ItemsSource = dataView;
        }

        public void FilterReservations(ObservableCollection<Reservation> varaukset, DateTime startDate, DateTime endDate)
        {
            foreach (var varaus in varaukset)
            {
                if ((varaus.VarausAlkaa >= startDate && varaus.VarausPaattyy <= endDate) || // varaus alkaa ja päättyy valitulla aikavälillä
                    (varaus.VarausAlkaa < startDate && varaus.VarausPaattyy <= endDate && varaus.VarausPaattyy >= startDate) || // varaus alkaa ennen valittua alkupäivää mutta päättyy valitulla aikavälillä
                    (varaus.VarausAlkaa < startDate && varaus.VarausPaattyy > endDate) || // varaus alkaa ennen valittua päivää ja päättyy valitun päivän jälkeen
                    (varaus.VarausAlkaa >= startDate && varaus.VarausAlkaa <= endDate && varaus.VarausPaattyy > endDate)) //varaus alkaa valitulla välillä ja päättyy valitun loppupäivän jälkeen
                //Jos varaus täyttää jonkin ehdoista:
                {
                    valitutVaraukset.Add(varaus);
                }
            }
        }


        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            ListView listView = lvVaraukset as ListView;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = System.IO.Path.Combine(desktopPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Tiedot väliltä {startDate.ToString("dd.MM.yyyy")}-{endDate.ToString("dd.MM.yyyy")}: ");
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();

                foreach (var item in listView.Items)
                {
                    if (item is Reservation reservation)
                    {
                        //Muotoillaan päivämäärä:
                        string varausAlkaa = reservation.VarausAlkaa.ToString("dd.MM.yyyy");
                        string varausPaattyy = reservation.VarausPaattyy.ToString("dd.MM.yyyy");

                        //Varauksen tiedot tiedostoon:
                        writer.WriteLine($"Aika: \t\t{varausAlkaa}-{varausPaattyy}");
                        writer.WriteLine($"Toimipiste: \t{reservation.Huone.Toimipiste}");
                        writer.WriteLine($"Huone: \t\t{reservation.Huone.Nimi}");
                        writer.WriteLine($"Asiakas: \t{reservation.Asiakas.Nimi}, asiakasnumero {reservation.Asiakas.AsiakasID}");
                        writer.WriteLine($"Varausnumero: \t{reservation.VarausID}");

                        //Haetaan varauksen palvelut:
                        var varauksenPalvelut = reservation.VarauksenPalvelut;

                        if (varauksenPalvelut.Count > 0)
                        {
                            writer.Write("Palvelut: \t");
                            for (int i = 0; i < varauksenPalvelut.Count; i++)
                            {
                                var palvelu = varauksenPalvelut[i];
                                writer.Write($"{palvelu.Palvelu.Tuote} {palvelu.Kpl} kpl");

                                //Lisätään pilkku ja välilyönti, paitsi jos viimeinen tulostettava palvelu
                                if (i < varauksenPalvelut.Count - 1)
                                {
                                    writer.Write(", ");
                                }
                            }
                            writer.WriteLine();
                        }

                        if (reservation.Lisatiedot != null && reservation.Lisatiedot != "")
                        {
                            writer.WriteLine($"Lisätiedot: \t{reservation.Lisatiedot}");
                        }

                        //Tyhjiä rivejä ja viivaa jokaisen varauksen väliin:
                        writer.WriteLine();
                        writer.WriteLine("--------------------------------------------------");
                        writer.WriteLine();
                    }
                }

                MessageBox.Show($"Raportti tallennettu työpöydälle tekstitiedostoon nimellä: {fileName}", "Tallennus onnistui", MessageBoxButton.OK, MessageBoxImage.Information);  //Confirmation box
            }


            //RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            //renderTargetBitmap.Render(this);

            //PngBitmapEncoder pngEncoder = new PngBitmapEncoder();                                            //Create screenshot as PNG
            //pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            //MemoryStream memoryStream = new MemoryStream();
            //pngEncoder.Save(memoryStream);

            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);              //Save to desktop
            //string fileName = $"UI_Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            //string filePath = System.IO.Path.Combine(desktopPath, fileName);
            //File.WriteAllBytes(filePath, memoryStream.ToArray());

            //MessageBox.Show($"Kuva tallennettu työpöydälle nimellä: {fileName}", "Tallennus onnistui", MessageBoxButton.OK, MessageBoxImage.Information);  //Confirmation box
        }

    }
}
