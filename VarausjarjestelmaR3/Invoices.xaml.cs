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
        private Repository repo = new Repository();
        private string currentPaymentMethod;

        public Invoices()
        {
            InitializeComponent();
            LoadData();
            amountExVATTextBox.TextChanged += AmountExVATTextBox_TextChanged;
        }

        private void LoadData()
        {
            //hae kaikki laskut ja aseta ListViewin tietolähde
            var invoices = repo.GetAllInvoices();
            invoicesListView.ItemsSource = invoices;
            //savedInvoicesListView.ItemsSource = invoices;//Toinen ikkuna, jossa on tallennettuja tilejä

            //haemme tilin enimmäisnumeron arkistosta ja asetamme TextBoxiin seuraavan numeron
            int nextInvoiceNumber = repo.GetMaxInvoiceNumber() + 1;
            invoiceNumberTextBox.Text = nextInvoiceNumber.ToString();
        }

        //maksutavan valinta
        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.IsChecked == true)
            {
                currentPaymentMethod = radioButton.Content.ToString();
            }
        }

        //muutokset tekstiin kentässä, joka koskee arvonlisäverotonta määrää.
        private void AmountExVATTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateVATandTotal();
        }

        //ravonlisäveron ja kokonaismäärän laskentamenetelmä
        private void UpdateVATandTotal()
        {
            if (double.TryParse(amountExVATTextBox.Text, out double amountExVAT))
            {
                double vat = amountExVAT * 0.24; //ALV 24%
                vatTextBox.Text = vat.ToString("N2");

                double totalAmount = amountExVAT + vat;
                totalAmountTextBox.Text = totalAmount.ToString("N2");
            }
            else
            {
                //asettaa oletusarvot, jos syötetty arvo ei ole numero
                vatTextBox.Text = "0.00";
                totalAmountTextBox.Text = "0.00";
            }
        }

        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(asiakasIDTextBox.Text, out int asiakasID) || !int.TryParse(varausIDTextBox.Text, out int varausID))
                {
                    MessageBox.Show("Asiakas ID tai VarausID on virheellinen.");
                    return;
                }

                Invoice newInvoice = new Invoice
                {
                    //tietojen lukeminen käyttöliittymästä
                    Laskunumero = int.Parse(invoiceNumberTextBox.Text),
                    VerotonSumma = double.Parse(amountExVATTextBox.Text),
                    AlvEuroina = double.Parse(vatTextBox.Text),
                    Loppusumma = double.Parse(totalAmountTextBox.Text),
                    Laskutustapa = currentPaymentMethod,
                    AsiakasID = asiakasID,
                    VarausID = varausID
                };


                repo.SaveInvoice(newInvoice); //laskun tallentaminen tietokantaan

                if (savedInvoicesListView.ItemsSource is ObservableCollection<Invoice> savedInvoices)
                {
                    savedInvoices.Add(newInvoice);
                }
                else
                {
                    savedInvoicesListView.ItemsSource = new ObservableCollection<Invoice> { newInvoice };
                }

                MessageBox.Show("Lasku tallennettiin ja lisättiin luetteloon.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tapahtui virhe: {ex.Message}");
            }
        }

        /*Käsittelee laskuluettelon valinnan muuttamista. Jos yksi tili on valittu, näyttää kyseisen tilin tiedot.
          Jos useita tilejä on valittu, laskee ja näyttää kaikkien valittujen tilien summat.*/
        private void InvoicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double totalVerotonSumma = 0;
            double totalAlv = 0;
            double totalLoppusumma = 0;

            //jos vain yksi kohde on valittu, näytä sen tiedot.
            if (invoicesListView.SelectedItems.Count == 1 && invoicesListView.SelectedItem is Invoice selectedInvoice)
            {
                amountExVATTextBox.Text = selectedInvoice.VerotonSumma.ToString("N2");
                vatTextBox.Text = selectedInvoice.AlvEuroina.ToString("N2");
                totalAmountTextBox.Text = selectedInvoice.Loppusumma.ToString("N2");
            }
            else
            {
                //muussa tapauksessa lasketaan valittujen kohtien kokonaissumma.
                foreach (Invoice invoice in invoicesListView.SelectedItems)
                {
                    totalVerotonSumma += invoice.VerotonSumma;
                    totalAlv += invoice.AlvEuroina;
                    totalLoppusumma += invoice.Loppusumma;
                }

                amountExVATTextBox.Text = totalVerotonSumma.ToString("N2");
                vatTextBox.Text = totalAlv.ToString("N2");
                totalAmountTextBox.Text = totalLoppusumma.ToString("N2");
            }
        }

        //menetelmä asiakastietokenttien automaattista täyttämistä varten ID perusteella
        private void AsiakasIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(asiakasIDTextBox.Text, out int asiakasID))
            {
                var customer = repo.GetCustomer(asiakasID);
                if (customer != null)
                {
                    customerNameTextBox.Text = customer.Nimi;
                    customerPhoneTextBox.Text = customer.Puhelin;
                    customerAddressTextBox.Text = customer.Katuosoite;
                    customerPostalCodeTextBox.Text = customer.Postinumero;
                    customerCityTextBox.Text = customer.Postitoimipaikka;
                    customerEmailTextBox.Text = customer.Sahkoposti;
                }
                else
                {
                    ClearCustomerFields();
                }
            }
            else
            {
                ClearCustomerFields();
            }
        }



        //kenttien tyhjentäminen, jos tunnus on väärä tai asiakasta ei löydy
        private void ClearCustomerFields()
        {
            customerNameTextBox.Text = "";
            customerPhoneTextBox.Text = "";
            customerAddressTextBox.Text = "";
            customerPostalCodeTextBox.Text = "";
            customerCityTextBox.Text = "";
            customerEmailTextBox.Text = "";
        }

        private void EditInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (savedInvoicesListView.SelectedItem is Invoice selectedInvoice)
            {
                // Открыть диалоговое окно для редактирования выбранного счёта
                // Здесь может быть вызов формы редактирования с передачей selectedInvoice как параметра
            }
        }

        //menetelmä laskun poistamiseksi tietokannasta
        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (savedInvoicesListView.SelectedItem is Invoice selectedInvoice)
            {
                repo.DeleteInvoice(selectedInvoice.Laskunumero);
                savedInvoicesListView.ItemsSource = repo.GetAllInvoices();
                MessageBox.Show("Tili on poistettu.");
            }
        }


        //menetelmä laskun poistamiseksi vain käyttöliittymästä
        /*private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (savedInvoicesListView.SelectedItem is Invoice selectedInvoice)
            {
                
                var invoices = savedInvoicesListView.ItemsSource as ObservableCollection<Invoice>;
                if (invoices != null)
                {
                    invoices.Remove(selectedInvoice);
                    MessageBox.Show("Tili on poistettu luettelosta.");
                }
                
            }
            else
            {
                MessageBox.Show("Valitse poistettava tili.");
            }
        }*/


    }

}