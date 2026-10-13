using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ApexSenseBridgeTray.Common
{
    public static class CoverCacheService
    {
        private static readonly string CacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ApexSenseBridge", "cache", "covers");

        private static readonly ConcurrentDictionary<string, ImageSource> MemoryCache =
            new ConcurrentDictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase);

        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };

        static CoverCacheService()
        {
            try
            {
                if (!Directory.Exists(CacheDir))
                {
                    Directory.CreateDirectory(CacheDir);
                }
                HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ApexSenseBridgeTray/1.0");
            }
            catch { }
        }

        public static ImageSource GetImage(string url, Action<ImageSource> onLoaded)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            // 1. Check in-memory cache
            ImageSource memImage;
            if (MemoryCache.TryGetValue(url, out memImage))
            {
                return memImage;
            }

            // 2. Check local disk cache
            string fileName = GetCacheFileName(url);
            string localPath = Path.Combine(CacheDir, fileName);

            if (File.Exists(localPath))
            {
                try
                {
                    var fileInfo = new FileInfo(localPath);
                    if (fileInfo.Length > 200)
                    {
                        var bmp = LoadBitmapFromFile(localPath);
                        if (bmp != null)
                        {
                            MemoryCache[url] = bmp;
                            return bmp;
                        }
                    }
                }
                catch { }
            }

            // 2b. Check application bundled covers directory (assets/covers/)
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string bundledCoversDir = Path.Combine(appDir, "assets", "covers");
                if (Directory.Exists(bundledCoversDir))
                {
                    string bundledPath = Path.Combine(bundledCoversDir, fileName);
                    if (File.Exists(bundledPath))
                    {
                        var bmp = LoadBitmapFromFile(bundledPath);
                        if (bmp != null)
                        {
                            MemoryCache[url] = bmp;
                            return bmp;
                        }
                    }
                }
            }
            catch { }

            // 3. Download in background if not found locally
            Task.Run(async () =>
            {
                try
                {
                    byte[] data = await DownloadCoverBytesAsync(url);
                    if (data != null && data.Length > 200)
                    {
                        try
                        {
                            File.WriteAllBytes(localPath, data);
                        }
                        catch { }

                        Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                var bmp = LoadBitmapFromBytes(data);
                                if (bmp != null)
                                {
                                    MemoryCache[url] = bmp;
                                    onLoaded?.Invoke(bmp);
                                }
                            }
                            catch { }
                        }));
                    }
                }
                catch
                {
                    // Fail silently, fallback placeholder stays active
                }
            });

            return null;
        }

        private static async Task<byte[]> DownloadCoverBytesAsync(string url)
        {
            try
            {
                var data = await HttpClient.GetByteArrayAsync(url);
                if (data != null && data.Length > 200) return data;
            }
            catch
            {
                // Fallback nếu Steam CDN library_600x900_2x.jpg gặp sự cố
                if (url.Contains("/library_600x900_2x.jpg"))
                {
                    var altUrl = url.Replace("/library_600x900_2x.jpg", "/header.jpg");
                    try
                    {
                        var altData = await HttpClient.GetByteArrayAsync(altUrl);
                        if (altData != null && altData.Length > 200) return altData;
                    }
                    catch { }
                }
            }
            return null;
        }

        private static BitmapSource LoadBitmapFromFile(string path)
        {
            try
            {
                var bytes = File.ReadAllBytes(path);
                return LoadBitmapFromBytes(bytes);
            }
            catch
            {
                return null;
            }
        }

        private static BitmapSource LoadBitmapFromBytes(byte[] bytes)
        {
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = ms;
                    bmp.EndInit();
                    bmp.Freeze();
                    return bmp;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string GetCacheFileName(string url)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(url));
                var sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                sb.Append(".jpg");
                return sb.ToString();
            }
        }
    }
}
