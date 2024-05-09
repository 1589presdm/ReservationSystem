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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VarausjarjestelmaR3;
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    public partial class Invoices : UserControl
    {
        private readonly Repository repo = new Repository();
        private string currentPaymentMethod;

        public Invoices()
        {
            InitializeComponent();
            LoadData();
            amountExVATTextBox.TextChanged += AmountExVATTextBox_TextChanged;
            invoicesListView.ItemsSource = null;
        }


        private void LoadData()
        {
            var reservations = repo.GetAllReservations();
            invoicesListView.ItemsSource = reservations;
            int nextInvoiceNumber = repo.GetMaxInvoiceNumber() + 1;
            invoiceNumberTextBox.Text = nextInvoiceNumber.ToString();
        }

        //maksutavan valintaa käsitellään
        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked == true)
            {
                currentPaymentMethod = radioButton.Content.ToString();
            }
        }

        //käsitellään tekstimuutoksia summakentässä ilman ALV
        private void AmountExVATTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateVATandTotal();
        }


        private void UpdateVATandTotal()
        {
            if (double.TryParse(amountExVATTextBox.Text, out double amountExVAT))
            {
                double vat = amountExVAT * 0.24; // ALV 24%
                vatTextBox.Text = vat.ToString("N2");
                double totalAmount = amountExVAT + vat;
                totalAmountTextBox.Text = totalAmount.ToString("N2");
            }
            else
            {
                //ALV- ja kokonaissumma-kenttien nollaaminen, jos ne on syötetty väärin
                vatTextBox.Text = "0.00";
                totalAmountTextBox.Text = "0.00";
            }
        }

        //tilin tallentaminen
        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VarausID-tarkistus
                if (!int.TryParse(varausIDTextBox.Text, out int varausID))
                {
                    MessageBox.Show("Virheellinen syöte.");
                    ClearCustomerFields();
                    ClearFinancialFields();
                    return;
                }

                //valittujen varaus- ja summakenttien tarkistaminen
                if (invoicesListView.SelectedItem is Reservation selectedReservation &&
                    double.TryParse(amountExVATTextBox.Text, out double amountExVAT) &&
                    double.TryParse(vatTextBox.Text, out double vat) &&
                    double.TryParse(totalAmountTextBox.Text, out double totalAmount))
                {
                    int nextInvoiceNumber = repo.GetMaxInvoiceNumber();

                    Invoice newInvoice = new Invoice
                    {
                        Laskunumero = nextInvoiceNumber,
                        VerotonSumma = amountExVAT,
                        AlvEuroina = vat,
                        Loppusumma = totalAmount,
                        Laskutustapa = currentPaymentMethod,
                        AsiakasID = selectedReservation.Asiakas.AsiakasID,
                        Asiakas = selectedReservation.Asiakas,
                        VarausID = varausID
                    };

                    repo.SaveInvoice(newInvoice);

                    if (savedInvoicesListView.ItemsSource is ObservableCollection<Invoice> savedInvoices)
                    {
                        savedInvoices.Add(newInvoice);
                        savedInvoicesListView.Items.Refresh();
                    }
                    else
                    {
                        var newInvoicesList = new ObservableCollection<Invoice> { newInvoice };
                        savedInvoicesListView.ItemsSource = newInvoicesList;
                        savedInvoicesListView.Items.Refresh();
                    }

                    MessageBox.Show("Lasku tallennettiin ja lisättiin luetteloon.");
                }
                else
                {
                    MessageBox.Show("Virheellinen syöte.");
                    ClearFinancialFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tapahtui virhe: {ex.Message}");
            }
        }


        //tekstimuutosten käsittely VarausID-kentässä
        private void VarausIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //VarausID-tarkistus
            if (int.TryParse(varausIDTextBox.Text, out int varausID))
            {

                var reservation = repo.GetReservation(varausID);

                if (reservation?.Asiakas != null)
                {
                    customerNameTextBox.Text = reservation.Asiakas.Nimi;
                    customerPhoneTextBox.Text = reservation.Asiakas.Puhelin;
                    customerAddressTextBox.Text = reservation.Asiakas.Katuosoite;
                    customerPostalCodeTextBox.Text = reservation.Asiakas.Postinumero;
                    customerCityTextBox.Text = reservation.Asiakas.Postitoimipaikka;
                    customerEmailTextBox.Text = reservation.Asiakas.Sahkoposti;

                    invoicesListView.ItemsSource = new ObservableCollection<Reservation> { reservation };
                }
                else
                {
                    MessageBox.Show("Määritetyllä ID varustettuja varauksia ei löytynyt.");
                    ClearCustomerFields();
                    ClearFinancialFields();
                    invoicesListView.ItemsSource = null;
                }
            }
            else
            {
                ClearCustomerFields();
                ClearFinancialFields();
                invoicesListView.ItemsSource = null;
            }
        }

        //varauksen yksityiskohtien näyttäminen
        private void DisplayReservationDetails(Reservation reservation)
        {
            double totalExVAT = 0;
            double totalVAT = 0;

            //huoneen hinnan tarkistaminen ja laskeminen
            if (reservation.Huone != null)
            {
                totalExVAT += reservation.Huone.Hinta;
                totalVAT += reservation.Huone.Hinta * (reservation.Huone.AlvProsentti / 100);
            }

            //palvelujen kustannusten tarkistaminen ja laskeminen
            if (reservation.VarauksenPalvelut != null && reservation.VarauksenPalvelut.Count > 0)
            {
                foreach (var service in reservation.VarauksenPalvelut)
                {
                    double serviceTotal = service.Palvelu.PalvelunHinta * service.Kpl;
                    totalExVAT += serviceTotal;
                    totalVAT += serviceTotal * (service.Palvelu.AlvProsentti / 100);
                }
            }

            double totalIncludingVAT = totalExVAT + totalVAT;

            amountExVATTextBox.Text = totalExVAT.ToString("N2");
            vatTextBox.Text = totalVAT.ToString("N2");
            totalAmountTextBox.Text = totalIncludingVAT.ToString("N2");
        }

        //valinnan muutosten käsittely ListView
        private void InvoicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (invoicesListView.SelectedItem is Reservation selectedReservation)
            {
                customerNameTextBox.Text = selectedReservation.Asiakas.Nimi;
                customerPhoneTextBox.Text = selectedReservation.Asiakas.Puhelin;
                customerAddressTextBox.Text = selectedReservation.Asiakas.Katuosoite;
                customerPostalCodeTextBox.Text = selectedReservation.Asiakas.Postinumero;
                customerCityTextBox.Text = selectedReservation.Asiakas.Postitoimipaikka;
                customerEmailTextBox.Text = selectedReservation.Asiakas.Sahkoposti;


                DisplayReservationDetails(selectedReservation);
            }
            else
            {
                ClearCustomerFields();
                ClearFinancialFields();
            }
        }

        private void ClearFinancialFields()
        {
            amountExVATTextBox.Text = "";
            vatTextBox.Text = "";
            totalAmountTextBox.Text = "";
        }

        private void ClearCustomerFields()
        {
            customerNameTextBox.Text = "";
            customerPhoneTextBox.Text = "";
            customerAddressTextBox.Text = "";
            customerPostalCodeTextBox.Text = "";
            customerCityTextBox.Text = "";
            customerEmailTextBox.Text = "";
        }

        //laskun poistaminen tietokannasta
        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (savedInvoicesListView.SelectedItem is Invoice selectedInvoice)
            {
                repo.DeleteInvoice(selectedInvoice.Laskunumero);
                savedInvoicesListView.ItemsSource = repo.GetAllInvoices();
                MessageBox.Show("Tili on poistettu.");
            }
        }
    }
}
