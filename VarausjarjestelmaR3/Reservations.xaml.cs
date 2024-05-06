using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using VarausjarjestelmaR3.Classes;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Data;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for Reservations.xaml
    /// </summary>
    public partial class Reservations : UserControl
    {
        Repository repo;
        ObservableCollection<Reservation> varaukset;
        ObservableCollection<Reservation> menneet;
        ObservableCollection<Reservation> nykyiset;
        ObservableCollection<Reservation> tulevat;
        ObservableCollection<Service> palvelut;

        public Reservations()
        {
            InitializeComponent();

            GetData();
        }

        public void GetData()
        {
            Repository repo = new Repository();

            varaukset = repo.GetAllReservations();

            menneet = new ObservableCollection<Reservation>();
            nykyiset = new ObservableCollection<Reservation>();
            tulevat = new ObservableCollection<Reservation>();

            OrganiseReservationsByTime(varaukset);

            lvMenneetVaraukset.ItemsSource = menneet;
            lvNytVaraukset.ItemsSource = nykyiset;
            lvTulevatVaraukset.ItemsSource = tulevat;
        }

        //Lajitellaan varaukset päivämäärien mukaan menneisiin, nykyisiin ja tuleviin:
        public void OrganiseReservationsByTime(ObservableCollection<Reservation> varaukset)
        {
            foreach (var varaus in varaukset)
            {
                if (varaus.VarausPaattyy < DateTime.Now)
                {
                    menneet.Add(varaus);
                }
                else if (varaus.VarausAlkaa <= DateTime.Now && varaus.VarausPaattyy >= DateTime.Now)
                {
                    nykyiset.Add(varaus);
                }
                else if (varaus.VarausAlkaa > DateTime.Now)
                {
                    tulevat.Add(varaus);
                }
                else
                {
                    MessageBox.Show("Tapahtui virhe.");
                }
            }
        }

        //Valitun varauksen avaus ja muokkaus Muokkaa-välilehdessä:
        private void Muokkaa_Click(object sender, RoutedEventArgs e)
        {
            var valittuVaraus = (sender as Button)?.DataContext as Reservation;

            //Varaus- ja asiakastiedot:
            ID.Text = valittuVaraus.VarausID.ToString();
            VarausAlkaa.Text = valittuVaraus.VarausAlkaa.ToString("dd.MM.yyyy");
            VarausPaattyy.Text = valittuVaraus.VarausPaattyy.ToString("dd.MM.yyyy");
            Varauspvm.Text = valittuVaraus.Varauspaiva.ToString("dd.MM.yyyy");
            Lisatiedot.Text = valittuVaraus.Lisatiedot;
            Huone.Text = valittuVaraus.Huone.Nimi;
            HuoneID.Text = valittuVaraus.Huone.HuoneenNumeroID.ToString();
            Tyontekija.Text = valittuVaraus.Tyontekija.TyontekijaID.ToString();

            Asiakasnumero.Text = valittuVaraus.Asiakas.AsiakasID.ToString();
            Nimi.Text = valittuVaraus.Asiakas.Nimi; ;
            Katuosoite.Text = valittuVaraus.Asiakas.Katuosoite;
            Postinumero.Text = valittuVaraus.Asiakas.Postinumero;
            Postitoimipaikka.Text = valittuVaraus.Asiakas.Postitoimipaikka;
            Puhelinnumero.Text = valittuVaraus.Asiakas.Puhelin;
            Sahkoposti.Text = valittuVaraus.Asiakas.Sahkoposti;

            //Varauksen palvelut:
            Repository repo = new Repository();

            var varauksenPalvelut = repo.GetReservationServices(valittuVaraus.VarausID);
            var kaikkiPalvelut = repo.GetAllServices();

            var toimipisteenPalvelut = new ObservableCollection<Service>();

            foreach (var palvelu in  kaikkiPalvelut)
            {
                if (palvelu.Toimipiste.ToimipisteID == valittuVaraus.Huone.Toimipiste.ToimipisteID)
                {
                    toimipisteenPalvelut.Add(palvelu);
                }
            }

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("PalveluID", typeof(int));
            dataTable.Columns.Add("Tuote", typeof(string));
            dataTable.Columns.Add("Maara", typeof(int));
            dataTable.Columns.Add("VarattuMaara", typeof(int));

            foreach (Service palvelu in toimipisteenPalvelut)
            {
                dataTable.Rows.Add(palvelu.PalveluID, palvelu.Tuote, palvelu.Maara, 0);
            }

            foreach (var varattuPalvelu in varauksenPalvelut)
            {
                DataRow row = dataTable.Rows.Cast<DataRow>().FirstOrDefault(r => (int)r["PalveluID"] == varattuPalvelu.Palvelu.PalveluID);
                if (row != null)
                {
                    row["VarattuMaara"] = (int)row["VarattuMaara"] + varattuPalvelu.Kpl;
                }
            }

            dgPalvelut.ItemsSource = dataTable.DefaultView;

            tabControl.SelectedIndex = 3;
        }

        //Varauksen muutosten tallennus:
        private void TallennaMuokkaus_Click(object sender, RoutedEventArgs e)
        {
            Repository repo = new Repository();

            //Asiakastiedot:
            Customer asiakas = new Customer();
            asiakas.AsiakasID = int.Parse(Asiakasnumero.Text);
            asiakas.Nimi = Nimi.Text;
            asiakas.Katuosoite = Katuosoite.Text;
            asiakas.Postinumero = Postinumero.Text;
            asiakas.Postitoimipaikka = Postitoimipaikka.Text;
            asiakas.Puhelin = Puhelinnumero.Text;
            asiakas.Sahkoposti = Sahkoposti.Text;

            //Varauksen palvelutiedot:
            var varauksenPalvelut = new ObservableCollection<ReservationService>();
            foreach (DataRowView rowView in dgPalvelut.ItemsSource)
            {
                var row = rowView.Row;
                var palveluID = (int)row["PalveluID"];
                var varattuMaara = (int)row["VarattuMaara"];

                if (varattuMaara > 0 ) {
                var palvelu = new ReservationService
                {
                    Palvelu = new Service { PalveluID = palveluID },
                    Kpl = varattuMaara
                };
                varauksenPalvelut.Add(palvelu);
                }
            }

            Reservation muokattuVaraus = new Reservation();
            muokattuVaraus.VarausID = int.Parse(ID.Text);
            muokattuVaraus.VarausAlkaa = DateTime.Parse(VarausAlkaa.Text);
            muokattuVaraus.VarausPaattyy = DateTime.Parse(VarausPaattyy.Text);
            muokattuVaraus.Varauspaiva = DateTime.Parse(Varauspvm.Text);
            muokattuVaraus.Huone = repo.GetRoom(int.Parse(HuoneID.Text));
            muokattuVaraus.Lisatiedot = Lisatiedot.Text;
            muokattuVaraus.Asiakas = asiakas;
            muokattuVaraus.Tyontekija = repo.GetEmployee(int.Parse(Tyontekija.Text));
            muokattuVaraus.VarauksenPalvelut = varauksenPalvelut;


            //Tallennus:

            repo.SaveCustomer(asiakas);
            repo.SaveReservation(muokattuVaraus);

            MessageBox.Show("Varauksen muutokset tallennettu.");

            GetData();
            tabControl.SelectedIndex = 0;
        }

        //Varauksen poisto: 
        private void PoistaVaraus_Click(object sender, RoutedEventArgs e)
        {
            Repository repo = new Repository();

            var valittuVaraus = (sender as Button)?.DataContext as Reservation;

            int varausID = int.Parse(ID.Text);

            Boolean laskuOlemassa = OnkoLaskuOlemassa(varausID);

            if (laskuOlemassa == true)
            {
                MessageBox.Show("Varausta ei voida poistaa, koska siihen liittyy lasku. Poista ensin lasku.");
            }
            else
            {
                repo.DeleteReservation(varausID);
                
                MessageBox.Show("Varaus poistettu.");
                GetData();
                tabControl.SelectedIndex = 0;
            }
        }

        //Ennen poistoa tarkistetaan, liittyykö varaukseen lasku; jos liittyy, ei varausta voi poistaa ennen laskun poistamista.
        public Boolean OnkoLaskuOlemassa(int varausID)
        {
            Repository repo = new Repository();
            var laskut = repo.GetAllInvoices();

            foreach (var lasku in laskut)
            {
                if (lasku.Varaus.VarausID == varausID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
