using MySqlConnector;
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
    /// Interaction logic for Employe.xaml
    /// </summary>
    public partial class Employe : UserControl
        {
        Classes.Employee employee;
        EmployeeInfoList EmployeeInfoList = new EmployeeInfoList();
        EmployeeInfoList EmployeeInfoListForChange = new EmployeeInfoList();
        EmployeeInfoList EmployeeInfoListForDel = new EmployeeInfoList();
        ObservableCollection<Reservation> List_varaukset;

        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";

        public Employe ()
            {
            InitializeComponent();
            }

        private void UserControl_Loaded (object sender, RoutedEventArgs e)
            {

            }

        private void OnLoaded (object sender, RoutedEventArgs e)
            {
            Employee_loaded();

            }
        private void Employee_loaded ()
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select tyontekijaID, nimi from tyontekija";

                MySqlCommand command = new MySqlCommand(query, connection);
                var Reader = command.ExecuteReader();
                List<Classes.Employee> Employees = new List<Classes.Employee>();

                while (Reader.Read())
                    {
                    Employees.Add(new Classes.Employee() { Nimi = Reader.GetString("nimi"), TyontekijaID = Reader.GetInt32("tyontekijaID") });

                    }
                combListOfDelete.ItemsSource = Employees;
                combListOfChange.ItemsSource = Employees;
                combListOfHistory.ItemsSource = Employees;

                }
            UserControlAddSec.Content = EmployeeInfoList;
            }

        private void AddNewEmployee (object sender, RoutedEventArgs e)
            {

            // Suoritetaan tietokantakysely ja lisätään uusi toimipiste
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "INSERT INTO tyontekija (nimi, osoite, puhelin, kayttajaID, salasana, kaytto_oikeus, yritysID) VALUES (@Nimi, @osoite, @puhelin, @kayttajaID, @salasana, @kaytto_oikeus, 1)";

                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Nimi", EmployeeInfoList.Nimi.Text);
                command.Parameters.AddWithValue("@osoite", EmployeeInfoList.Osoite.Text);
                command.Parameters.AddWithValue("@puhelin", EmployeeInfoList.Puhelin.Text);
                command.Parameters.AddWithValue("@kayttajaID", EmployeeInfoList.KayttajaID.Text);
                command.Parameters.AddWithValue("@salasana", EmployeeInfoList.Salasana.Text);
                command.Parameters.AddWithValue("@kaytto_oikeus", EmployeeInfoList.KayttoOikeus.Text);
                try
                    {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Työntekijä lisätty onnistuneesti.");
                    EmployeeInfoList.DataContext = new Employe();
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Virhe: " + ex.Message);
                    }
                }

            Employee_loaded();

            }

        private void ShowEmployeeBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select * from tyontekija where tyontekijaID = @tyontekijaID ";


                MySqlCommand command = new MySqlCommand(query, connection);
                if (changeSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@tyontekijaID", combListOfChange.SelectedValue);
                    }
                else if (deleteSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@tyontekijaID", combListOfDelete.SelectedValue);
                    }
                else if (historySection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@tyontekijaID", combListOfHistory.SelectedValue);
                    }

                command.ExecuteNonQuery();
                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    employee = new Classes.Employee() { Nimi = Reader.GetString("nimi"), Osoite = Reader.GetString("osoite"), Puhelin = Reader.GetString("puhelin"), KayttajaID = Reader.GetString("kayttajaID"), Salasana = Reader.GetString("salasana"), KayttoOikeus = Reader.GetInt32("kaytto_oikeus") };
                    }
                if (changeSection.IsSelected)
                    {
                    EmployeeInfoListForChange.DataContext = employee;
                    toimiposteContentControlsec.Content = EmployeeInfoListForChange;
                    ChangeBtn.Visibility = Visibility.Visible;
                    }
                else if (deleteSection.IsSelected) 
                    {
                    EmployeeInfoListForDel.DataContext = employee;
                    toimiposteContentControl.Content = EmployeeInfoListForDel;
                    deleteBtn.Visibility = Visibility.Visible;
                    }
                }
            }

        private void DeleteBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                MessageBoxResult result = MessageBox.Show("Haluatko varmasti poistaa tämän työntekijän?", "Vahvista poisto", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                    {
                    // Poista työntekijä tietokannasta
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    try
                        {
                        // Siirrä työntekijän varaukset pääkäyttäjälle
                        using (MySqlCommand transferCommand = new MySqlCommand())
                            {
                            transferCommand.Connection = connection;
                            transferCommand.Transaction = transaction;
                            transferCommand.CommandText = "UPDATE asiakkaan_varaus SET tyontekijaID = @AdminUserID WHERE tyontekijaID = @EmployeeID";
                            transferCommand.Parameters.AddWithValue("@AdminUserID", 1);
                            transferCommand.Parameters.AddWithValue("@EmployeeID", combListOfDelete.SelectedValue);
                            transferCommand.ExecuteNonQuery();
                            }

                        //poistetaan työntekijä työpisteestä
                        using (MySqlCommand deleteEmployeeCommand = new MySqlCommand())
                            {
                            deleteEmployeeCommand.Connection = connection;
                            deleteEmployeeCommand.Transaction = transaction;
                            deleteEmployeeCommand.CommandText = "DELETE FROM toimipisteen_tyontekija WHERE tyontekijaID = @EmployeeID";
                            deleteEmployeeCommand.Parameters.AddWithValue("@EmployeeID", combListOfDelete.SelectedValue);
                            deleteEmployeeCommand.ExecuteNonQuery();
                            }

                        // Lopuksi poista työntekijä
                        using (MySqlCommand deleteEmployeeCommand = new MySqlCommand())
                            {
                            deleteEmployeeCommand.Connection = connection;
                            deleteEmployeeCommand.Transaction = transaction;
                            deleteEmployeeCommand.CommandText = "DELETE FROM tyontekija WHERE tyontekijaID = @EmployeeID";
                            deleteEmployeeCommand.Parameters.AddWithValue("@EmployeeID", combListOfDelete.SelectedValue);
                            deleteEmployeeCommand.ExecuteNonQuery();
                            }

                        transaction.Commit();
                        MessageBox.Show("Työntekijä on poistettu onnistuneesti.", "Poisto onnistui", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    catch (Exception ex)
                        {
                        MessageBox.Show("Virhe: " + ex.Message);
                        transaction.Rollback();
                        }
                    }
                else
                    {
                    MessageBox.Show("Poisto peruutettu.", "Toiminto peruutettu", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                } 

            
            EmployeeInfoListForDel.DataContext = new Classes.Employee();
            Employee_loaded();
            }

        private void Chnage (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                
                string query = "UPDATE tyontekija SET nimi = @nimi, osoite = @osoite, puhelin = @puhelin, kayttajaID = @kayttajaID, salasana = @salasana, kaytto_oikeus = @kaytto_oikeus  WHERE tyontekijaID = @tyontekijaID;";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nimi", EmployeeInfoListForChange.Nimi.Text);
                command.Parameters.AddWithValue("@osoite", EmployeeInfoListForChange.Osoite.Text);
                command.Parameters.AddWithValue("@puhelin", EmployeeInfoListForChange.Puhelin.Text);
                command.Parameters.AddWithValue("@kayttajaID", EmployeeInfoListForChange.KayttajaID.Text);
                command.Parameters.AddWithValue("@salasana", EmployeeInfoListForChange.Salasana.Text);
                command.Parameters.AddWithValue("@kaytto_oikeus", EmployeeInfoListForChange.KayttoOikeus.Text);
                command.Parameters.AddWithValue("@tyontekijaID", combListOfChange.SelectedValue);
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
            EmployeeInfoListForChange.DataContext = new Classes.Employee();
            Employee_loaded();
            }

        private void ShowHestory (object sender, RoutedEventArgs e)
            {
            Classes.ReservationForEmplyoeeHis reservation = new Classes.ReservationForEmplyoeeHis();
            Repository repo = new Repository();

            List_varaukset = repo.GetAllReservationsforEmployee(Convert.ToInt32(combListOfHistory.SelectedValue));
            Varaukset.ItemsSource = List_varaukset;
           

            }
        }


    }