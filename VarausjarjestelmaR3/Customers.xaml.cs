using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
    /// Interaction logic for Customers.xaml
    /// </summary>
    public partial class Customers : UserControl
    {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";
        Customer customer;
        Repository repo;
        ObservableCollection<Customer> customers;
        
        public Customers()
        {
            InitializeComponent();
         
            repo = new Repository();
            comCustomers.ItemsSource = GetCustomers();
            comCustomer.ItemsSource = GetCustomers();
           
            this.DataContext = customer;
        }
        public ObservableCollection<Customer> GetCustomers()     //haetaan asiakkaat tietokannasta
        {
            var customers = new ObservableCollection<Customer>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd =new MySqlCommand("SELECT * FROM asiakas", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    customers.Add(new Customer
                    {
                        AsiakasID = dr.GetInt32("asiakasID"),
                        Nimi = dr.GetString("nimi"),
                        Puhelin = dr.GetString("puhelin"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Sahkoposti = dr.GetString("sahkoposti")
                    });
                }
            }

            return customers;
        }
       
        private void AddCustomer(object sender, RoutedEventArgs e)   //lisätään asiakas tietokantaan
        {
            
            var repo = new Repository();
            Customer customer = new Customer();
            customer.Nimi = Nimi.Text;
            customer.Katuosoite = Katuosoite.Text;
            customer.Postinumero = Postinumero.Text;
            customer.Postitoimipaikka = Postitoimipaikka.Text;
            customer.Puhelin = Puhelin.Text;
            customer.Sahkoposti = Sahkoposti.Text;
            
            repo.AddNewCustomer(customer);
            
        }
        private void DeleteSelectedCustomer(object sender, RoutedEventArgs e) //poistetaan asiakas
        {
            var selectedCustomer = (Customer)comCustomer.SelectedItem;

            if (selectedCustomer != null)
            {
                var result = MessageBox.Show("Haluatko varmasti poistaa asiakkaan tiedot?", "Varoitus", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    repo.DeleteCustomer(selectedCustomer);
                    comCustomer.ItemsSource = GetCustomers();
                }
            }
        }
        private void UpdateCustomer(object sender, RoutedEventArgs e) //päivitetään asiakas
        {
            var selectedCustomer = (Customer)comCustomers.SelectedItem; 

            if (selectedCustomer != null)
            {
                UpdateCustomerWin updateCustomerWindow = new UpdateCustomerWin(selectedCustomer);
                var returnValue = updateCustomerWindow.ShowDialog();

                if (returnValue == true)
                {
                    comCustomers.ItemsSource = GetCustomers();
                }
            }
        }
    }
}
