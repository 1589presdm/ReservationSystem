﻿using MySqlConnector;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VarausjarjestelmaR3
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";
        private MainView mainView = new MainView();


        public MainWindow ()
            {
            InitializeComponent();
            }

        /// <summary>
        /// Väliaikainen "ohita kirjautuminen", poistetaan myöhemmin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {

            MainView main = new MainView();
            main.Show();
            this.Close();
        }

        private void makeconnetion(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "select kayttajaID, salasana from tyontekija where kayttajaID = @user and salasana = @pass";
                MySqlCommand mySqlCommand = new MySqlCommand(query, connection);
                mySqlCommand.Parameters.AddWithValue("@user", userName.Text);
                mySqlCommand.Parameters.AddWithValue("@pass", Pass.Text);
                var Reader = mySqlCommand.ExecuteReader();

                try
                {

                    Reader.Read();
                    {
                        string user = Reader.GetString("kayttajaID");
                        string pass = Reader.GetString("salasana");

                        if (user == userName.Text && pass == Pass.Text)
                        {
                            mainView.ShowDialog();  
                        }
                    }
                }

                catch
                {
                    MessageBox.Show("Käyttäjätunnus tai salasana on väärin ");
                }
            }

        }
    } 
    }
