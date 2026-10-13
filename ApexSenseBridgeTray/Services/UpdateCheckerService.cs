using ApexSenseBridgeTray.Common;
using ApexSenseBridge.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace ApexSenseBridgeTray.Services
{
    public class UpdateInfo
    {
        public bool HasUpdate { get; set; }
        public string CurrentVersion { get; set; }
        public string LatestVersion { get; set; }
        public string ReleaseNotes { get; set; }
        public string ReleaseUrl { get; set; }
        public string InstallerUrl { get; set; }
    }

    public class UpdateCheckerService
    {
        private const string GitHubApiLatestRelease = "https://api.github.com/repos/ReynArts/ApexSenseBridge/releases/latest";
        private const string GitHubReleasesPage = "https://github.com/ReynArts/ApexSenseBridge/releases";

        public event Action<UpdateInfo> UpdateAvailable;

        public string GetCurrentVersion()
        {
            try
            {
                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                if (ver != null)
                {
                    return string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
                }
            }
            catch
            {
            }
            return "1.1.0";
        }

        public async Task<UpdateInfo> CheckForUpdatesAsync(bool silent)
        {
            UpdateInfo info = new UpdateInfo();
            info.CurrentVersion = GetCurrentVersion();
            info.HasUpdate = false;
            info.ReleaseUrl = GitHubReleasesPage;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("User-Agent", "ApexSenseBridgeTray/1.0");

                    HttpResponseMessage response = await client.GetAsync(GitHubApiLatestRelease);
                    if (!response.IsSuccessStatusCode)
                    {
                        if (!silent)
                        {
                            MessageBox.Show(
                                LocalizationManager.Get("Loc_UpdateCheckUnavailable"),
                                LocalizationManager.Get("Loc_UpdateDialogTitle"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                        return info;
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = int.MaxValue;
                    Dictionary<string, object> dict = serializer.Deserialize<Dictionary<string, object>>(json);

                    if (dict != null)
                    {
                        string tagName = dict.ContainsKey("tag_name") && dict["tag_name"] != null
                            ? dict["tag_name"].ToString().TrimStart('v', 'V')
                            : string.Empty;

                        info.LatestVersion = tagName;

                        if (dict.ContainsKey("html_url") && dict["html_url"] != null)
                        {
                            info.ReleaseUrl = dict["html_url"].ToString();
                        }

                        if (dict.ContainsKey("body") && dict["body"] != null)
                        {
                            info.ReleaseNotes = dict["body"].ToString();
                        }

                        if (dict.ContainsKey("assets") && dict["assets"] is ArrayList)
                        {
                            ArrayList assets = dict["assets"] as ArrayList;
                            if (assets != null)
                            {
                                foreach (object rawAsset in assets)
                                {
                                    Dictionary<string, object> asset = rawAsset as Dictionary<string, object>;
                                    if (asset != null && asset.ContainsKey("name") && asset["name"] != null)
                                    {
                                        string name = asset["name"].ToString();
                                        string downloadUrl = asset.ContainsKey("browser_download_url") &&
                                            asset["browser_download_url"] != null
                                            ? asset["browser_download_url"].ToString()
                                            : string.Empty;
                                        if (ReleaseSecurity.IsExpectedSetupAsset(name, downloadUrl))
                                        {
                                            info.InstallerUrl = downloadUrl;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(info.LatestVersion))
                        {
                            info.HasUpdate = IsNewerVersion(info.LatestVersion, info.CurrentVersion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show(
                        LocalizationManager.Get("Loc_UpdateError") + ex.Message,
                        LocalizationManager.Get("Loc_UpdateDialogTitle"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                return info;
            }

            if (info.HasUpdate)
            {
                Action<UpdateInfo> handler = UpdateAvailable;
                if (handler != null)
                {
                    handler(info);
                }

                if (!silent)
                {
                    string msg = LocalizationManager.Format("Loc_UpdatePrompt", info.CurrentVersion, info.LatestVersion);

                    MessageBoxResult result = MessageBox.Show(
                        msg,
                        LocalizationManager.Get("Loc_UpdateDialogAvailableTitle"),
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        DownloadOrOpenRelease(info);
                    }
                }
            }
            else if (!silent)
            {
                MessageBox.Show(
                    LocalizationManager.Format("Loc_UpdateUpToDate", info.CurrentVersion),
                    LocalizationManager.Get("Loc_UpdateDialogTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            return info;
        }

        public async Task<bool> DownloadAndInstallAsync(UpdateInfo info, IProgress<int> progress = null)
        {
            if (info == null || string.IsNullOrEmpty(info.InstallerUrl))
            {
                OpenReleaseUrl(info);
                return false;
            }

            string tempPath = null;
            try
            {
                tempPath = Path.Combine(
                    Path.GetTempPath(),
                    "ApexSenseBridge-Setup-" + Guid.NewGuid().ToString("N") + ".exe");

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "ApexSenseBridgeTray/1.0");

                    using (var response = await httpClient.GetAsync(info.InstallerUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        long? totalBytes = response.Content.Headers.ContentLength;

                        using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            var buffer = new byte[8192];
                            long totalRead = 0;
                            int bytesRead;

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                                totalRead += bytesRead;

                                if (totalBytes.HasValue && totalBytes.Value > 0 && progress != null)
                                {
                                    int percent = (int)((totalRead * 100) / totalBytes.Value);
                                    progress.Report(Math.Min(100, percent));
                                }
                            }
                        }
                    }
                }

                if (progress != null) progress.Report(100);

                string verificationError;
                if (ReleaseSecurity.VerifyInstaller(
                    tempPath,
                    info.LatestVersion,
                    Assembly.GetExecutingAssembly().Location,
                    out verificationError))
                {
                    ProcessStartInfo psi = new ProcessStartInfo(tempPath) { UseShellExecute = true };
                    Process.Start(psi);
                    return true;
                }

                try { File.Delete(tempPath); } catch { }
                MessageBox.Show(
                    "The downloaded installer was not launched because its identity could not be verified.\n\n" + verificationError,
                    LocalizationManager.Get("Loc_UpdateDialogTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            catch (Exception)
            {
                try { if (tempPath != null && File.Exists(tempPath)) File.Delete(tempPath); } catch { }
                OpenReleaseUrl(info);
                return false;
            }
        }

        public void OpenReleaseUrl(UpdateInfo info)
        {
            string targetUrl = (info != null && !string.IsNullOrEmpty(info.ReleaseUrl)) ? info.ReleaseUrl : GitHubReleasesPage;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(targetUrl) { UseShellExecute = true };
                Process.Start(psi);
            }
            catch { }
        }

        public void DownloadOrOpenRelease(UpdateInfo info)
        {
            Task.Run(async () => await DownloadAndInstallAsync(info, null));
        }

        private bool IsNewerVersion(string latestStr, string currentStr)
        {
            try
            {
                Version latest;
                Version current;

                if (Version.TryParse(latestStr, out latest) && Version.TryParse(currentStr, out current))
                {
                    return latest > current;
                }

                string[] lParts = latestStr.Split('.');
                string[] cParts = currentStr.Split('.');
                int maxLen = Math.Max(lParts.Length, cParts.Length);

                for (int i = 0; i < maxLen; i++)
                {
                    int lNum = 0;
                    int cNum = 0;
                    if (i < lParts.Length) int.TryParse(lParts[i], out lNum);
                    if (i < cParts.Length) int.TryParse(cParts[i], out cNum);

                    if (lNum > cNum) return true;
                    if (lNum < cNum) return false;
                }
            }
            catch
            {
            }
            return false;
        }
    }
}
