using ApexSenseBridgeTray.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace ApexSenseBridgeTray.Services
{
    /// <summary>
    /// Quản lý per-game custom profiles — lưu/load từ file JSON riêng.
    /// Mỗi game có thể có một profile tùy chỉnh trigger/vibration/haptic
    /// ghi đè lên cấu hình controller mặc định.
    /// </summary>
    public sealed class GameProfileService
    {
        private static readonly string ProfileFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ApexSenseBridge", "game_profiles.json");

        private readonly object syncLock = new object();
        private Dictionary<string, GameProfile> profiles;

        /// <summary>Phát khi profile thay đổi</summary>
        public event Action ProfilesChanged;

        public GameProfileService()
        {
            profiles = new Dictionary<string, GameProfile>(StringComparer.OrdinalIgnoreCase);
            Load();
        }

        /// <summary>Lấy profile cho game cụ thể, null nếu chưa tạo</summary>
        public GameProfile GetProfile(string gameKey)
        {
            if (string.IsNullOrWhiteSpace(gameKey)) return null;
            lock (syncLock)
            {
                GameProfile profile;
                return profiles.TryGetValue(gameKey, out profile) ? profile : null;
            }
        }

        /// <summary>Lấy hoặc tạo mới profile cho game</summary>
        public GameProfile GetOrCreateProfile(string gameKey, string displayName)
        {
            if (string.IsNullOrWhiteSpace(gameKey)) return null;
            lock (syncLock)
            {
                GameProfile profile;
                if (!profiles.TryGetValue(gameKey, out profile))
                {
                    profile = new GameProfile
                    {
                        GameKey = gameKey,
                        DisplayName = displayName ?? gameKey
                    };
                    profiles[gameKey] = profile;
                }
                return profile;
            }
        }

        /// <summary>Lưu profile đã chỉnh sửa</summary>
        public void SaveProfile(GameProfile profile)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.GameKey)) return;
            lock (syncLock)
            {
                profile.ModifiedAt = DateTime.UtcNow;
                profiles[profile.GameKey] = profile;
            }
            Save();
            ProfilesChanged?.Invoke();
        }

        /// <summary>Xóa profile của game, quay về default</summary>
        public void DeleteProfile(string gameKey)
        {
            if (string.IsNullOrWhiteSpace(gameKey)) return;
            lock (syncLock)
            {
                profiles.Remove(gameKey);
            }
            Save();
            ProfilesChanged?.Invoke();
        }

        /// <summary>Kiểm tra game có custom profile không</summary>
        public bool HasProfile(string gameKey)
        {
            if (string.IsNullOrWhiteSpace(gameKey)) return false;
            lock (syncLock)
            {
                return profiles.ContainsKey(gameKey);
            }
        }

        /// <summary>Lấy tất cả profiles</summary>
        public List<GameProfile> GetAllProfiles()
        {
            lock (syncLock)
            {
                return new List<GameProfile>(profiles.Values);
            }
        }

        /// <summary>Số lượng game có custom profile</summary>
        public int Count
        {
            get { lock (syncLock) return profiles.Count; }
        }

        /// <summary>
        /// Giải quyết calibration cuối cùng cho game: nếu có per-game profile
        /// thì áp dụng, nếu không dùng controller calibration mặc định.
        /// </summary>
        public ControllerCalibration ResolveCalibration(string gameKey, ControllerCalibration controllerCalibration, string model)
        {
            var profile = GetProfile(gameKey);
            if (profile != null && profile.HasCustomValues)
            {
                return profile.ApplyTo(controllerCalibration, model);
            }
            return controllerCalibration;
        }

        /// <summary>Xuất profile của 1 game ra file JSON</summary>
        public bool ExportProfile(string gameKey, string targetPath)
        {
            try
            {
                var profile = GetProfile(gameKey);
                if (profile == null) return false;
                string json = new JavaScriptSerializer().Serialize(profile);
                File.WriteAllText(targetPath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Nhập profile từ file JSON và lưu vào hệ thống</summary>
        public GameProfile ImportProfile(string sourcePath)
        {
            try
            {
                if (!File.Exists(sourcePath)) return null;
                string json = File.ReadAllText(sourcePath);
                var profile = new JavaScriptSerializer().Deserialize<GameProfile>(json);
                if (profile != null && !string.IsNullOrWhiteSpace(profile.GameKey))
                {
                    SaveProfile(profile);
                    return profile;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Xuất toàn bộ cấu hình profile ra file JSON backup</summary>
        public bool ExportAllProfiles(string targetPath)
        {
            try
            {
                string json;
                lock (syncLock)
                {
                    json = new JavaScriptSerializer().Serialize(profiles);
                }
                File.WriteAllText(targetPath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Khôi phục tất cả profile từ file JSON backup</summary>
        public int ImportAllProfiles(string sourcePath)
        {
            try
            {
                if (!File.Exists(sourcePath)) return 0;
                string json = File.ReadAllText(sourcePath);
                var loaded = new JavaScriptSerializer().Deserialize<Dictionary<string, GameProfile>>(json);
                if (loaded != null && loaded.Count > 0)
                {
                    lock (syncLock)
                    {
                        foreach (var kv in loaded)
                        {
                            profiles[kv.Key] = kv.Value;
                        }
                    }
                    Save();
                    ProfilesChanged?.Invoke();
                    return loaded.Count;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private void Load()
        {
            try
            {
                if (File.Exists(ProfileFilePath))
                {
                    string json = File.ReadAllText(ProfileFilePath);
                    var serializer = new JavaScriptSerializer();
                    var loaded = serializer.Deserialize<Dictionary<string, GameProfile>>(json);
                    if (loaded != null)
                    {
                        lock (syncLock)
                        {
                            profiles = new Dictionary<string, GameProfile>(loaded, StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }
            }
            catch
            {
                // Nếu đọc thất bại, dùng dictionary rỗng
            }
        }

        private void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(ProfileFilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string json;
                lock (syncLock)
                {
                    json = new JavaScriptSerializer().Serialize(profiles);
                }
                File.WriteAllText(ProfileFilePath, json);
            }
            catch
            {
                // Ghi thất bại không crash app
            }
        }
    }
}
