using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace Callen
{
    public partial class App : Application
    {
        private const string DatabaseName = "Callen.db";
        private static string AppBasePath => AppDomain.CurrentDomain.BaseDirectory;
        private static string PortableDatabasePath => Path.Combine(AppBasePath, "Data", DatabaseName);
        private static string PortableImagePath => Path.Combine(AppBasePath, "Images");

        public static string DatabasePath
        {
            get { return ResolveConfiguredPath("database_path", PortableDatabasePath); }
        }

        public static string ImagePath
        {
            get { return ResolveConfiguredPath("image_path", PortableImagePath); }
        }

        public static string databasePath => DatabasePath;

        private static string ResolveConfiguredPath(string key, string defaultPath)
        {
            var configuredPath = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(configuredPath) ? defaultPath : configuredPath.Trim();
        }
    }
}
