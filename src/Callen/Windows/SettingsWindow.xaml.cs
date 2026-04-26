using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;

namespace Callen.Windows
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private const string LanguageSettingKey = "language";
        private const string LatestReleaseApiUrl = "https://api.github.com/repos/dedukun/callen/releases/latest";
        private const string ReleasesPageUrl = "https://github.com/tiagomfmadeira/callen/releases/";
        private const string UpdateManifestAssetName = "callen-update.json";
        private const string LocalVersionFileName = "version.txt";
        private static readonly HttpClient UpdateHttpClient = CreateUpdateHttpClient();

        private readonly Configuration config;
        private readonly WindowOverlaySync overlaySync;
        private readonly Action onSettingsSaved;
        private readonly string initialLanguageCode;

        private string latestReleasePageUrl;
        private string latestZipDownloadUrl;
        private string latestZipSha256;
        private string latestTagName;
        private bool suppressCloseRevertPrompt;
        private bool isRefreshingLanguageOptions;

        private sealed class LanguageOption
        {
            public string Code { get; set; }
            public string Display { get; set; }
        }

        public SettingsWindow(Action onSettingsSaved = null)
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);
            this.onSettingsSaved = onSettingsSaved;

            PreviewKeyDown += HandleEsc;
            SourceInitialized += WinSettings_SourceInitialized;
            Closed += WinSettings_Closed;
            Closing += WinSettings_Closing;

            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            initialLanguageCode = App.CurrentLanguageCode;
            InitializeLanguageOptions(initialLanguageCode);

            var configuredDbPath = config.AppSettings.Settings["database_path"]?.Value;
            var configuredImagePath = config.AppSettings.Settings["image_path"]?.Value;
            db_string.Text = string.IsNullOrWhiteSpace(configuredDbPath)
                ? App.ToPortableConfigPath(App.DefaultPortableDatabasePath, App.DefaultPortableDatabasePath)
                : configuredDbPath;
            img_folder.Text = string.IsNullOrWhiteSpace(configuredImagePath)
                ? App.ToPortableConfigPath(App.DefaultPortableImagePath, App.DefaultPortableImagePath)
                : configuredImagePath;
            app_version.Text = GetCurrentVersionText();
            btn_check_updates.ToolTip = Loc.T("Settings.Update");
        }

        private void WinSettings_SourceInitialized(object sender, EventArgs e)
        {
            overlaySync.Attach();
        }

        private void WinSettings_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown -= HandleEsc;
            Closing -= WinSettings_Closing;
            overlaySync.Detach();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void btn_save_changes_Click(object sender, RoutedEventArgs e)
        {
            var configuredDbPath = App.ToPortableConfigPath(
                db_string.Text?.Trim() ?? string.Empty,
                App.DefaultPortableDatabasePath);

            var configuredImagePath = App.ToPortableConfigPath(
                img_folder.Text?.Trim() ?? string.Empty,
                App.DefaultPortableImagePath);

            var selectedLanguageCode = (combo_language.SelectedItem as LanguageOption)?.Code ?? App.CurrentLanguageCode;

            if (config.AppSettings.Settings["database_path"] == null)
                config.AppSettings.Settings.Add("database_path", configuredDbPath);
            else
                config.AppSettings.Settings["database_path"].Value = configuredDbPath;

            if (config.AppSettings.Settings["image_path"] == null)
                config.AppSettings.Settings.Add("image_path", configuredImagePath);
            else
                config.AppSettings.Settings["image_path"].Value = configuredImagePath;

            if (config.AppSettings.Settings[LanguageSettingKey] == null)
                config.AppSettings.Settings.Add(LanguageSettingKey, selectedLanguageCode);
            else
                config.AppSettings.Settings[LanguageSettingKey].Value = selectedLanguageCode;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("AppSettings");

            string applyError;
            if (!App.TryApplyPaths(configuredDbPath, configuredImagePath, out applyError))
            {
                ShowSettingsDialog(
                    Loc.T("Msg.GenericTitle"),
                    string.Empty,
                    Loc.F("Msg.SettingsSavedWarnApply", Environment.NewLine, applyError));
                return;
            }

            App.ApplyLanguage(selectedLanguageCode);
            RefreshLanguageOptionDisplay(selectedLanguageCode);
            suppressCloseRevertPrompt = true;
            db_string.Text = configuredDbPath;
            img_folder.Text = configuredImagePath;

            new NotificationWindow(Loc.T("Noti.SettingsSavedTitle"), Loc.T("Msg.SettingsSaved"), string.Empty).Show();
            onSettingsSaved?.Invoke();
            Close();
        }

        private void combo_language_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (isRefreshingLanguageOptions)
                return;

            var selectedLanguageCode = (combo_language.SelectedItem as LanguageOption)?.Code;
            if (string.IsNullOrWhiteSpace(selectedLanguageCode))
                return;

            var normalizedLanguageCode = App.NormalizeLanguageCode(selectedLanguageCode);
            if (string.Equals(normalizedLanguageCode, App.CurrentLanguageCode, StringComparison.OrdinalIgnoreCase))
                return;

            App.ApplyLanguage(normalizedLanguageCode);
            RefreshLanguageOptionDisplay(normalizedLanguageCode);
        }

        private void btn_img_folder_Click(object sender, RoutedEventArgs e)
        {
            var diag = new VistaFolderBrowserDialog();
            if (diag.ShowDialog() == true)
                img_folder.Text = diag.SelectedPath;
        }

        private void btn_db_path_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = Loc.T("Msg.SelectDatabaseTitle"),
                Filter = "SQLite DB (*.db;*.sqlite)|*.db;*.sqlite|All files (*.*)|*.*",
                CheckFileExists = false
            };

            if (dialog.ShowDialog() == true)
                db_string.Text = dialog.FileName;
        }

        private void app_version_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ReleasesPageUrl,
                    UseShellExecute = true
                });
            }
            catch
            {
                ShowSettingsDialog(
                    Loc.T("Msg.GenericTitle"),
                    string.Empty,
                    Loc.T("Msg.OpenReleasesError"));
            }
        }

        private async void btn_check_updates_Click(object sender, RoutedEventArgs e)
        {
            btn_check_updates.IsEnabled = false;
            latestReleasePageUrl = null;
            latestZipDownloadUrl = null;
            latestZipSha256 = null;
            latestTagName = null;
            btn_check_updates.ToolTip = Loc.T("Settings.Update");

            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                var response = await UpdateHttpClient.GetAsync(LatestReleaseApiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    ShowUpdatePrompt(Loc.T("Msg.UpdatesGithubFail"));
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var release = JsonConvert.DeserializeObject<GitHubRelease>(json);
                if (release == null)
                {
                    ShowUpdatePrompt(Loc.T("Msg.UpdatesInvalidResponse"));
                    return;
                }

                latestReleasePageUrl = release.HtmlUrl;
                latestTagName = release.TagName;

                var zipAsset = FindZipAsset(release);
                latestZipDownloadUrl = zipAsset == null ? null : zipAsset.BrowserDownloadUrl;

                var manifestAsset = release.Assets == null
                    ? null
                    : release.Assets.FirstOrDefault(a =>
                        a != null &&
                        string.Equals(a.Name, UpdateManifestAssetName, StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(a.BrowserDownloadUrl));

                if (manifestAsset != null)
                {
                    try
                    {
                        var manifestJson = await UpdateHttpClient.GetStringAsync(manifestAsset.BrowserDownloadUrl);
                        var manifest = JsonConvert.DeserializeObject<UpdateManifest>(manifestJson);
                        var manifestVersionMatchesTag =
                            manifest != null &&
                            !string.IsNullOrWhiteSpace(manifest.Version) &&
                            string.Equals(manifest.Version.Trim(), latestTagName, StringComparison.OrdinalIgnoreCase);

                        var manifestZipMatchesAsset =
                            manifest != null &&
                            !string.IsNullOrWhiteSpace(manifest.Zip) &&
                            zipAsset != null &&
                            string.Equals(manifest.Zip.Trim(), zipAsset.Name, StringComparison.OrdinalIgnoreCase);

                        if (manifestVersionMatchesTag && manifestZipMatchesAsset)
                            latestZipSha256 = manifest.Sha256?.Trim().ToLowerInvariant();
                    }
                    catch
                    {
                        latestZipSha256 = null;
                    }
                }

                var currentVersion = GetCurrentAppVersion();
                var latestVersion = ParseTagVersion(release.TagName);

                if (latestVersion != null && currentVersion != null && latestVersion > currentVersion)
                {
                    if (string.IsNullOrWhiteSpace(latestZipDownloadUrl))
                    {
                        ShowUpdatePrompt(Loc.F("Msg.NewVersionAvailable", release.TagName));
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(latestZipSha256))
                    {
                        ShowUpdatePrompt(Loc.F("Msg.NewVersionNoAuto", release.TagName));
                        return;
                    }

                    await ShowUpdateDialogAsync(Loc.F("Msg.NewVersionCurrent", release.TagName, currentVersion), true);
                    return;
                }

                if (latestVersion == null)
                {
                    ShowUpdatePrompt(Loc.F("Msg.LatestReleaseFound", release.TagName ?? "(sem tag)"));
                    return;
                }

                ShowUpdatePrompt(Loc.F("Msg.AlreadyLatest", currentVersion));
            }
            catch (Exception)
            {
                ShowUpdatePrompt(Loc.T("Msg.UpdatesCheckFailNow"));
            }
            finally
            {
                btn_check_updates.IsEnabled = true;
            }
        }

        private async Task StartAutoUpdateAsync(bool requireConfirmation = true)
        {
            if (string.IsNullOrWhiteSpace(latestZipDownloadUrl) || string.IsNullOrWhiteSpace(latestZipSha256))
            {
                ShowUpdatePrompt(Loc.T("Msg.AutoUpdateUnavailable"));
                return;
            }

            var appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            var updaterPath = Path.Combine(appBasePath, "Callen.Updater.exe");
            if (!File.Exists(updaterPath))
            {
                ShowUpdatePrompt(Loc.T("Msg.UpdaterMissing"));
                btn_check_updates.ToolTip = Loc.T("Settings.Update");
                return;
            }

            if (requireConfirmation)
            {
                var confirmDialog = new ActionDialogWindow(
                    Loc.T("Msg.UpdateTitle"),
                    string.Empty,
                    Loc.F("Msg.UpdateConfirm", latestTagName, Environment.NewLine),
                    Loc.T("Settings.Install"),
                    Loc.T("Dlg.Cancel"));

                DialogHelper.ShowOwnedDialog(confirmDialog, this);
                if (confirmDialog.SelectedAction != DialogAction.Primary)
                    return;
            }

            btn_check_updates.IsEnabled = false;

            var tempZipPath = Path.Combine(Path.GetTempPath(), "callen-update-" + Guid.NewGuid().ToString("N") + ".zip");

            try
            {
                await DownloadFileAsync(latestZipDownloadUrl, tempZipPath);

                var hash = ComputeSha256(tempZipPath);
                if (!string.Equals(hash, latestZipSha256, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(Loc.T("Msg.UpdateHashMismatch"));

                var currentExePath = Assembly.GetExecutingAssembly().Location;
                var currentExeName = Path.GetFileName(currentExePath);
                var updaterArgs = BuildUpdaterArguments(Process.GetCurrentProcess().Id, appBasePath, tempZipPath, currentExeName);

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = updaterArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = appBasePath
                });

                Application.Current.Shutdown();
            }
            catch (Exception)
            {
                TryDeleteFile(tempZipPath);
                ShowUpdatePrompt(Loc.T("Msg.UpdateInstallFail"));
                btn_check_updates.ToolTip = Loc.T("Settings.Update");
                btn_check_updates.IsEnabled = true;
            }
        }

        private async Task ShowUpdateDialogAsync(string message, bool allowInstall)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var primaryText = allowInstall ? Loc.T("Settings.Install") : Loc.T("Dlg.Close");
            var secondaryText = allowInstall ? Loc.T("Dlg.Close") : null;
            var updateDialog = new ActionDialogWindow(
                Loc.T("Msg.UpdateTitle"),
                string.Empty,
                message,
                primaryText,
                secondaryText);

            DialogHelper.ShowOwnedDialog(updateDialog, this);

            if (allowInstall && updateDialog.SelectedAction == DialogAction.Primary)
                await StartAutoUpdateAsync(false);
        }

        private void ShowUpdatePrompt(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var updateDialog = new ActionDialogWindow(
                Loc.T("Msg.UpdateTitle"),
                string.Empty,
                message,
                Loc.T("Dlg.Close"));

            DialogHelper.ShowOwnedDialog(updateDialog, this);
        }

        private void ShowSettingsDialog(string title, string context, string message)
        {
            var dialog = new ActionDialogWindow(
                title ?? Loc.T("Msg.GenericTitle"),
                context ?? string.Empty,
                message ?? string.Empty,
                Loc.T("Dlg.Close"));

            DialogHelper.ShowOwnedDialog(dialog, this);
        }

        private void InitializeLanguageOptions(string selectedLanguageCode = null)
        {
            isRefreshingLanguageOptions = true;
            combo_language.ItemsSource = new[]
            {
                new LanguageOption { Code = "pt", Display = Loc.T("Lang.Portuguese") },
                new LanguageOption { Code = "en", Display = Loc.T("Lang.English") }
            };
            combo_language.DisplayMemberPath = "Display";
            combo_language.SelectedValuePath = "Code";
            combo_language.SelectedValue = App.NormalizeLanguageCode(selectedLanguageCode ?? App.CurrentLanguageCode);
            isRefreshingLanguageOptions = false;
        }

        private void RefreshLanguageOptionDisplay(string selectedLanguageCode = null)
        {
            InitializeLanguageOptions(selectedLanguageCode);
            btn_check_updates.ToolTip = Loc.T("Settings.Update");
        }

        private void WinSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (suppressCloseRevertPrompt)
                return;

            if (string.Equals(App.CurrentLanguageCode, initialLanguageCode, StringComparison.OrdinalIgnoreCase))
                return;

            var languageDialog = new ActionDialogWindow(
                Loc.T("Dlg.LanguagePreviewDiscardTitle"),
                string.Empty,
                Loc.T("Dlg.LanguagePreviewDiscardMessage"),
                Loc.T("Dlg.Save"),
                Loc.T("Dlg.Discard"),
                Loc.T("Dlg.Cancel"));

            DialogHelper.ShowOwnedDialog(languageDialog, this);

            if (languageDialog.SelectedAction == DialogAction.Primary)
            {
                PersistLanguageSettingOnly();
                suppressCloseRevertPrompt = true;
                return;
            }

            if (languageDialog.SelectedAction == DialogAction.Secondary)
            {
                suppressCloseRevertPrompt = true;
                App.ApplyLanguage(initialLanguageCode);
                return;
            }

            e.Cancel = true;
        }

        private void PersistLanguageSettingOnly()
        {
            var selectedLanguageCode = (combo_language.SelectedItem as LanguageOption)?.Code ?? App.CurrentLanguageCode;
            var normalizedLanguageCode = App.NormalizeLanguageCode(selectedLanguageCode);

            if (config.AppSettings.Settings[LanguageSettingKey] == null)
                config.AppSettings.Settings.Add(LanguageSettingKey, normalizedLanguageCode);
            else
                config.AppSettings.Settings[LanguageSettingKey].Value = normalizedLanguageCode;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("AppSettings");
        }

        private static HttpClient CreateUpdateHttpClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Callen/1.0 (+https://github.com/dedukun/callen)");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
            return client;
        }

        private static string GetCurrentVersionText()
        {
            var localTag = ReadLocalVersionTag();
            if (!string.IsNullOrWhiteSpace(localTag))
                return localTag;

            var assembly = Assembly.GetExecutingAssembly();
            var infoVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            if (!string.IsNullOrWhiteSpace(infoVersion))
                return "v" + infoVersion;

            var version = assembly.GetName().Version;
            return version == null ? "n/d" : "v" + version;
        }

        private static Version GetCurrentAppVersion()
        {
            var localTag = ReadLocalVersionTag();
            var localVersion = ParseTagVersion(localTag);
            if (localVersion != null)
                return localVersion;

            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        private static string ReadLocalVersionTag()
        {
            try
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var versionFilePath = Path.Combine(basePath, LocalVersionFileName);
                if (!File.Exists(versionFilePath))
                    return null;

                var text = File.ReadAllText(versionFilePath).Trim();
                if (string.IsNullOrWhiteSpace(text))
                    return null;

                return text.StartsWith("v", StringComparison.OrdinalIgnoreCase) ? text : "v" + text;
            }
            catch
            {
                return null;
            }
        }

        private static Version ParseTagVersion(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
                return null;

            var value = tagName.Trim();
            if (value.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                value = value.Substring(1);

            var dashIndex = value.IndexOf('-');
            if (dashIndex >= 0)
                value = value.Substring(0, dashIndex);

            var plusIndex = value.IndexOf('+');
            if (plusIndex >= 0)
                value = value.Substring(0, plusIndex);

            Version parsed;
            if (Version.TryParse(value, out parsed))
                return parsed;

            return null;
        }

        private static GitHubReleaseAsset FindZipAsset(GitHubRelease release)
        {
            if (release == null || release.Assets == null || release.Assets.Length == 0)
                return null;

            var zipAsset = release.Assets.FirstOrDefault(a =>
                a != null &&
                !string.IsNullOrWhiteSpace(a.Name) &&
                string.Equals(a.Name, "callen.zip", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(a.BrowserDownloadUrl));

            if (zipAsset != null)
                return zipAsset;

            return release.Assets.FirstOrDefault(a =>
                a != null &&
                !string.IsNullOrWhiteSpace(a.Name) &&
                a.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(a.BrowserDownloadUrl));
        }

        private static async Task DownloadFileAsync(string url, string destinationPath)
        {
            using (var response = await UpdateHttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var input = await response.Content.ReadAsStreamAsync())
                using (var output = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await input.CopyToAsync(output);
                }
            }
        }

        private static string ComputeSha256(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private static string BuildUpdaterArguments(int pid, string appDir, string zipPath, string exeName)
        {
            return "--pid " + pid +
                   " --appDir " + QuoteArgument(appDir) +
                   " --zip " + QuoteArgument(zipPath) +
                   " --exe " + QuoteArgument(exeName);
        }

        private static string QuoteArgument(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }

        private static void TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch
            {
                // Best effort.
            }
        }

        private sealed class GitHubRelease
        {
            [JsonProperty("tag_name")]
            public string TagName { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("assets")]
            public GitHubReleaseAsset[] Assets { get; set; }
        }

        private sealed class GitHubReleaseAsset
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("browser_download_url")]
            public string BrowserDownloadUrl { get; set; }
        }

        private sealed class UpdateManifest
        {
            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("zip")]
            public string Zip { get; set; }

            [JsonProperty("sha256")]
            public string Sha256 { get; set; }
        }
    }
}


