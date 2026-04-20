using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Callen.Updater
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var options = ParseArgs(args);

                var appDir = Path.GetFullPath(options["appDir"]);
                var zipPath = Path.GetFullPath(options["zip"]);
                var exeName = options["exe"];

                int pid;
                if (!int.TryParse(options["pid"], out pid))
                    throw new ArgumentException("Invalid pid argument.");

                if (!Directory.Exists(appDir))
                    throw new DirectoryNotFoundException("Application directory does not exist: " + appDir);
                if (!File.Exists(zipPath))
                    throw new FileNotFoundException("Update package not found.", zipPath);
                if (string.IsNullOrWhiteSpace(exeName))
                    throw new ArgumentException("Missing executable name.");

                WaitForProcessExit(pid, TimeSpan.FromMinutes(2));

                var stagingDir = Path.Combine(Path.GetTempPath(), "callen-update-" + Guid.NewGuid().ToString("N"));
                var backupDir = Path.Combine(Path.GetTempPath(), "callen-backup-" + Guid.NewGuid().ToString("N"));

                Directory.CreateDirectory(stagingDir);
                Directory.CreateDirectory(backupDir);

                try
                {
                    ZipFile.ExtractToDirectory(zipPath, stagingDir);

                    var packageRoot = ResolvePackageRoot(stagingDir, exeName);
                    ApplyUpdate(packageRoot, appDir, backupDir);

                    TryDeleteFile(zipPath);
                    TryDeleteDirectory(stagingDir);

                    var exePath = Path.Combine(appDir, exeName);
                    if (File.Exists(exePath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = exePath,
                            UseShellExecute = true
                        });
                    }
                }
                catch
                {
                    RestoreBackup(backupDir, appDir);
                    throw;
                }
                finally
                {
                    TryDeleteDirectory(backupDir);
                    TryDeleteDirectory(stagingDir);
                }

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < args.Length; i++)
            {
                var key = args[i];
                if (!key.StartsWith("--", StringComparison.Ordinal))
                    continue;

                if (i + 1 >= args.Length)
                    throw new ArgumentException("Missing value for " + key);

                values[key.Substring(2)] = args[++i];
            }

            var required = new[] { "pid", "appDir", "zip", "exe" };
            foreach (var key in required)
            {
                if (!values.ContainsKey(key))
                    throw new ArgumentException("Missing required argument --" + key);
            }

            return values;
        }

        private static void WaitForProcessExit(int processId, TimeSpan timeout)
        {
            try
            {
                using (var process = Process.GetProcessById(processId))
                {
                    if (!process.WaitForExit((int)timeout.TotalMilliseconds))
                        throw new TimeoutException("Timed out while waiting for the main process to exit.");
                }
            }
            catch (ArgumentException)
            {
                // Already exited.
            }
        }

        private static string ResolvePackageRoot(string extractedPath, string exeName)
        {
            var directExe = Path.Combine(extractedPath, exeName);
            if (File.Exists(directExe))
                return extractedPath;

            var subDirs = Directory.GetDirectories(extractedPath);
            if (subDirs.Length == 1)
            {
                var nestedExe = Path.Combine(subDirs[0], exeName);
                if (File.Exists(nestedExe))
                    return subDirs[0];
            }

            throw new InvalidOperationException("Update package does not contain " + exeName + ".");
        }

        private static void ApplyUpdate(string packageRoot, string appDir, string backupDir)
        {
            var files = Directory.GetFiles(packageRoot, "*", SearchOption.AllDirectories);

            foreach (var sourceFile in files)
            {
                var relativePath = sourceFile.Substring(packageRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (ShouldSkip(relativePath))
                    continue;

                var destinationFile = Path.Combine(appDir, relativePath);
                var destinationDir = Path.GetDirectoryName(destinationFile);
                if (!string.IsNullOrWhiteSpace(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                if (File.Exists(destinationFile))
                {
                    var backupFile = Path.Combine(backupDir, relativePath);
                    var backupFileDir = Path.GetDirectoryName(backupFile);
                    if (!string.IsNullOrWhiteSpace(backupFileDir))
                        Directory.CreateDirectory(backupFileDir);

                    File.Copy(destinationFile, backupFile, true);
                }

                File.Copy(sourceFile, destinationFile, true);
            }
        }

        private static bool ShouldSkip(string relativePath)
        {
            var normalized = relativePath.Replace('/', '\\');

            if (normalized.StartsWith("data\\", StringComparison.OrdinalIgnoreCase))
                return true;
            if (normalized.StartsWith("images\\", StringComparison.OrdinalIgnoreCase))
                return true;

            var fileName = Path.GetFileName(normalized);
            if (fileName.Equals("Callen.Updater.exe", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static void RestoreBackup(string backupDir, string appDir)
        {
            if (!Directory.Exists(backupDir))
                return;

            var files = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
            foreach (var backupFile in files)
            {
                var relativePath = backupFile.Substring(backupDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var destinationFile = Path.Combine(appDir, relativePath);
                var destinationDir = Path.GetDirectoryName(destinationFile);
                if (!string.IsNullOrWhiteSpace(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                File.Copy(backupFile, destinationFile, true);
            }
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                // Best effort cleanup.
            }
        }

        private static void TryDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch
            {
                // Best effort cleanup.
            }
        }
    }
}
