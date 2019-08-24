using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagingWebSerwer.Conections;
using System.Threading;
using ManagingWebSerwer.pages.Test;
using System.Reflection;
using ManagingWebSerwer.Compilation;
using ManagingWebSerwer.pages;

namespace ManagingWebSerwer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            var colorConsole = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Generating Pages: ----------------------------------");
            DateTime dat = DateTime.Now;

            WebFabric webFabric = new WebFabric();
            webFabric.MakeHttpSerwer();

            Engine engine = new Engine();
            engine.GeneratePages();

            foreach(KeyValuePair<string, Page> intem in engine.CompilatedPages){
                webFabric.AddPage(intem.Key, intem.Value);
            }
            Console.WriteLine("----------------------------------------------------");
            Console.ForegroundColor = colorConsole;
            Console.WriteLine("Pages generated sucesfuly ...");
            Console.WriteLine("Serwer will start in: 3");
          
            Thread.Sleep(2000);
            Console.WriteLine("Serwer will start in: 2");
            Thread.Sleep(2000);
            Console.WriteLine("Serwer will start in: 1");
            Thread.Sleep(2000);
            Console.WriteLine("Serwer will start in: 0");
            Console.Clear();
            Console.WriteLine("Serwer Starting");
            Thread.Sleep(2000);
            
            Console.Clear();


            Console.WriteLine("     _______. __    ______    __    __  ___   ___ ");
            Console.WriteLine("    /       ||  |  /  __  \\  |  |  |  | \\  \\ /  / ");
            Console.WriteLine("   |   (----`|  | |  |  |  | |  |  |  |  \\  V  /  ");
            Console.WriteLine("    \\   \\    |  | |  |  |  | |  |  |  |   >   <   ");
            Console.WriteLine(".----)   |   |  | |  `--'  | |  `--'  |  /  .  \\  ");
            Console.WriteLine("|_______/    |__|  \\______/   \\______/  /__/ \\__\\ ");

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
