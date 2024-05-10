﻿using System;
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
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for MakeAReservation.xaml
    /// </summary>
    public partial class MakeAReservation : UserControl
    {
        Repository repo = new Repository();

        public MakeAReservation()
        {
            InitializeComponent();

            var customers = repo.GetAllCustomers();

            ComMunicipality.ItemsSource = GetDistinctMunicipalities();

            ComCustomer.ItemsSource = customers;

        }

        private void MunicipalityCom_Changed(object sender, RoutedEventArgs e)
        {

            var selected = ComMunicipality.SelectedItem as Office;

            ComOffices.ItemsSource = repo.GetAllOfficesByMunicipality(selected.Paikkakunta);

        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            Search();

        }

        private void Search()
        {
            var selected = ComOffices.SelectedItem as Office;

            var rooms = repo.GetAllRoomsByOffice(selected.ToimipisteID);

            var reservations = repo.GetAllReservationsForRoomsAndTime(rooms);

            var baseAvailability = repo.CreateBaseAvailability(rooms, StartDateP.SelectedDate.Value, EndDateP.SelectedDate.Value);

            var availability = repo.CreateAvailability(baseAvailability, reservations);

            foreach (var room in rooms)
            {
                if (availability.Any(x => !x.Available && x.Room.HuoneenNumeroID == room.HuoneenNumeroID))
                {
                    room.Color = Brushes.Red;
                    room.CanBeReserved = false;
                }
                else
                {
                    room.Color = Brushes.Green;
                    room.CanBeReserved = true;
                }
            }

            OfficesLB.ItemsSource = rooms;

            ServicesLB.ItemsSource = repo.GetAllServicesByOffice(selected.ToimipisteID);
        }

        private void ReserveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OfficesLB.SelectedItem == null)
            {
                return;
            }

            if (((Room)OfficesLB.SelectedItem).CanBeReserved == false)
            {
                MessageBox.Show("Huonetta ei voi varata valitulle ajanjaksolle.");
                return;
            }

            var reservation = new Reservation();
            reservation.Asiakas = new Customer();
            reservation.Tyontekija = new Employee();

            reservation.VarausAlkaa = StartDateP.SelectedDate.Value;
            reservation.VarausPaattyy = EndDateP.SelectedDate.Value;
            reservation.Huone = ((Room)OfficesLB.SelectedItem);
            reservation.Varauspaiva = DateTime.Now;
            reservation.Lisatiedot = AdditionalInfoTB.Text;
            reservation.Asiakas = ((Customer)ComCustomer.SelectedItem);
            reservation.Tyontekija.TyontekijaID = 2;

            repo.AddReservation(reservation);

            foreach (var item in ServicesLB.SelectedItems)
            {
                var reservationService = new ReservationService();
                reservationService.Palvelu = (Service)item;
                reservationService.VarausID = reservation.VarausID;
                reservationService.Kpl = 1;
                
                repo.AddReservationServices(reservationService);
            }
            

            MessageBox.Show("Varaus tehty");

            Search();

        }

        /// <summary>
        /// Hakee uniikit paikkakuntien nimet
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Office> GetDistinctMunicipalities()
        {
            ObservableCollection<Office> distinctNames = repo.GetAllOffices();

            var distinct = new ObservableCollection<Office>();

            foreach (var distinctName in distinctNames)
            {
                if (distinct.Any(x=>x.Paikkakunta == distinctName.Paikkakunta))
                {
                    continue;
                }

                distinct.Add(distinctName);
            }        

            return distinct;
        }

    }
}
