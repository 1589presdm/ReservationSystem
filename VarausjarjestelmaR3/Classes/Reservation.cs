using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace VarausjarjestelmaR3.Classes
{
    public class Reservation
    {
        public int VarausID { get; set; }
        public DateTime VarausAlkaa { get; set; }
        public DateTime VarausPaattyy { get; set; }
        public DateTime Varauspaiva { get; set; }
        public Room Huone {  get; set; }
        public String Lisatiedot { get; set; }
        public Customer Asiakas { get; set; }
        public Employee Tyontekija { get; set; }
        public ObservableCollection<ReservationServices> VarauksenPalvelut { get; set; }
    }

    public class ReservationServices
    {
        public int PalveluvarausID { get; set; }
        public Service Palvelu {  get; set; }
        public int Kpl {  get; set; }
        public Reservation Varaus { get; set; }
    }
}
