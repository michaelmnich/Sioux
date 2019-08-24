using SiouxComon;
using SiouxComon.FrameworkExtensions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Design;

namespace ManagingWebSerwer.Compilation
{
    class Compiler : ICompiler
    {
        private static readonly LogWriter Log = new LogWriter();
        private const string TempDcriptdll = "TempScriptDll";
        private readonly List<string> _fileNotCompiledStorage;

        private string _pathDirectory;

        static Compiler()
        {
            //Log.AddPrivateSink(FileSink.GetCustomWithDateSubfolder("compiler"));
            //Log2.AddPrivateSink(FileSink.GetCustomWithDateSubfolder("ScriptCode" + DateTime.Now.ToLongTimeString().Replace(":", "")));
        }

        public Compiler(String directory)
        {
            CacheCompilation = new List<Assembly>();
            FileStorage = new List<FileInfo>();
            _pathDirectory = directory;
            _fileNotCompiledStorage = new List<string>();
        }

        private List<Assembly> CacheCompilation { get; set; }
        public List<FileInfo> FileStorage { get; private set; }


        /// <summary>
        ///     Przeładowana metoda kompilatora zastosowanie w rekurencji patrz wywołanie
        /// </summary>
        public Assembly CompileExecutable(String sourceName)
        {
            return CompileExecutable(sourceName, true);
        }


        /// <summary>
        ///    Tworzy instancję obiektu określonego typu za pomocą konstruktora, 
        ///    Konstyktor jest to zawsze konstruktor który najlepiej odpowiada okreslonym parametrą.
        /// </summary>
        /// <param name="asemblyConector">Przyjmujemy Assembly, który jest wynikiem Kompilacji</param>
        /// <returns>Zracana instancja obiektu</returns>
        public Object InstanceGenerator(Assembly asemblyConector)
        {
            try
            {
                IEnumerable<Type> typeName = asemblyConector.GetTypes().Where(e => e.BaseType == (typeof(BaseScript)));
                if (typeName.Count() > 0) return Activator.CreateInstance(typeName.First());
                else return null;
            }
            catch (Exception e)
            {
                throw new CompilerException("Instance of Assembly cannot be crated: " + e, ScriptStateEnum.Corrupted);
            }
        }

        /// <summary>
        ///     Zwraca typ pierwszego Assembly wedłóg listy kompilacji
        /// </summary>
        /// <param name="AsemblyConector"></param>
        /// <returns></returns>
        public Type GetType(Assembly AsemblyConector)
        {
            try
            {
                return AsemblyConector.GetTypes().First(e => e.BaseType == (typeof(BaseScript)));
            }
            catch (Exception e)
            {
                throw new CompilerException("Assembly type cannot be reached: " + e,
                    ScriptStateEnum.Corrupted);
            }
        }

        /// <summary>
        ///     Metoda Prekompilacyjna sprawdza czy pliki do kompilacji istnieją
        /// </summary>
        /// <param name="filename">plik do kompliacji np. "example.cs"</param>
        /// <param name="directory"> ścieżka pliku do kompilacji należy podawać w następującej postaci np.  "C:\\Temp\\"</param>
        public void PreCompilation(String directory, String filename)
        {
            if (File.Exists(PathEx.GetFullFilePath(directory, filename)))
            {
                Log.Info("File is ready to Compilation" + filename);
            }
            else
            {
                Log.Error("Input source file not found " + filename);
                throw new CompilerException("Input source file: " + filename + " not found",
                    ScriptStateEnum.ScriptNotExist);
            }
        }


        /// <summary>
        ///     Metoda kompilatora kompiluje kod z plików typu VB oraz CS
        /// </summary>
        /// <param name="flag">Flaga przeciwdziała zapętleniu kompilacji wynikającego z rekurencji metody patrz opis przy użyciu</param>
        /// <param name="sourceName"> Parametr określający nazwę pliku identycznie jak w metodzie  PreCompilation </param>
        /// <returns></returns>
        public Assembly CompileExecutable(String sourceName, bool flag)
        {
            //Ustalanie dostępu do pliku-------------------------------------------------
            if (_pathDirectory == "") _pathDirectory = Environment.CurrentDirectory;
            PreCompilation(_pathDirectory, sourceName); //czy wszytko ładnie istnieje
            var sourceFile = new FileInfo(PathEx.GetFullFilePath(_pathDirectory, sourceName));

            //Ustalanie dostępu do pliku-------------------------------------------------

            //Badanie czy taki plik nie został skompilowany-----------------------------
            if (FileStorage.Any(e => (e.Name == sourceFile.Name) && (e.CreationTime == sourceFile.CreationTime)))
            {
                Log.Info("Source file Alredy Compiled");
                //Gdy identyczny plik jest już skompilowany zwraca  Assembly, Który posiada CodeBase odnoszący sie do nazwy pliku, Który właśnie miał zostać skompilowany.   
                Assembly storageAssembly =
                    CacheCompilation.First(
                        e =>
                            e.CodeBase.Split('/').Contains(String.Format(@"{0}.dll", sourceFile.Name.Replace(".", "_"))));
                if (storageAssembly != null) return storageAssembly;
                throw new CompilerException(
                    "Compilation Assembly cache is corupted: " + sourceFile.Name +
                    " already compiled but Assembly not found. ", ScriptStateEnum.ScriptNotExist);
            }
            //Badanie czy taki plik nie został skompilowany-----------------------------

            CodeDomProvider provider = null;
            Assembly compileOk = null;

            //--------------------------------------------------------------------------
            // wybieranie rodzaju Providera w zależności od rozszerzenia pliku wejściowego.
            // W razie gdy żadna opcja nie zostanie spelniona cała metoda zwróci null
            if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
            {
                provider = CodeDomProvider.CreateProvider("CSharp");
            }
            else if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".VB")
            {
                provider = CodeDomProvider.CreateProvider("VisualBasic");
            }
            else
            {
                Log.Error("Source file must have a .cs or .vb extension");
            }
            //-------------------------------------------------------------------------

            //KOMPILACJA---------------------------------------------------------------
            if (provider == null)
            {
                Log.Error("Compilation imposible: Source file must have a .cs or .vb extension");
                //zwraca wyjątek gdy kompilacja jest w zaden sposób nie możliwa provider nie mogł zostac utworzony
                throw new CompilerException(
                    "Provider cannot be craeted. Source file must have a .cs or .vb extension",
                    ScriptStateEnum.CompilerError);
            }
            //USTALANIE SCIEŻEK----------------------------------------------------
            // Format Nazwy pliku wykonywalnego.
            // Budowanie ścieżki dla Assembly za pomocą aktualnego katalogu
            // wyniki: <source>_cs.exe/dll/anny albo <source>_vb.exe/dll/anny.
            //jeżeli Katalog w temp "TempScriptDll" nie istnieje jest tworzony
            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), TempDcriptdll)))
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), TempDcriptdll));
            //nazwa oraz ścieżka do dll
            String exeName = String.Format(@"{0}.dll",
                Path.Combine(Path.GetTempPath(), TempDcriptdll, sourceFile.Name.Replace(".", "_")));
            //USTALANIE SCIEŻEK----------------------------------------------------


            //Ustawianie Paremetrów kompilatora------------------------------------
            var cp = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = exeName,
                GenerateInMemory = false,
                TreatWarningsAsErrors = false
            };

            // Generate an executable instead of 
            // a class library.

            // Specify the assembly file name to generate.

            // Save the assembly as a physical file.

            // Set whether to treat all warnings as errors.

            //dodawanie bibliotek potrzebnych do działania skryptów
            var comonDlls = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
            //System.Reflection.Assembly.GetAssembly(typeof( System.Linq ));
            foreach (Assembly dll in comonDlls)
            {
                if (dll.IsDynamic) continue;
                AddReference(cp, dll);
            }

            //magiczne dodanie kilku referencji ktore nie ida z a utomatu
            AddReference(cp, typeof(FileNameEditor).Assembly);
            AddReference(cp, typeof(UITypeEditor).Assembly);
            AddReference(cp, typeof(Form).Assembly);
            AddReference(cp, typeof(Chart).Assembly);
            AddReference(cp, typeof(BufferBlock<>).Assembly);
            AddReference(cp, typeof(Task).Assembly);


            //Ustawianie Paremetrów kompilatora-----------------------------------

            // Inicializuje kompilacje
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, PathEx.GetFullFilePath(_pathDirectory, sourceName));

            //Generowanie Info o błędach Kompliacji--------------------------------
            if (cr.Errors.Count > 0)
            {
                // Informacja o błędach kompilacji
                Log.Error(string.Format("Errors building {0} into {1}", sourceName, cr.PathToAssembly));
                string stacktraceString = null; //Przechowuje informacje o błedach kompilacji
                foreach (CompilerError ce in cr.Errors)
                {
                    if (flag) //jeżeli status flagi nie jest ustawiony na true znaczy ze jest to pierwszy
                              //bład i mógł być spowodowany brakiem skompilowanego pliku podrzędnego.
                    {
                        ErrorCompilatorHandler(ce.ErrorText, sourceName);
                        CompileExecutable(sourceName, false);
                    }
                    else
                    {
                        stacktraceString += string.Format("  {0}", ce);
                        Log.Error(" " + ce);

                    }
                }
                //jeśli błedy nie byly spowodowane brakiem skompilowanego pliku, od którego zalezy skrypt zwracany jest wyjątek
                if (!flag)
                    throw new CompilerException("Compilation cannot be finished: " + stacktraceString,
                        ScriptStateEnum.CompilerError);
                return cr.CompiledAssembly;
            }

            // Komplicja powiodla sie info na ten temat.
            Log.Info("Source " + sourceName + " built into " + cr.PathToAssembly + " successfully.");
            //Generowanie Info o błędach Kompliacji--------------------------------

            // Zwraca efekty kompilacji--------------------------------------------
            //Dodawanie Assembly oraz pliku źródłowego do Cache 
            FileStorage.Add(sourceFile);
            CacheCompilation.Add(cr.CompiledAssembly);
            Log.Info("skompilowano: " + sourceName + " " + DateTime.Now.ToLongTimeString() + "------------------------------------------------------------------------------");
            Log.Info(File.ReadAllText(@"" + sourceFile));
            return cr.CompiledAssembly;
            // Zwraca efekty kompilacji--------------------------------------------

            //KOMPILACJA---------------------------------------------------------------
        }

        private static void AddReference(CompilerParameters cp, Assembly dll)
        {
            string replace = dll.CodeBase.Replace("file:///", "");
            if (!cp.ReferencedAssemblies.Contains(replace))
                cp.ReferencedAssemblies.Add(replace);
        }

        /// <summary>
        ///     Metoda, która została wykonana podczas błedu kompilacji wynikającego z braku pliku podrzędnego dla danej klasy
        ///     metoda kompiluje go a następnie odbywa się ponowna kompilacja w głównej metodzie kompilatora
        /// </summary>
        /// <param name="errorText"> źródło błedu, z którego wydobywana jest nazwa pliku do kompilacji</param>
        /// <param name="sourceName">Plik nadrzędny, który nie mógł zostac skompilowany</param>
        /// <returns></returns>
        private void ErrorCompilatorHandler(string errorText, string sourceName)
        {
            if (!_fileNotCompiledStorage.Contains(sourceName))
            {
                if (errorText.Contains("ScriptEngine"))
                {
                    string fileRecompiler = errorText.Split(' ').First(e => e.Contains("ScriptEngine"));
                    fileRecompiler = fileRecompiler.Split('.').Last().Split('\'').First() + ".cs";
                    _fileNotCompiledStorage.Add(sourceName);
                    CompileExecutable(fileRecompiler);
                }
            }
            else
            {
                Log.Error("Croos code compilation error: Class A include Class B and class B include A.");
                throw new CompilerException(
                    "Cross code compilation error: Class A include Class B and Class B include A. ",
                    ScriptStateEnum.CompilerError);
            }
            ;
        }
    }
}
