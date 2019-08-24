using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;



namespace SiouxComon.FrameworkExtensions
{
    public static class PathEx
    {
        public static string ProgramPath
        {
            get;
            private set;
        }

        static PathEx()
        {
            var inAssembly = Assembly.GetEntryAssembly();
            if (inAssembly != null)
            {

                ProgramPath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            }
            else
            {
                ProgramPath = Environment.CurrentDirectory;
            }
        }

        public static string GetFullDirectoryPath(string folder)
        {
            if (!Path.IsPathRooted(folder))
                folder = Path.Combine(ProgramPath, folder);

            return folder;
        }


        public static string GetFullFilePath(string folder, string file)
        {
            folder = GetFullDirectoryPath(folder);

            string fullFileName = Path.Combine(folder, file);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return fullFileName;
        }

        public static string GetFullFilePath(FileLocation plase, string filename)
        {
            string subdir;
            if (plase == FileLocation.CurrentUserSettings ||
                plase == FileLocation.AllUsersSettings ||
                plase == FileLocation.CurrentUserLocalSettings)
                subdir = Process.GetCurrentProcess().ProcessName.Replace(".vshost", "") + "/" + filename;
            else subdir = filename;
            string myfile;
            switch (plase)
            {
                case FileLocation.AllUsersSettings:
                    myfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        subdir);
                    break;
                case FileLocation.CurrentUserLocalSettings:
                    myfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        subdir);
                    break;
                case FileLocation.CurrentUserSettings:
                    myfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), subdir);
                    break;
                case FileLocation.ProgramPathSlashCfg:
                    myfile = Path.Combine(ProgramPath, "cfg", subdir);
                    break;
                default:
                    myfile = Path.Combine(ProgramPath, subdir);
                    break;
            }
            DirectoryInfo directoryInfo = (new FileInfo(myfile)).Directory;
            if (directoryInfo == null || directoryInfo.Exists) return myfile;
            string directoryName = (new FileInfo(myfile)).DirectoryName;
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);


            return myfile;
        }
    }

    public enum FileLocation
    {
        /// <summary>
        ///     Gdzies wewnatrz settingsow aktualnego zalogowanego usera na dowolnym komputerze
        /// </summary>
        CurrentUserSettings,

        /// <summary>
        ///     Gdzies wewnatrz settingsow aktualnego zalogowanego usera na danym komputerze
        /// </summary>
        CurrentUserLocalSettings,

        /// <summary>
        ///     gdzies wewnatrz settingsow wszystkich userow danego komputera
        /// </summary>
        AllUsersSettings,

        /// <summary>
        ///     folder programu
        /// </summary>
        ProgramPath,

        /// <summary>
        ///     folder programu
        /// </summary>
        ProgramPathSlashCfg
    }
}