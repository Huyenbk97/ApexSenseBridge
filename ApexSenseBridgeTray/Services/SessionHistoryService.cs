using ApexSenseBridgeTray.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace ApexSenseBridgeTray.Services
{
    /// <summary>
    /// Lưu và truy vấn lịch sử phiên chơi game.
    /// File JSON lưu tại %LocalAppData%/ApexSenseBridge/session_history.json.
    /// Giới hạn 500 bản ghi gần nhất để tránh file quá lớn.
    /// </summary>
    public sealed class SessionHistoryService
    {
        private const int MaxRecords = 500;

        private static readonly string HistoryFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ApexSenseBridge", "session_history.json");

        private readonly object syncLock = new object();
        private List<SessionRecord> records;
        private SessionRecord currentSession;

        /// <summary>Phát khi lịch sử thay đổi</summary>
        public event Action HistoryChanged;

        public SessionHistoryService()
        {
            records = new List<SessionRecord>();
            Load();
        }

        /// <summary>Bắt đầu ghi nhận session mới</summary>
        public void StartSession(string gameKey, string gameTitle, string profile,
                                  string controllerModel, int apexSlot, string owner)
        {
            lock (syncLock)
            {
                // Kết thúc session trước nếu còn đang chạy
                if (currentSession != null)
                {
                    currentSession.EndedAt = DateTime.UtcNow;
                    currentSession.CompletedSuccessfully = false;
                    currentSession.StopReason = "Replaced by new session";
                }

                currentSession = new SessionRecord
                {
                    GameKey = gameKey ?? string.Empty,
                    GameTitle = gameTitle ?? string.Empty,
                    Profile = profile ?? "standard",
                    ControllerModel = controllerModel ?? string.Empty,
                    StartedAt = DateTime.UtcNow,
                    ApexProfileSlot = apexSlot,
                    Owner = owner ?? "Tray"
                };
            }
        }

        /// <summary>Kết thúc session hiện tại</summary>
        public void EndSession(bool success, string reason)
        {
            lock (syncLock)
            {
                if (currentSession == null) return;

                currentSession.EndedAt = DateTime.UtcNow;
                currentSession.CompletedSuccessfully = success;
                currentSession.StopReason = reason ?? string.Empty;

                records.Add(currentSession);

                // Giới hạn số bản ghi
                while (records.Count > MaxRecords)
                    records.RemoveAt(0);

                currentSession = null;
            }
            Save();
            HistoryChanged?.Invoke();
        }

        /// <summary>Cập nhật controller model cho session đang chạy</summary>
        public void UpdateCurrentController(string model)
        {
            lock (syncLock)
            {
                if (currentSession != null)
                    currentSession.ControllerModel = model ?? string.Empty;
            }
        }

        /// <summary>Lấy tất cả bản ghi, mới nhất trước</summary>
        public List<SessionRecord> GetAllRecords()
        {
            lock (syncLock)
            {
                var result = new List<SessionRecord>(records);
                result.Reverse();
                return result;
            }
        }

        /// <summary>Lấy N bản ghi gần nhất</summary>
        public List<SessionRecord> GetRecentRecords(int count)
        {
            lock (syncLock)
            {
                var result = new List<SessionRecord>(records);
                result.Reverse();
                if (result.Count > count) result.RemoveRange(count, result.Count - count);
                return result;
            }
        }

        /// <summary>Lấy lịch sử theo game</summary>
        public List<SessionRecord> GetRecordsByGame(string gameKey)
        {
            lock (syncLock)
            {
                var result = new List<SessionRecord>();
                foreach (var r in records)
                {
                    if (string.Equals(r.GameKey, gameKey, StringComparison.OrdinalIgnoreCase))
                        result.Add(r);
                }
                result.Reverse();
                return result;
            }
        }

        /// <summary>Tổng thời gian chơi (giây)</summary>
        public double TotalPlayTimeSeconds
        {
            get
            {
                double total = 0;
                lock (syncLock)
                {
                    foreach (var r in records)
                        total += r.DurationSeconds;
                }
                return total;
            }
        }

        /// <summary>Tổng thời gian chơi định dạng dễ đọc (vd 2h 15m)</summary>
        public string GetTotalPlayTimeFormatted()
        {
            var ts = TimeSpan.FromSeconds(TotalPlayTimeSeconds);
            if (ts.TotalHours >= 1)
                return string.Format("{0}h {1:D2}m", (int)ts.TotalHours, ts.Minutes);
            if (ts.TotalMinutes >= 1)
                return string.Format("{0}m {1:D2}s", ts.Minutes, ts.Seconds);
            return string.Format("{0}s", (int)ts.TotalSeconds);
        }

        /// <summary>Tổng thời gian chơi cho 1 game (giây)</summary>
        public double GetPlayTimeByGame(string gameKey)
        {
            double total = 0;
            lock (syncLock)
            {
                foreach (var r in records)
                {
                    if (string.Equals(r.GameKey, gameKey, StringComparison.OrdinalIgnoreCase))
                        total += r.DurationSeconds;
                }
            }
            return total;
        }

        /// <summary>Tổng số session</summary>
        public int TotalSessions
        {
            get { lock (syncLock) return records.Count; }
        }

        /// <summary>Thống kê top game theo thời gian chơi</summary>
        public List<KeyValuePair<string, double>> GetTopGamesByPlayTime(int topN)
        {
            var grouped = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            lock (syncLock)
            {
                foreach (var r in records)
                {
                    if (!grouped.ContainsKey(r.GameTitle))
                        grouped[r.GameTitle] = 0;
                    grouped[r.GameTitle] += r.DurationSeconds;
                }
            }
            var sorted = grouped.ToList();
            sorted.Sort((a, b) => b.Value.CompareTo(a.Value));
            if (sorted.Count > topN) sorted.RemoveRange(topN, sorted.Count - topN);
            return sorted;
        }

        /// <summary>Thống kê theo controller model</summary>
        public Dictionary<string, int> GetSessionCountByController()
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            lock (syncLock)
            {
                foreach (var r in records)
                {
                    string key = string.IsNullOrEmpty(r.ControllerModel) ? "unknown" : r.ControllerModel;
                    if (!result.ContainsKey(key)) result[key] = 0;
                    result[key]++;
                }
            }
            return result;
        }

        /// <summary>Xóa toàn bộ lịch sử</summary>
        public void ClearHistory()
        {
            lock (syncLock)
            {
                records.Clear();
            }
            Save();
            HistoryChanged?.Invoke();
        }

        private void Load()
        {
            try
            {
                if (File.Exists(HistoryFilePath))
                {
                    string json = File.ReadAllText(HistoryFilePath);
                    var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                    var loaded = serializer.Deserialize<List<SessionRecord>>(json);
                    if (loaded != null)
                    {
                        lock (syncLock) records = loaded;
                    }
                }
            }
            catch
            {
                // Đọc thất bại — bắt đầu với list rỗng
            }
        }

        private void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(HistoryFilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string json;
                lock (syncLock)
                {
                    json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(records);
                }
                File.WriteAllText(HistoryFilePath, json);
            }
            catch
            {
                // Ghi thất bại không crash app
            }
        }
    }
}
