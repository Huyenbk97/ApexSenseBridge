using System;

namespace ApexSenseBridgeTray.Models
{
    /// <summary>
    /// Một bản ghi lịch sử phiên chơi game.
    /// </summary>
    public sealed class SessionRecord
    {
        /// <summary>Game đã chơi (normalized key)</summary>
        public string GameKey { get; set; }

        /// <summary>Tên hiển thị game</summary>
        public string GameTitle { get; set; }

        /// <summary>Profile sử dụng (standard, spider-man-2...)</summary>
        public string Profile { get; set; }

        /// <summary>Controller model đã dùng (apex4, apex5, apex6)</summary>
        public string ControllerModel { get; set; }

        /// <summary>Thời điểm bắt đầu session (UTC)</summary>
        public DateTime StartedAt { get; set; }

        /// <summary>Thời điểm kết thúc session (UTC), null nếu đang chạy</summary>
        public DateTime? EndedAt { get; set; }

        /// <summary>Thời gian chơi tính bằng giây</summary>
        public double DurationSeconds
        {
            get
            {
                if (EndedAt.HasValue)
                    return (EndedAt.Value - StartedAt).TotalSeconds;
                return (DateTime.UtcNow - StartedAt).TotalSeconds;
            }
        }

        /// <summary>Định dạng thời gian chơi dạng đọc được (VD: "1h 23m")</summary>
        public string DurationFormatted
        {
            get
            {
                var ts = TimeSpan.FromSeconds(DurationSeconds);
                if (ts.TotalHours >= 1)
                    return string.Format("{0}h {1:D2}m", (int)ts.TotalHours, ts.Minutes);
                if (ts.TotalMinutes >= 1)
                    return string.Format("{0}m {1:D2}s", ts.Minutes, ts.Seconds);
                return string.Format("{0}s", ts.Seconds);
            }
        }

        /// <summary>Session kết thúc thành công hay lỗi</summary>
        public bool CompletedSuccessfully { get; set; }

        /// <summary>Lý do kết thúc nếu lỗi</summary>
        public string StopReason { get; set; }

        /// <summary>APEX profile slot đã dùng (0 = không đổi)</summary>
        public int ApexProfileSlot { get; set; }

        /// <summary>Owner session (Tray hoặc Playnite)</summary>
        public string Owner { get; set; }

        public SessionRecord()
        {
            GameKey = string.Empty;
            GameTitle = string.Empty;
            Profile = "standard";
            ControllerModel = string.Empty;
            StartedAt = DateTime.UtcNow;
            CompletedSuccessfully = true;
            StopReason = string.Empty;
            Owner = "Tray";
        }
    }
}

