using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
    }
}
