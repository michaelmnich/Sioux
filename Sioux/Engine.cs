using ManagingWebSerwer.Compilation;
using ManagingWebSerwer.pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManagingWebSerwer
{
    public class Engine
    {
        public Dictionary<string, Page> CompilatedPages { get; private set; }

        public Engine()
        {
            CompilatedPages = new Dictionary<string, Page>();
           
        }

        public void GeneratePages()
        {
            string dir = Environment.CurrentDirectory + "\\pages\\";
            string[] fileEntries = Directory.GetFiles(dir);
            foreach (string fileName in fileEntries)
            {
                CreateAndCompilePages(dir, fileName);
            }

            
           
        }

        private void CreateAndCompilePages(string dir, string file)
        {
            var c = new Compiler(dir);
            Console.WriteLine("Compilation: "+ file);
            Assembly AsemblyConector = c.CompileExecutable(file); 

            IEnumerable<Type> typeName2 = AsemblyConector.GetTypes().Where(e => e.BaseType == (typeof(BasePage)));
            try
            {
                object obj = c.InstanceGenerator(AsemblyConector);
                object output = c.GetType(AsemblyConector).GetMethod("GetContent").Invoke(obj, new object[] { });
                object uri = c.GetType(AsemblyConector).GetMethod("GetUri").Invoke(obj, new object[] { });
                CompilatedPages.Add(uri.ToString(), (Page) obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Class Cross");
            }

        }

    }
}
