using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using ManagingWebSerwer.Compilation;
using ManagingWebSerwer.pages;

namespace SiouxTests
{
    [TestFixture]
    public class ComplerTests
    {
        private int _dupolicznik;

        public ComplerTests()
        {
            Directory.Delete(Path.Combine(Path.GetTempPath(), "TempScriptDll"), true);
            _dupolicznik = 0; //Jezeli kompilujemy pare razy ten sam assembly to czasem chcemy zeby stara zostala usunieta (dla testów)
            // i dupolicznik tworzy katalog obok z numerkiem
        }




        [Test]
        public void AtomicCompilationTest() //TEST Sprawdza czy nie da śie kompilować wielu identycznych plików
        {
            //Tworzenie Asembly następnie twożenie identycznego w rmach testów unikatowości kompilacji
            Console.WriteLine("Rozpoczynamy Test");
            var c = new Compiler(Environment.CurrentDirectory + "\\Scripts\\");
            Assembly AsemblyConector = c.CompileExecutable("DummyScript.cs");

            Console.WriteLine("Pruba kompilacji identycznego pliku");
            Assembly AsemblyConector2 = c.CompileExecutable("DummyScript.cs");

            Assert.True(c.FileStorage.Count() < 2);
            Console.WriteLine("Kompilacja sie nie zaczeła plik indentyczny");
            //AsemblyConector.

            //File.Delete();
            //File.Copy(AsemblyConector.Location,);
            File.Move(AsemblyConector.Location, AsemblyConector.Location + _dupolicznik);
            _dupolicznik++;
        }

        [Test]
        public void CompilationMannyConnectedFilesTest()
        //TEST Sprawdza kompilacje wielu powiązanych plików oraz czy ich instancje zostąły dobrze stwozone
        {
            var c = new Compiler(Environment.CurrentDirectory + "\\Scripts\\");
            Console.WriteLine("Compilation DummyScript.cs");
            Assembly AsemblyConector2 = c.CompileExecutable("DummyScript.cs");


            //Twożenie i odpalanie instancji Skryptu DummyScript.cs
            IEnumerable<Type> typeName2 = AsemblyConector2.GetTypes().Where(e => e.BaseType == (typeof(BasePage)));
            try
            {
                object obj2 = c.InstanceGenerator(AsemblyConector2);
                object output2 = c.GetType(AsemblyConector2).GetMethod("tesrRunn").Invoke(obj2, new object[] { });
                Assert.NotNull(obj2);
                File.Move(AsemblyConector2.Location, AsemblyConector2.Location + _dupolicznik);
                _dupolicznik++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Class Cross");
            }
        }

        [Test]
        public void CompilationTest()
        //TEST Sprawdza kompilacje wielu plików oraz czy ich instancje zostąły dobrze stwozone
        {
            //Tworzenie Asembly następnie twożenie identycznego w rmach testów unikatowości kompilacji
            Console.WriteLine("Kompilacja DummyScript.cs");
            var c = new Compiler(Environment.CurrentDirectory + "\\Scripts\\");
            Assembly AsemblyConector = c.CompileExecutable("DummyScript.cs");

            Console.WriteLine("Kompilacja SuperDummyScript.cs");
            Assembly AsemblyConector2 = c.CompileExecutable("SuperDummyScript.cs");

            Assert.True(c.FileStorage.Count() == 2);
            Console.WriteLine("Kompilacja obu plików przeszła pomyślnie");

            //Twożenie i odpalanie instancji Skryptu SuperDummyScript.cs
            IEnumerable<Type> typeName = AsemblyConector.GetTypes().Where(e => e.BaseType == (typeof(BasePage)));
            object obj = c.InstanceGenerator(AsemblyConector);
            object output = c.GetType(AsemblyConector).GetMethod("DummyFunction").Invoke(obj, new object[] { });
            Assert.NotNull(obj);

            //Twożenie i odpalanie instancji Skryptu DummyScript.cs
            IEnumerable<Type> typeName2 = AsemblyConector2.GetTypes().Where(e => e.BaseType == (typeof(BasePage)));
            object obj2 = c.InstanceGenerator(AsemblyConector2);
            object output2 = c.GetType(AsemblyConector2).GetMethod("MstrTGanGstaRap").Invoke(obj2, new object[] { });
            Assert.NotNull(obj2);
            File.Move(AsemblyConector.Location, AsemblyConector.Location + _dupolicznik);
            _dupolicznik++;
            File.Move(AsemblyConector2.Location, AsemblyConector2.Location + _dupolicznik);
            _dupolicznik++;
        }

        [Test]
        public void ExceptionTest()
        {
            try
            {
                var c = new Compiler(Environment.CurrentDirectory + "\\Scripts\\");
                Console.WriteLine("Kompilacja SuperDummyScriptCoGoNieMa.cs");

                Assembly AsemblyConector2 = c.CompileExecutable("SuperDummyScriptCoGoNieMa.cs");
                File.Move(AsemblyConector2.Location, AsemblyConector2.Location + _dupolicznik);
                _dupolicznik++;
            }
            catch (CompilerException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}