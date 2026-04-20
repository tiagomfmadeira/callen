using System;
using System.Configuration;
using System.IO;
using System.Windows;
using SQLite;

namespace Callen
{
    public partial class App : Application
    {
        private const string DatabaseName = "Callen.db";
        private const string LanguageSettingKey = "language";
        private const string PortugueseLanguageCode = "pt";
        private const string EnglishLanguageCode = "en";
        private static string AppBasePath => AppDomain.CurrentDomain.BaseDirectory;
        private static string PortableDatabasePath => Path.Combine(AppBasePath, "data", DatabaseName);
        private static string PortableImagePath => Path.Combine(AppBasePath, "images");
        private static string currentDatabasePath;
        private static string currentImagePath;

        public static string DatabasePath
        {
            get
            {
                EnsureRuntimePathsInitialized();
                return currentDatabasePath;
            }
        }

        public static string ImagePath
        {
            get
            {
                EnsureRuntimePathsInitialized();
                return currentImagePath;
            }
        }

        public static string databasePath => DatabasePath;
        public static string CurrentLanguageCode { get; private set; } = PortugueseLanguageCode;

        private static void EnsureRuntimePathsInitialized()
        {
            if (!string.IsNullOrWhiteSpace(currentDatabasePath) && !string.IsNullOrWhiteSpace(currentImagePath))
                return;

            TryInitializeStorage(out _);
        }

        private static string ResolveConfiguredPath(string configuredPath, string defaultPath)
        {
            var path = string.IsNullOrWhiteSpace(configuredPath) ? defaultPath : configuredPath.Trim();
            if (!Path.IsPathRooted(path))
                path = Path.Combine(AppBasePath, path);

            return Path.GetFullPath(path);
        }

        public static bool TryInitializeStorage(out string error)
        {
            var configuredDbPath = ConfigurationManager.AppSettings["database_path"];
            var configuredImagePath = ConfigurationManager.AppSettings["image_path"];
            return TryApplyPaths(configuredDbPath, configuredImagePath, out error);
        }

        public static bool TryApplyPaths(string configuredDbPath, string configuredImagePath, out string error)
        {
            try
            {
                var resolvedDatabasePath = ResolveConfiguredPath(configuredDbPath, PortableDatabasePath);
                var resolvedImagePath = ResolveConfiguredPath(configuredImagePath, PortableImagePath);

                EnsureStorageLayout(resolvedDatabasePath, resolvedImagePath);

                currentDatabasePath = resolvedDatabasePath;
                currentImagePath = resolvedImagePath;
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static void EnsureStorageLayout(string databasePath, string imagePath)
        {
            var dbFolder = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrWhiteSpace(dbFolder))
                Directory.CreateDirectory(dbFolder);

            Directory.CreateDirectory(imagePath);

            using (var conn = new SQLiteConnection(databasePath))
            {
                conn.Execute(
                    @"CREATE TABLE IF NOT EXISTS Archive (
                        id INTEGER PRIMARY KEY,
                        code TEXT,
                        theme TEXT
                    );");

                conn.Execute(
                    @"CREATE TABLE IF NOT EXISTS Calendar (
                        id INTEGER PRIMARY KEY,
                        name TEXT,
                        description TEXT,
                        year TEXT,
                        matrix TEXT,
                        collection TEXT,
                        date_inserted TEXT,
                        date_modified TEXT,
                        date_viewed TEXT,
                        deleted INTEGER,
                        pic_path TEXT,
                        archive_id INTEGER,
                        FOREIGN KEY (archive_id) REFERENCES Archive (id)
                    );");
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ApplyLanguage(ConfigurationManager.AppSettings[LanguageSettingKey]);

            string error;
            if (!TryInitializeStorage(out error))
            {
                MessageBox.Show(
                    Loc.F("Msg.InitStorageError", Environment.NewLine, error),
                    Loc.T("Msg.GenericTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public static string NormalizeLanguageCode(string languageCode)
        {
            return string.Equals(languageCode, EnglishLanguageCode, StringComparison.OrdinalIgnoreCase)
                ? EnglishLanguageCode
                : PortugueseLanguageCode;
        }

        public static void ApplyLanguage(string languageCode)
        {
            var app = Current;
            if (app == null)
                return;

            var normalizedLanguageCode = NormalizeLanguageCode(languageCode);
            var dictionaryPath = normalizedLanguageCode == EnglishLanguageCode
                ? "/Callen;component/Resources/Strings.en.xaml"
                : "/Callen;component/Resources/Strings.pt.xaml";

            for (var i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
            {
                var source = app.Resources.MergedDictionaries[i].Source;
                if (source == null)
                    continue;

                var sourceText = source.OriginalString ?? string.Empty;
                if (sourceText.IndexOf("/Resources/Strings.", StringComparison.OrdinalIgnoreCase) >= 0)
                    app.Resources.MergedDictionaries.RemoveAt(i);
            }

            app.Resources.MergedDictionaries.Insert(
                0,
                new ResourceDictionary { Source = new Uri(dictionaryPath, UriKind.Relative) });

            CurrentLanguageCode = normalizedLanguageCode;
        }
    }
}
