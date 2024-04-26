﻿using System;
using System.Collections.Generic;
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

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MakeAReservationBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new MakeAReservation();
        }

        private void ReservationsBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Reservations();
        }

        private void OfficesBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Offices();
        }

        private void ServicesBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Services();
        }

        private void CustomersBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Customers();
        }

        private void InvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Invoices();
        }

    }
}
