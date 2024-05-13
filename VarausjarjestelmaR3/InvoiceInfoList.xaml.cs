using MySqlConnector;
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
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for InvoiceInfoList.xaml
    /// </summary>
    public partial class InvoiceInfoList : Window
    {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";
        private Invoice currentInvoice;
        public ObservableCollection<Room> Huoneet { get; set; }
        public ObservableCollection<Service> Palvelut { get; set; }
        public ObservableCollection<string> Laskutustavat { get; set; }

        public InvoiceInfoList(Invoice invoice)
        {
            InitializeComponent();
            currentInvoice = invoice;
            DataContext = this;
            ObservableCollection<Invoice> invoiceDetails = new ObservableCollection<Invoice> { invoice };
            invoiceDetailsListView.ItemsSource = invoiceDetails;
            LoadHuoneet();
            LoadPalvelut();
            LoadLaskutustavat();
            UpdateTotalAmount();
        }

        private void LoadHuoneet()
        {
            Huoneet = new ObservableCollection<Room>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM huoneet";
                var command = new MySqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Huoneet.Add(new Room
                    {
                        HuoneenNumeroID = reader.GetInt32("huoneen_numeroID"),
                        Nimi = reader.GetString("nimi"),
                        Hinta = reader.GetDouble("hinta"),
                        AlvProsentti = reader.GetDouble("alv_prosentti"),
                        HloMaara = reader.GetInt32("hlo_maara"),

                    });
                }
            }
        }

        private void LoadPalvelut()
        {
            Palvelut = new ObservableCollection<Service>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM palvelu";
                var command = new MySqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Palvelut.Add(new Service
                    {
                        PalveluID = reader.GetInt32("palveluID"),
                        Tuote = reader.GetString("tuote"),
                        PalvelunHinta = reader.GetDouble("palvelun_hinta"),
                        AlvProsentti = reader.GetDouble("alv_prosentti"),
                        Maara = reader.GetInt32("maara"),

                    });
                }
            }
        }


        private void LoadLaskutustavat()
        {
            Laskutustavat = new ObservableCollection<string> { "Sähköpostilasku", "Paperilasku" };
        }

        //Laskun loppusumman päivitysmenetelmä
        private void UpdateTotalAmount()
        {
            double totalExVAT = 0;
            double totalVAT = 0;

            //huoneen arvon tarkistaminen ja lisääminen
            if (currentInvoice.Varaus.Huone != null)
            {
                totalExVAT += currentInvoice.Varaus.Huone.Hinta;
                totalVAT += currentInvoice.Varaus.Huone.Hinta * (currentInvoice.Varaus.Huone.AlvProsentti / 100);
            }

            //palvelujen kustannusten tarkistaminen ja lisääminen
            if (currentInvoice.Varaus.VarauksenPalvelut != null && currentInvoice.Varaus.VarauksenPalvelut.Count > 0)
            {
                foreach (var service in currentInvoice.Varaus.VarauksenPalvelut)
                {
                    double serviceTotal = service.Palvelu.PalvelunHinta * service.Kpl;
                    totalExVAT += serviceTotal;
                    totalVAT += serviceTotal * (service.Palvelu.AlvProsentti / 100);
                }
            }

            double totalIncludingVAT = totalExVAT + totalVAT;

            currentInvoice.VerotonSumma = totalExVAT;
            currentInvoice.AlvEuroina = totalVAT;
            currentInvoice.Loppusumma = totalIncludingVAT;

            //käyttöliittymän päivitys
            totalAmountTextBox.Text = totalIncludingVAT.ToString("N2");
            invoiceDetailsListView.Items.Refresh();
        }

        private void SaveInvoiceToDatabase(Invoice invoice)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE lasku SET 
                                    laskutustapa = @Laskutustapa, 
                                    alv_euroina = @AlvEuroina, 
                                    veroton_summa = @VerotonSumma, 
                                    loppusumma = @Loppusumma, 
                                    asiakasID = @AsiakasID, 
                                    varausID = @VarausID 
                                 WHERE laskuID = @Laskunumero";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Laskutustapa", invoice.Laskutustapa);
                command.Parameters.AddWithValue("@AlvEuroina", invoice.AlvEuroina);
                command.Parameters.AddWithValue("@VerotonSumma", invoice.VerotonSumma);
                command.Parameters.AddWithValue("@Loppusumma", invoice.Loppusumma);
                command.Parameters.AddWithValue("@AsiakasID", invoice.AsiakasID);
                command.Parameters.AddWithValue("@VarausID", invoice.VarausID);
                command.Parameters.AddWithValue("@Laskunumero", invoice.Laskunumero);
                command.ExecuteNonQuery();

                string updateVarausQuery = @"UPDATE asiakkaan_varaus SET 
                                                varaus_alkaa = @VarausAlkaa, 
                                                varaus_paattyy = @VarausPaattyy, 
                                                huoneen_numeroID = @HuoneenNumeroID 
                                            WHERE varausID = @VarausID";
                var varausCommand = new MySqlCommand(updateVarausQuery, connection);
                varausCommand.Parameters.AddWithValue("@VarausAlkaa", invoice.Varaus.VarausAlkaa);
                varausCommand.Parameters.AddWithValue("@VarausPaattyy", invoice.Varaus.VarausPaattyy);
                varausCommand.Parameters.AddWithValue("@HuoneenNumeroID", invoice.Varaus.Huone.HuoneenNumeroID);
                varausCommand.Parameters.AddWithValue("@VarausID", invoice.VarausID);
                varausCommand.ExecuteNonQuery();


                foreach (var service in invoice.Varaus.VarauksenPalvelut)
                {
                    string updatePalveluQuery = @"UPDATE varauksen_palvelut SET 
                                                    palveluID = @PalveluID, 
                                                    kpl = @Kpl 
                                                WHERE as_palveluvarauksenID = @PalveluvarausID";
                    var palveluCommand = new MySqlCommand(updatePalveluQuery, connection);
                    palveluCommand.Parameters.AddWithValue("@PalveluID", service.Palvelu.PalveluID);
                    palveluCommand.Parameters.AddWithValue("@Kpl", service.Kpl);
                    palveluCommand.Parameters.AddWithValue("@PalveluvarausID", service.PalveluvarausID);
                    palveluCommand.ExecuteNonQuery();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            SaveInvoiceToDatabase(currentInvoice);
            MessageBox.Show("Muutokset tallennettu.");
            invoiceDetailsListView.IsEnabled = false;
        }
    }
}
