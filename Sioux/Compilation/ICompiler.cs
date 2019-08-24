using System;
using System.Reflection;


namespace ManagingWebSerwer.Compilation
{
    public interface ICompiler
    {
        /// <summary>
        ///     Metoda kompilatora kompiluje kod z plików typu VB oraz CS
        /// </summary>
        /// <param name="sourceName"> Parametr okreslający nazwę pliku identycznie jak w metodzie  PreCompilation </param>
        /// <returns></returns>
        Assembly CompileExecutable(String sourceName);

        /// <summary>
        ///     Zwraca Activator instancji Assembly
        /// </summary>
        /// <param name="asemblyConector"></param>
        /// <returns></returns>
        Object InstanceGenerator(Assembly asemblyConector);

        /// <summary>
        ///     Zwraca typ Assembly pierwszego wedłóg listy kompilacji
        /// </summary>
        /// <param name="asemblyConector"></param>
        /// <returns></returns>
        Type GetType(Assembly asemblyConector);
    }
}