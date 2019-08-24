using System;



namespace SiouxComon
{
    public class LogWriter
    {
        public void Error(string v)
        {
            var colorConsole =  Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: "+v);
            Console.ForegroundColor = colorConsole;
        }

        public void Info(string v)
        {
            Console.WriteLine(v);
        }
    }
}
