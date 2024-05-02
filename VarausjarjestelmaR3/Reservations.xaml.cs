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
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for Reservations.xaml
    /// </summary>
    public partial class Reservations : UserControl
    {
        public Reservations()
        {
            InitializeComponent();

            Repository repo = new Repository();

            var varaukset = repo.GetAllReservations();

            dgVaraukset.DataContext = varaukset;
            dgVaraukset.ItemsSource = varaukset;

            //Repository-metodien testailua:
            //int yritysID = 1;
            //int toimipisteID = 1;
            //int huoneID = 1;
            //int palveluID = 1;
            //int tyontekijaID = 1;
            //int asiakasID = 1;
            //int varausID = 1;
            //int laskunumero = 1;

            //repo.GetCompany(yritysID);
            //repo.GetAllCompanies();
            //repo.GetOffice(toimipisteID);
            //repo.GetAllOffices();
            //repo.GetRoom(huoneID);
            //repo.GetAllRooms();
            //repo.GetService(palveluID);
            //repo.GetAllServices();
            //repo.GetEmployee(tyontekijaID);
            //repo.GetAllEmployees();
            //repo.GetCustomer(asiakasID);
            //repo.GetAllCustomers();
            //repo.GetReservation(varausID);
            //repo.GetReservationServices(varausID);
            //repo.GetAllReservations();
            //repo.GetInvoice(laskunumero);
            //repo.GetAllInvoices();
        }
    }
}
