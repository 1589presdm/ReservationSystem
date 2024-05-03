using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace VarausjarjestelmaR3
    {
    /// <summary>
    /// Interaction logic for Offices.xaml
    /// </summary>
    public partial class Offices : UserControl
        {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";

        public List<Classes.Office> toimipistet = new List<Classes.Office>() { };
        Classes.Office toimipiste;
        Classes.Room room;
        List<Classes.Room> rooms = new List<Classes.Room>();
        OffficeInfoList OffficeInfoListForChng = new OffficeInfoList();
        OffficeInfoList OffficeInfoListForAdd = new OffficeInfoList();
        RoomInfoList RoomInfoList = new RoomInfoList();
        RoomInfoList RoomInfoListForChange = new RoomInfoList();

        public Offices ()
            {
            InitializeComponent();

            }

        private void Check (object sender, RoutedEventArgs e)
            {

            }


        private void AddNewOffice (object sender, RoutedEventArgs e)
            {
            bool check = checker();
            if (check)
                {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                    connection.Open();
                    string query = "INSERT INTO toimipiste (toimipiste_nimi, paikkakunta, katuosoite, postinumero, postitoimipaikka, puhelin, yritysID) VALUES (@Nimi, @Paikkakunta, @Katuosoite, @Postinumero, @Postitoimipaikka, @Puhelin, 1)";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Nimi", OffficeInfoListForAdd.txtToimipisteNimi.Text);
                    command.Parameters.AddWithValue("@Paikkakunta", OffficeInfoListForAdd.txtPaikkakunta.Text);
                    command.Parameters.AddWithValue("@Katuosoite", OffficeInfoListForAdd.txtKatuosoite.Text);
                    command.Parameters.AddWithValue("@Postinumero", OffficeInfoListForAdd.txtPostinumero.Text);
                    command.Parameters.AddWithValue("@Postitoimipaikka", OffficeInfoListForAdd.txtPostitoimipaikka.Text);
                    command.Parameters.AddWithValue("@Puhelin", OffficeInfoListForAdd.txtPuhelin.Text);


                    try
                        {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Toimipiste lisätty onnistuneesti.");
                        }
                    catch (Exception ex)
                        {
                        MessageBox.Show("Virhe: " + ex.Message);
                        }

                    }

                }
            else
                {
                MessageBox.Show("täytä puutteet!");
                }

            }
        private bool checker ()
            {
            return true;
            }


        private void DeleteBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "delete from toimipiste where toimipisteID = @toimipisteID ";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@toimipisteID", combListOfDelete.SelectedValue);
                command.ExecuteNonQuery();

                MessageBox.Show("Toimipiste " + combListOfDelete.Text + " poistettu ");
                }
            }

        private void onLoaded (object sender, RoutedEventArgs e)
            {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select toimipisteID, toimipiste_nimi from toimipiste";

                MySqlCommand command = new MySqlCommand(query, connection);
                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    toimipistet.Add(new Classes.Office() { ToimipisteID = Reader.GetInt32("toimipisteID"), ToimipisteNimi = Reader.GetString("toimipiste_nimi") });

                    }
                combListOfDelete.ItemsSource = toimipistet;
                combListOfChange.ItemsSource = toimipistet;
                RoomInfoList.ToimipisteNimi.ItemsSource = toimipistet;
                RoomInfoListForChange.ToimipisteNimi.ItemsSource= toimipistet;
                }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string queryForRooms = "select huoneen_numeroID, nimi from huoneet";


                MySqlCommand command = new MySqlCommand(queryForRooms, connection);
                command = new MySqlCommand(queryForRooms, connection);
                var ReaderForRooms = command.ExecuteReader();
                while (ReaderForRooms.Read())
                    {
                    rooms.Add(new Classes.Room() { HuoneenNumeroID = ReaderForRooms.GetInt32("huoneen_numeroID"), Nimi = ReaderForRooms.GetString("nimi") });
                    }
                RoomcombListOfDelete.ItemsSource = rooms;
                RoomcombListOfChange.ItemsSource= rooms;

                }

            UserControlAddSec.Content = OffficeInfoListForAdd;
            RoomUserControlAddSec.Content = RoomInfoList;

            }

        private void ShowOfficeInfo (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select * from toimipiste where toimipisteID = @toimipisteID ";


                MySqlCommand command = new MySqlCommand(query, connection);
                if (changeSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@toimipisteID", combListOfChange.SelectedValue);
                    }
                else if (deleteSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@toimipisteID", combListOfDelete.SelectedValue);
                    }

                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    toimipiste = new Classes.Office() { ToimipisteNimi = Reader.GetString("toimipiste_nimi"), Katuosoite = Reader.GetString("katuosoite"), Paikkakunta = Reader.GetString("paikkakunta"), Postinumero = Reader.GetString("postinumero"), Postitoimipaikka = Reader.GetString("postitoimipaikka"), Puhelin = Reader.GetString("puhelin") };
                    }



                if (changeSection.IsSelected)
                    {
                    OffficeInfoListForChng.DataContext = toimipiste;
                    toimiposteContentControlsec.Content = OffficeInfoListForChng;
                    ChangeBtn.Visibility = Visibility.Visible;
                    }
                else if (deleteSection.IsSelected)
                    {
                    OffficeInfoList offficeInfoList = new OffficeInfoList();
                    offficeInfoList.DataContext = toimipiste;
                    toimiposteContentControl.Content = offficeInfoList;
                    deleteBtn.Visibility = Visibility.Visible;
                    }

                }
            }

        private void Chnage (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "UPDATE toimipiste SET toimipiste_nimi = @toimipiste_nimi, katuosoite = @katuosoite, paikkakunta = @paikkakunta, postinumero = @postinumero, postitoimipaikka = @postitoimipaikka, puhelin = @puhelin  WHERE toimipisteID = @toimipisteID;";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@toimipiste_nimi", OffficeInfoListForChng.txtToimipisteNimi.Text);
                command.Parameters.AddWithValue("@Paikkakunta", OffficeInfoListForChng.txtPaikkakunta.Text);
                command.Parameters.AddWithValue("@Katuosoite", OffficeInfoListForChng.txtKatuosoite.Text);
                command.Parameters.AddWithValue("@Postinumero", OffficeInfoListForChng.txtPostinumero.Text);
                command.Parameters.AddWithValue("@Postitoimipaikka", OffficeInfoListForChng.txtPostitoimipaikka.Text);
                command.Parameters.AddWithValue("@Puhelin", OffficeInfoListForChng.txtPuhelin.Text);
                command.Parameters.AddWithValue("@toimipisteID", combListOfChange.SelectedValue);
                command.ExecuteNonQuery();

                MessageBox.Show("muokattu!");
                LoadedEvent.Equals(true);
                }
            }
        //---------------------------------------------------------------------------------------------------

        private void AddNewRoom (object sender, RoutedEventArgs e)
            {


            bool check = checker();
            if (check)
                {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                    connection.Open();
                    string query = "INSERT INTO huoneet (nimi, hinta, alv_prosentti, hlo_maara, toimipisteID) VALUES (@Nimi, @hinta, @alv_prosentti, @hlo_maara, @toimipisteID)";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Nimi", RoomInfoList.RoomNimi.Text);
                    command.Parameters.AddWithValue("@hinta", RoomInfoList.Hinta.Text);
                    command.Parameters.AddWithValue("@alv_prosentti", RoomInfoList.AlvProsentti.Text);
                    command.Parameters.AddWithValue("@hlo_maara", RoomInfoList.HloMaara.Text);
                    command.Parameters.AddWithValue("@toimipisteID", RoomInfoList.ToimipisteNimi.SelectedValue);


                    try
                        {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Huone lisätty onnistuneesti.");
                        }
                    catch (Exception ex)
                        {
                        MessageBox.Show("Virhe: " + ex.Message);
                        }

                    }

                }
            else
                {
                MessageBox.Show("täytä puutteet!");
                }


            }

        private void RoomInfo (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select * from huoneet where huoneen_numeroID = @huoneen_numeroID ";


                MySqlCommand command = new MySqlCommand(query, connection);
                if (RoomDeleteSec.IsSelected)
                    {
                    command.Parameters.AddWithValue("@huoneen_numeroID", RoomcombListOfDelete.SelectedValue);
                    }
                else if (RoomChangeSec.IsSelected)
                    {
                    command.Parameters.AddWithValue("@huoneen_numeroID", RoomcombListOfChange.SelectedValue);
                    }

                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    room = new Classes.Room() { Nimi = Reader.GetString("nimi"), Hinta = Reader.GetFloat("hinta"), AlvProsentti = Reader.GetFloat("alv_prosentti"), HloMaara = Reader.GetInt32("hlo_maara"), Toimipiste = new Classes.Office() { ToimipisteID = Reader.GetInt32("toimipisteID") } };
                    }
                if (RoomDeleteSec.IsSelected)
                    {
                    RoomInfoList.DataContext = room;
                    RoomInfoList.ToimipisteNimi.SelectedValue = room.Toimipiste.ToimipisteID;
                    HuoneContentControl.Content = RoomInfoList;
                    RoomDeleteBtn.Visibility = Visibility.Visible;
                    }

                else if (RoomChangeSec.IsSelected)
                    {
                    RoomInfoListForChange.DataContext = room;
                    RoomInfoListForChange.ToimipisteNimi.SelectedValue = room.Toimipiste.ToimipisteID;
                    HuoneContentControlForChange.Content = RoomInfoListForChange;
                    RoomChangeBtn.Visibility = Visibility.Visible;
                    }

                }
            }
        private void DeleteRoomBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "delete from huoneet where huoneen_numeroID = @huoneen_numeroID ";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@huoneen_numeroID", RoomcombListOfDelete.SelectedValue);
                command.ExecuteNonQuery();

                MessageBox.Show("Huone " + RoomcombListOfDelete.Text + " poistettu ");
                }
            }

        private void ChangeRoomBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "UPDATE huoneet SET nimi = @nimi, hinta = @hinta, alv_prosentti = @alv_prosentti, hlo_maara = @hlo_maara, toimipisteID = @toimipisteID where huoneen_numeroID = @huoneen_numeroID ";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nimi", RoomInfoListForChange.RoomNimi.Text);
                command.Parameters.AddWithValue("@hinta", RoomInfoListForChange.Hinta.Text);
                command.Parameters.AddWithValue("@alv_prosentti", RoomInfoListForChange.AlvProsentti.Text);
                command.Parameters.AddWithValue("@hlo_maara", RoomInfoListForChange.HloMaara.Text);
                command.Parameters.AddWithValue("@toimipisteID", RoomInfoListForChange.ToimipisteNimi.SelectedValue);
                command.Parameters.AddWithValue("@huoneen_numeroID", RoomcombListOfChange.SelectedValue);
                command.ExecuteNonQuery();

                MessageBox.Show("muokattu!");

                }
            }
        }
    //public class toimipiste
    //    {
    //    public int ToimipisteID { get; set; }
    //    public string ToimipisteNimi { get; set; }
    //    public string Paikkakunta { get; set; }
    //    public string Katuosoite { get; set; }
    //    public string Postinumero { get; set; }
    //    public string Postitoimipaikka { get; set; }
    //    public string Puhelin { get; set; }



    //    }
    }







































