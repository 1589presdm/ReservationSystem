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
        CustomerInfoList customerInfoList = new CustomerInfoList();
        CustomerInfoList customerInfoListForChang = new CustomerInfoList();
        CustomerInfoList customerInfoListForDel = new CustomerInfoList();
        ObservableCollection<Customer> customers;
        ObservableCollection<Reservation> List_varaukset;


        public Customers ()
            {
            InitializeComponent();

            repo = new Repository();


            this.DataContext = customer;
            }
        public ObservableCollection<Customer> GetCustomers ()     //haetaan asiakkaat tietokannasta
            {
            var customers = new ObservableCollection<Customer>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakas", conn);

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

        private void OnLoaded (object sender, RoutedEventArgs e)
            {
            Laod_Customers();

            }
        private void Laod_Customers ()
            {
            ObservableCollection<Customer> customers = repo.GetAllCustomers();
            combListOfDelete.ItemsSource = customers;
            combListOfChange.ItemsSource = customers;
            combListOfHistory.ItemsSource = customers;
            UserControlAddSec.Content = customerInfoList;
            }


        private void AddNewCustomer (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
                {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO asiakas (nimi, puhelin, katuosoite, postinumero, postitoimipaikka, sahkoposti) VALUES (@nimi, @puhelin, @katuosoite, @postinumero, @postitoimipaikka, @sahkoposti)", conn);

                cmd.Parameters.AddWithValue("@nimi", customerInfoList.Nimi.Text);
                cmd.Parameters.AddWithValue("@puhelin", customerInfoList.Puhelin.Text);
                cmd.Parameters.AddWithValue("@katuosoite", customerInfoList.Katuosoite.Text);
                cmd.Parameters.AddWithValue("@postinumero", customerInfoList.Postinumero.Text);
                cmd.Parameters.AddWithValue("@postitoimipaikka", customerInfoList.Postitoimipaikka.Text);
                cmd.Parameters.AddWithValue("@sahkoposti", customerInfoList.Sahkoposti.Text);
                try
                    {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Asiakas lisätty onnistuneesti.");
                    customerInfoList.DataContext = new Customer();
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Virhe: " + ex.Message);
                    }
                Laod_Customers();

                }
            }

        private void ShowCustomerBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select * from asiakas where asiakasID = @asiakasID ";


                MySqlCommand command = new MySqlCommand(query, connection);
                if (changeSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@asiakasID", combListOfChange.SelectedValue);
                    }
                else if (deleteSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@asiakasID", combListOfDelete.SelectedValue);
                    }
                else if (historySection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@asiakasID", combListOfHistory.SelectedValue);
                    }

                command.ExecuteNonQuery();
                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    customer = new Classes.Customer() { Nimi = Reader.GetString("nimi"), Katuosoite = Reader.GetString("katuosoite"), Puhelin = Reader.GetString("puhelin"), Postinumero = Reader.GetString("postinumero"), Postitoimipaikka = Reader.GetString("postitoimipaikka"), Sahkoposti = Reader.GetString("sahkoposti") };
                    }
                if (changeSection.IsSelected)
                    {
                    customerInfoListForChang.DataContext = customer;
                    toimiposteContentControlsec.Content = customerInfoListForChang;
                    ChangeBtn.Visibility = Visibility.Visible;
                    }
                else if (deleteSection.IsSelected)
                    {
                    customerInfoListForDel.DataContext = customer;
                    toimiposteContentControl.Content = customerInfoListForDel;
                    deleteBtn.Visibility = Visibility.Visible;
                    }
                }

            }

        private void DeleteBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "delete from asiakas where asiakasID = @asiakasID;";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@asiakasID", combListOfDelete.SelectedValue);
                try
                    {
                    command.ExecuteNonQuery();

                    MessageBox.Show("Asiakas " + combListOfDelete.Text + " poistettu ");
                    this.DataContext = null;
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Virhe: " + ex.Message);

                    }
                }
            customerInfoListForDel.DataContext = new Classes.Customer();
            Laod_Customers();


            }

        private void Chnage (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();

                string query = "UPDATE asiakas SET nimi = @nimi, katuosoite = @katuosoite, puhelin = @puhelin, postitoimipaikka = @postitoimipaikka, postinumero = @postinumero, sahkoposti = @sahkoposti  WHERE asiakasID = @asiakasID;";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nimi", customerInfoListForChang.Nimi.Text);
                command.Parameters.AddWithValue("@katuosoite", customerInfoListForChang.Katuosoite.Text);
                command.Parameters.AddWithValue("@puhelin", customerInfoListForChang.Puhelin.Text);
                command.Parameters.AddWithValue("@postitoimipaikka", customerInfoListForChang.Postitoimipaikka.Text);
                command.Parameters.AddWithValue("@postinumero", customerInfoListForChang.Postinumero.Text);
                command.Parameters.AddWithValue("@sahkoposti", customerInfoListForChang.Sahkoposti.Text);
                command.Parameters.AddWithValue("@asiakasID", combListOfChange.SelectedValue);
                try
                    {
                    command.ExecuteNonQuery();
                    MessageBox.Show("muokattu!");
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Virhe: " + ex.Message);
                    }
                }
            customerInfoListForChang.DataContext = new Classes.Customer();
            Laod_Customers();

            }

        private void ShowHestory (object sender, RoutedEventArgs e)
            {
            Classes.ReservationForEmplyoeeHis reservation = new Classes.ReservationForEmplyoeeHis();
            Repository repo = new Repository();

            List_varaukset = repo.GetAllReservationsforCustomer(Convert.ToInt32(combListOfHistory.SelectedValue));
            Varaukset.ItemsSource = List_varaukset;

            }
        }
    }
