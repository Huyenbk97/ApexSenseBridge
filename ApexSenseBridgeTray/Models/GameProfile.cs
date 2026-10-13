using System;

namespace ApexSenseBridgeTray.Models
{
    /// <summary>
    /// Cấu hình riêng cho từng game, ghi đè lên ControllerCalibration mặc định.
    /// Chỉ lưu các giá trị người dùng đã tùy chỉnh; giá trị null = dùng default.
    /// </summary>
    public sealed class GameProfile
    {
        /// <summary>Khóa game (normalized title)</summary>
        public string GameKey { get; set; }

        /// <summary>Tên hiển thị</summary>
        public string DisplayName { get; set; }

        /// <summary>Trigger strength riêng (0-100%), null = dùng mặc định controller</summary>
        public int? TriggerStrengthPercent { get; set; }

        /// <summary>Vibration strength riêng (0-200%), null = dùng mặc định controller</summary>
        public int? VibrationStrengthPercent { get; set; }

        /// <summary>Haptic threshold riêng (0-95%), null = dùng mặc định controller</summary>
        public int? HapticThresholdPercent { get; set; }

        /// <summary>Bật/tắt rumble riêng, null = dùng mặc định controller</summary>
        public bool? EnableRumble { get; set; }

        /// <summary>Thời gian tạo profile</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Thời gian sửa cuối</summary>
        public DateTime ModifiedAt { get; set; }

        public GameProfile()
        {
            GameKey = string.Empty;
            DisplayName = string.Empty;
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Áp dụng profile game lên calibration controller, giữ nguyên giá trị
        /// controller cho những thuộc tính chưa tùy chỉnh.
        /// </summary>
        public ControllerCalibration ApplyTo(ControllerCalibration baseCalibration, string model)
        {
            if (baseCalibration == null) return null;
            var result = new ControllerCalibration
            {
                TriggerStrengthPercent = TriggerStrengthPercent ?? baseCalibration.TriggerStrengthPercent,
                VibrationStrengthPercent = VibrationStrengthPercent ?? baseCalibration.VibrationStrengthPercent,
                HapticThresholdPercent = HapticThresholdPercent ?? baseCalibration.HapticThresholdPercent,
                EnableRumble = EnableRumble ?? baseCalibration.EnableRumble,
                SyncLightbar = baseCalibration.SyncLightbar,
                GyroStrengthPercent = baseCalibration.GyroStrengthPercent,
                GyroYawStrengthPercent = baseCalibration.GyroYawStrengthPercent
            };
            result.Normalize(model);
            return result;
        }

        /// <summary>Kiểm tra profile có tùy chỉnh gì không hay chỉ dùng default</summary>
        public bool HasCustomValues
        {
            get
            {
                return TriggerStrengthPercent.HasValue ||
                       VibrationStrengthPercent.HasValue ||
                       HapticThresholdPercent.HasValue ||
                       EnableRumble.HasValue;
            }
        }

        public const string PresetSubtle = "subtle";
        public const string PresetBalanced = "balanced";
        public const string PresetIntense = "intense";

        /// <summary>Áp dụng bộ thông số preset chuẩn</summary>
        public void ApplyPreset(string presetName)
        {
            switch (presetName?.ToLowerInvariant())
            {
                case PresetSubtle:
                    TriggerStrengthPercent = 50;
                    VibrationStrengthPercent = 40;
                    HapticThresholdPercent = 20;
                    EnableRumble = true;
                    break;
                case PresetIntense:
                    TriggerStrengthPercent = 100;
                    VibrationStrengthPercent = 160;
                    HapticThresholdPercent = 8;
                    EnableRumble = true;
                    break;
                case PresetBalanced:
                default:
                    TriggerStrengthPercent = 100;
                    VibrationStrengthPercent = 100;
                    HapticThresholdPercent = 12;
                    EnableRumble = true;
                    break;
            }
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>Xóa tất cả tùy chỉnh, quay về default controller</summary>
        public void ResetToDefaults()
        {
            TriggerStrengthPercent = null;
            VibrationStrengthPercent = null;
            HapticThresholdPercent = null;
            EnableRumble = null;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
