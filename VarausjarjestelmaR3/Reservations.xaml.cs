using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using VarausjarjestelmaR3.Classes;

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
                } else if (varaus.VarausAlkaa <= DateTime.Now && varaus.VarausPaattyy >= DateTime.Now) {
                    nykyiset.Add(varaus);
                } else if (varaus.VarausAlkaa > DateTime.Now)
                {
                    tulevat.Add(varaus);
                } else
                {
                    MessageBox.Show("Tapahtui virhe.");
                }
            }
        }

        //Valitun varauksen avaus Tarkastele-välilehdessä:
        //Tällä hetkellä valitut palvelut näytetään CheckBoxeissa, mutta pitäisi muuttaa sellaiseen systeemiin, että myös varatut määrät näkyvät.
        private void Tarkastele_Click(object sender, RoutedEventArgs e)
        {
            stackPanelPalvelut.Children.Clear();

            var valittuVaraus = (sender as Button)?.DataContext as Reservation;

            ID.Text = valittuVaraus.VarausID.ToString();
            VarausAlkaa.Text = valittuVaraus.VarausAlkaa.ToString("dd.MM.yyyy");
            VarausPaattyy.Text = valittuVaraus.VarausPaattyy.ToString("dd.MM.yyyy");
            Varauspvm.Text = valittuVaraus.Varauspaiva.ToString("dd.MM.yyyy");
            Lisatiedot.Text = valittuVaraus.Lisatiedot;
            Huone.Text = valittuVaraus.Huone.Nimi;
            Asiakas.Text = valittuVaraus.Asiakas.Nimi;

            Repository repo = new Repository();
            palvelut = repo.GetAllServices();

            // Täytetään CheckBoxit kaikilla palveluilla
            foreach (var palvelu in palvelut)
            {
                if (valittuVaraus.Huone.Toimipiste.ToimipisteID == palvelu.Toimipiste.ToimipisteID) { 
                CheckBox checkBox = new CheckBox();
                checkBox.Content = palvelu.Tuote;
                stackPanelPalvelut.Children.Add(checkBox);
                }
            }

            // Asetetaan CheckBoxit valituiksi varauksen palveluiden perusteella
            foreach (var palvelu in valittuVaraus.VarauksenPalvelut)
            {
                // Etsitään CheckBox, joka vastaa tätä palvelua
                CheckBox checkBox = stackPanelPalvelut.Children.OfType<CheckBox>().FirstOrDefault(cb => cb.Content.ToString() == palvelu.Palvelu.Tuote);

                // Jos vastaava CheckBox löytyy, asetetaan se valituksi
                if (checkBox != null)
                {
                    checkBox.IsChecked = true;
                }
            }

            tabControl.SelectedIndex = 3;
        }
    }
}
