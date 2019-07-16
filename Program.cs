using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagingWebSerwer.Conections;
using System.Threading;
using ManagingWebSerwer.pages.Test;

namespace ManagingWebSerwer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            DateTime dat = DateTime.Now;

            WebFabric webFabric = new WebFabric();
            webFabric.MakeHttpSerwer();
            Test testPage = new Test(webFabric.PagesWoeker);
            webFabric.AddPage("test", testPage);

            Thread.Sleep(1000);
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Cypyright BinnarySoft Michał Mnich 2018. ");
            Console.WriteLine("----------------------------------");
            Console.WriteLine("");
            webFabric.GetMyLocalAdress();

            Console.WriteLine("\nToday is {0:d} at {0:T}.", dat);
            Console.Write("\nPress any key to continue... ");

            Console.WriteLine("");
            Console.WriteLine("----------------------------------"); //dsa
            Console.WriteLine("");

            String consoleInnput = "";
            while (consoleInnput != "close")
            {
                consoleInnput = Console.ReadLine();
            }
            webFabric.StopHttpSerwer();
        }
    }




}
