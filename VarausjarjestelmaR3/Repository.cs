using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace VarausjarjestelmaR3
{
    internal class Repository
    {





        public static void PrintVisual(FrameworkElement pageToPrint)             //Metodi, joka avaa printti-ikkunan 
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(pageToPrint, "Print Information");
            }


            //Koodi jolla luodaan printti elementistä:
            //FrameworkElement pageToPrint = GetPageToPrint();
            //Nimeä tulostettava elementti, esim window tässä tapauksessa, tähän


            //Tämä lähettää sen PrintVisual-metodille, joka avaa tulostusikkunan.
            //Repository.PrintVisual(pageToPrint);
        }
    }
}
