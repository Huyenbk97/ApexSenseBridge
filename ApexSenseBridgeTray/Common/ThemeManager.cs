using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;

namespace ApexSenseBridgeTray.Common
{
    public static class ThemeManager
    {
        public const string ThemePlayStation = "playstation";
        public const string ThemeXbox = "xbox";
        public const string ThemeCyberpunk = "cyberpunk";
        public const string ThemeOled = "oled";
        public const string ThemeLight = "light";
        public const string ThemeSystem = "system";

        private static string currentTheme = ThemePlayStation;
        private static string customAccentColor = null;
        private static bool isDarkTheme = true;

        public static string CurrentTheme => currentTheme;
        public static string CustomAccentColor => customAccentColor;
        public static bool IsDarkTheme => isDarkTheme;

        public static event Action ThemeChanged;

        public static void Initialize(string configuredTheme = null, string configuredAccent = null)
        {
            if (!string.IsNullOrWhiteSpace(configuredTheme))
            {
                currentTheme = configuredTheme.ToLowerInvariant();
            }
            if (!string.IsNullOrWhiteSpace(configuredAccent))
            {
                customAccentColor = configuredAccent;
            }
            UpdateTheme();
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        public static void SetTheme(string themeKey, bool clearCustomAccent = true)
        {
            if (string.IsNullOrWhiteSpace(themeKey)) themeKey = ThemePlayStation;
            currentTheme = themeKey.ToLowerInvariant();
            if (clearCustomAccent)
            {
                customAccentColor = null;
            }
            UpdateTheme();
            ThemeChanged?.Invoke();
        }

        public static void SetCustomAccent(string hexColor)
        {
            customAccentColor = hexColor;
            UpdateTheme();
            ThemeChanged?.Invoke();
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General || e.Category == UserPreferenceCategory.Color)
            {
                if (Application.Current != null && Application.Current.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (currentTheme == ThemeSystem)
                        {
                            UpdateTheme();
                            ThemeChanged?.Invoke();
                        }
                    }));
                }
            }
        }

        public static void UpdateTheme()
        {
            bool isSystemDark = QueryWindowsDarkTheme();
            string effectiveTheme = currentTheme;
            if (effectiveTheme == ThemeSystem)
            {
                effectiveTheme = isSystemDark ? ThemePlayStation : ThemeLight;
            }

            isDarkTheme = (effectiveTheme != ThemeLight);

            if (Application.Current != null)
            {
                if (Application.Current.Resources != null)
                {
                    ApplyThemeToResources(Application.Current.Resources, effectiveTheme);
                }

                foreach (Window w in Application.Current.Windows)
                {
                    if (w != null && w.Resources != null)
                    {
                        ApplyThemeToResources(w.Resources, effectiveTheme);
                    }
                }
            }
        }

        public static void ApplyTheme(Window window)
        {
            if (window == null || window.Resources == null) return;
            bool isSystemDark = QueryWindowsDarkTheme();
            string effectiveTheme = currentTheme;
            if (effectiveTheme == ThemeSystem)
            {
                effectiveTheme = isSystemDark ? ThemePlayStation : ThemeLight;
            }
            ApplyThemeToResources(window.Resources, effectiveTheme);
        }

        private static bool QueryWindowsDarkTheme()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("AppsUseLightTheme");
                        if (value is int)
                        {
                            int lightTheme = (int)value;
                            return lightTheme == 0;
                        }
                    }
                }
            }
            catch
            {
            }
            return true;
        }

        private static LinearGradientBrush CreateLinearGradient(Color c1, Color c2, Color c3)
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };
            brush.GradientStops.Add(new GradientStop(c1, 0));
            brush.GradientStops.Add(new GradientStop(c2, 0.45));
            brush.GradientStops.Add(new GradientStop(c3, 1));
            brush.Freeze();
            return brush;
        }

        private static RadialGradientBrush CreateRadialGradient(Color c1, Color c2, Point center, double rx, double ry)
        {
            var brush = new RadialGradientBrush
            {
                Center = center,
                GradientOrigin = center,
                RadiusX = rx,
                RadiusY = ry
            };
            brush.GradientStops.Add(new GradientStop(c1, 0));
            brush.GradientStops.Add(new GradientStop(c2, 1));
            brush.Freeze();
            return brush;
        }

        private static void ApplyThemeToResources(ResourceDictionary res, string theme)
        {
            ApplyThemeToResources(res, theme, new System.Collections.Generic.HashSet<ResourceDictionary>());
        }

        private static void ApplyThemeToResources(ResourceDictionary res, string theme, System.Collections.Generic.HashSet<ResourceDictionary> visited)
        {
            if (res == null || !visited.Add(res)) return;

            switch (theme)
            {
                case ThemeXbox:
                    ApplyXboxTheme(res);
                    break;
                case ThemeCyberpunk:
                    ApplyCyberpunkTheme(res);
                    break;
                case ThemeOled:
                    ApplyOledTheme(res);
                    break;
                case ThemeLight:
                    ApplyLightTheme(res);
                    break;
                case ThemePlayStation:
                default:
                    ApplyPlayStationTheme(res);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(customAccentColor) && TryParseHexColor(customAccentColor, out Color customCol))
            {
                ApplyAccentOverride(res, customCol);
            }

            if (res.MergedDictionaries != null)
            {
                foreach (var md in res.MergedDictionaries)
                {
                    ApplyThemeToResources(md, theme, visited);
                }
            }
        }

        public static bool TryParseHexColor(string hex, out Color color)
        {
            color = Colors.Transparent;
            if (string.IsNullOrWhiteSpace(hex)) return false;
            try
            {
                hex = hex.Trim().TrimStart('#');
                if (hex.Length == 6)
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                    color = Color.FromRgb(r, g, b);
                    return true;
                }
                if (hex.Length == 8)
                {
                    byte a = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte r = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(4, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(6, 2), 16);
                    color = Color.FromArgb(a, r, g, b);
                    return true;
                }
            }
            catch { }
            return false;
        }

        private static void ApplyAccentOverride(ResourceDictionary res, Color accent)
        {
            Color pressed = Color.FromRgb((byte)(accent.R * 0.85), (byte)(accent.G * 0.85), (byte)(accent.B * 0.85));
            Color active = Color.FromRgb((byte)(accent.R * 0.70), (byte)(accent.G * 0.70), (byte)(accent.B * 0.70));

            res["PlayStationBlue"] = new SolidColorBrush(accent);
            res["PlayStationBluePressed"] = new SolidColorBrush(pressed);
            res["PlayStationBlueActive"] = new SolidColorBrush(active);
            res["TileHoverBorder"] = new SolidColorBrush(Color.FromArgb(0x66, accent.R, accent.G, accent.B));
            res["GamepadFocusBorder"] = new SolidColorBrush(accent);
            res["GamepadFocusGlow"] = new SolidColorBrush(Color.FromArgb(0x40, accent.R, accent.G, accent.B));
            res["CardBorder"] = new SolidColorBrush(Color.FromArgb(0x28, accent.R, accent.G, accent.B));

            res["AmbientLightTopBrush"] = CreateRadialGradient(Color.FromArgb(0x60, accent.R, accent.G, accent.B), Color.FromArgb(0x00, accent.R, accent.G, accent.B), new Point(0.88, 0), 0.45, 0.7);
            res["AmbientLightBottomBrush"] = CreateRadialGradient(Color.FromArgb(0x35, (byte)(accent.R * 0.7), (byte)(accent.G * 0.7), (byte)(accent.B * 0.7)), Color.FromArgb(0x00, accent.R, accent.G, accent.B), new Point(0, 1), 0.4, 0.55);
        }

        private static void ApplyPlayStationTheme(ResourceDictionary res)
        {
            res["ConsoleBackdrop"] = CreateLinearGradient(Color.FromRgb(0x0E, 0x1D, 0x45), Color.FromRgb(0x08, 0x11, 0x28), Color.FromRgb(0x04, 0x06, 0x0E));
            res["AmbientLightTopBrush"] = CreateRadialGradient(Color.FromArgb(0x55, 0x2A, 0x6B, 0xFF), Color.FromArgb(0x00, 0x2A, 0x6B, 0xFF), new Point(0.88, 0), 0.45, 0.7);
            res["AmbientLightBottomBrush"] = CreateRadialGradient(Color.FromArgb(0x33, 0x16, 0x3C, 0x9E), Color.FromArgb(0x00, 0x16, 0x3C, 0x9E), new Point(0, 1), 0.4, 0.55);

            res["WindowBackground"] = new SolidColorBrush(Color.FromRgb(0x06, 0x0A, 0x16));
            res["CardBackground"] = new SolidColorBrush(Color.FromArgb(0xB3, 0x11, 0x1A, 0x31));
            res["CardBorder"] = new SolidColorBrush(Color.FromArgb(0x14, 0xFF, 0xFF, 0xFF));
            res["FooterBackground"] = new SolidColorBrush(Color.FromArgb(0xEE, 0x07, 0x0A, 0x14));
            res["ControlBackground"] = new SolidColorBrush(Color.FromRgb(0x1B, 0x25, 0x40));
            res["ControlBorder"] = new SolidColorBrush(Color.FromRgb(0x3A, 0x47, 0x68));
            res["TextPrimary"] = new SolidColorBrush(Color.FromRgb(0xF4, 0xF6, 0xFB));
            res["TextSecondary"] = new SolidColorBrush(Color.FromRgb(0xAA, 0xB4, 0xCC));
            res["TextMuted"] = new SolidColorBrush(Color.FromRgb(0x6E, 0x79, 0x95));

            res["TrackBackground"] = new SolidColorBrush(Color.FromRgb(0x2A, 0x35, 0x52));
            res["SurfaceSubtle"] = new SolidColorBrush(Color.FromArgb(0x10, 0xFF, 0xFF, 0xFF));
            res["SurfaceHover"] = new SolidColorBrush(Color.FromArgb(0x1C, 0xFF, 0xFF, 0xFF));
            res["TileHoverBorder"] = new SolidColorBrush(Color.FromArgb(0x55, 0x00, 0x70, 0xD1));

            res["BadgeActiveBg"] = new SolidColorBrush(Color.FromArgb(0x28, 0x10, 0xB9, 0x81));
            res["BadgeActiveFg"] = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
            res["BadgeStandbyBg"] = new SolidColorBrush(Color.FromArgb(0x24, 0x00, 0x70, 0xD1));
            res["BadgeStandbyFg"] = new SolidColorBrush(Color.FromRgb(0x38, 0xBD, 0xF8));

            res["FeaturePillBg"] = new SolidColorBrush(Color.FromArgb(0x18, 0xFF, 0xFF, 0xFF));
            res["FeaturePillBorder"] = Brushes.Transparent;

            res["GamepadFocusBorder"] = new SolidColorBrush(Color.FromRgb(0x00, 0x70, 0xD1));
            res["GamepadFocusGlow"] = new SolidColorBrush(Color.FromArgb(0x30, 0x00, 0x70, 0xD1));
            res["GamepadHudBg"] = new SolidColorBrush(Color.FromRgb(0x07, 0x0A, 0x14));
            res["GamepadBadgeBg"] = new SolidColorBrush(Color.FromRgb(0x18, 0x22, 0x38));
            res["GamepadBadgeBorder"] = Brushes.Transparent;
            res["GamepadBadgeFg"] = new SolidColorBrush(Color.FromRgb(0xEE, 0xEE, 0xF2));

            res["PlayStationBlue"] = new SolidColorBrush(Color.FromRgb(0x00, 0x70, 0xD1));
            res["PlayStationBluePressed"] = new SolidColorBrush(Color.FromRgb(0x00, 0x5B, 0xAA));
            res["PlayStationBlueActive"] = new SolidColorBrush(Color.FromRgb(0x00, 0x4D, 0x8D));
            res["TextOnPrimary"] = new SolidColorBrush(Colors.White);
        }

        private static void ApplyXboxTheme(ResourceDictionary res)
        {
            res["ConsoleBackdrop"] = CreateLinearGradient(Color.FromRgb(0x0D, 0x1A, 0x12), Color.FromRgb(0x07, 0x12, 0x0A), Color.FromRgb(0x03, 0x08, 0x04));
            res["AmbientLightTopBrush"] = CreateRadialGradient(Color.FromArgb(0x55, 0x10, 0x7C, 0x10), Color.FromArgb(0x00, 0x10, 0x7C, 0x10), new Point(0.88, 0), 0.45, 0.7);
            res["AmbientLightBottomBrush"] = CreateRadialGradient(Color.FromArgb(0x30, 0x0B, 0x4E, 0x0B), Color.FromArgb(0x00, 0x0B, 0x4E, 0x0B), new Point(0, 1), 0.4, 0.55);

            res["WindowBackground"] = new SolidColorBrush(Color.FromRgb(0x05, 0x09, 0x06));
            res["CardBackground"] = new SolidColorBrush(Color.FromArgb(0xD9, 0x0F, 0x1A, 0x12));
            res["CardBorder"] = new SolidColorBrush(Color.FromArgb(0x22, 0x10, 0x7C, 0x10));
            res["FooterBackground"] = new SolidColorBrush(Color.FromArgb(0xEE, 0x05, 0x0B, 0x07));
            res["ControlBackground"] = new SolidColorBrush(Color.FromRgb(0x13, 0x22, 0x17));
            res["ControlBorder"] = new SolidColorBrush(Color.FromRgb(0x23, 0x45, 0x29));
            res["TextPrimary"] = new SolidColorBrush(Color.FromRgb(0xF0, 0xFD, 0xF4));
            res["TextSecondary"] = new SolidColorBrush(Color.FromRgb(0x86, 0xEF, 0xAC));
            res["TextMuted"] = new SolidColorBrush(Color.FromRgb(0x4A, 0x7C, 0x59));

            res["TrackBackground"] = new SolidColorBrush(Color.FromRgb(0x1B, 0x2E, 0x1F));
            res["SurfaceSubtle"] = new SolidColorBrush(Color.FromArgb(0x14, 0x10, 0x7C, 0x10));
            res["SurfaceHover"] = new SolidColorBrush(Color.FromArgb(0x26, 0x10, 0x7C, 0x10));
            res["TileHoverBorder"] = new SolidColorBrush(Color.FromArgb(0x66, 0x10, 0x7C, 0x10));

            res["BadgeActiveBg"] = new SolidColorBrush(Color.FromArgb(0x28, 0x10, 0x7C, 0x10));
            res["BadgeActiveFg"] = new SolidColorBrush(Color.FromRgb(0x22, 0xC5, 0x5E));
            res["BadgeStandbyBg"] = new SolidColorBrush(Color.FromArgb(0x24, 0x10, 0x7C, 0x10));
            res["BadgeStandbyFg"] = new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80));

            res["FeaturePillBg"] = new SolidColorBrush(Color.FromArgb(0x18, 0x10, 0x7C, 0x10));
            res["FeaturePillBorder"] = Brushes.Transparent;

            res["GamepadFocusBorder"] = new SolidColorBrush(Color.FromRgb(0x10, 0x7C, 0x10));
            res["GamepadFocusGlow"] = new SolidColorBrush(Color.FromArgb(0x35, 0x10, 0x7C, 0x10));
            res["GamepadHudBg"] = new SolidColorBrush(Color.FromRgb(0x06, 0x0B, 0x07));
            res["GamepadBadgeBg"] = new SolidColorBrush(Color.FromRgb(0x13, 0x24, 0x18));
            res["GamepadBadgeBorder"] = Brushes.Transparent;
            res["GamepadBadgeFg"] = new SolidColorBrush(Color.FromRgb(0xEE, 0xEE, 0xF2));

            res["PlayStationBlue"] = new SolidColorBrush(Color.FromRgb(0x10, 0x7C, 0x10));
            res["PlayStationBluePressed"] = new SolidColorBrush(Color.FromRgb(0x0E, 0x6B, 0x0E));
            res["PlayStationBlueActive"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x55, 0x0B));
            res["TextOnPrimary"] = new SolidColorBrush(Colors.White);
        }

        private static void ApplyCyberpunkTheme(ResourceDictionary res)
        {
            res["ConsoleBackdrop"] = CreateLinearGradient(Color.FromRgb(0x26, 0x0D, 0x33), Color.FromRgb(0x15, 0x07, 0x22), Color.FromRgb(0x09, 0x03, 0x10));
            res["AmbientLightTopBrush"] = CreateRadialGradient(Color.FromArgb(0x5A, 0xEC, 0x48, 0x99), Color.FromArgb(0x00, 0xEC, 0x48, 0x99), new Point(0.88, 0), 0.45, 0.7);
            res["AmbientLightBottomBrush"] = CreateRadialGradient(Color.FromArgb(0x45, 0x8B, 0x5C, 0xF6), Color.FromArgb(0x00, 0x8B, 0x5C, 0xF6), new Point(0, 1), 0.4, 0.55);

            res["WindowBackground"] = new SolidColorBrush(Color.FromRgb(0x0A, 0x04, 0x12));
            res["CardBackground"] = new SolidColorBrush(Color.FromArgb(0xD9, 0x20, 0x10, 0x33));
            res["CardBorder"] = new SolidColorBrush(Color.FromArgb(0x28, 0xEC, 0x48, 0x99));
            res["FooterBackground"] = new SolidColorBrush(Color.FromArgb(0xEE, 0x0A, 0x04, 0x14));
            res["ControlBackground"] = new SolidColorBrush(Color.FromRgb(0x2B, 0x16, 0x44));
            res["ControlBorder"] = new SolidColorBrush(Color.FromRgb(0x52, 0x2C, 0x7D));
            res["TextPrimary"] = new SolidColorBrush(Color.FromRgb(0xFD, 0xF2, 0xF8));
            res["TextSecondary"] = new SolidColorBrush(Color.FromRgb(0xF4, 0x72, 0xB6));
            res["TextMuted"] = new SolidColorBrush(Color.FromRgb(0xA8, 0x55, 0x8A));

            res["TrackBackground"] = new SolidColorBrush(Color.FromRgb(0x3B, 0x1F, 0x52));
            res["SurfaceSubtle"] = new SolidColorBrush(Color.FromArgb(0x18, 0xEC, 0x48, 0x99));
            res["SurfaceHover"] = new SolidColorBrush(Color.FromArgb(0x2D, 0xEC, 0x48, 0x99));
            res["TileHoverBorder"] = new SolidColorBrush(Color.FromArgb(0x66, 0xEC, 0x48, 0x99));

            res["BadgeActiveBg"] = new SolidColorBrush(Color.FromArgb(0x28, 0x10, 0xB9, 0x81));
            res["BadgeActiveFg"] = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
            res["BadgeStandbyBg"] = new SolidColorBrush(Color.FromArgb(0x24, 0xEC, 0x48, 0x99));
            res["BadgeStandbyFg"] = new SolidColorBrush(Color.FromRgb(0xF4, 0x72, 0xB6));

            res["FeaturePillBg"] = new SolidColorBrush(Color.FromArgb(0x22, 0xEC, 0x48, 0x99));
            res["FeaturePillBorder"] = Brushes.Transparent;

            res["GamepadFocusBorder"] = new SolidColorBrush(Color.FromRgb(0xEC, 0x48, 0x99));
            res["GamepadFocusGlow"] = new SolidColorBrush(Color.FromArgb(0x40, 0xEC, 0x48, 0x99));
            res["GamepadHudBg"] = new SolidColorBrush(Color.FromRgb(0x0C, 0x05, 0x16));
            res["GamepadBadgeBg"] = new SolidColorBrush(Color.FromRgb(0x2D, 0x16, 0x42));
            res["GamepadBadgeBorder"] = Brushes.Transparent;
            res["GamepadBadgeFg"] = new SolidColorBrush(Color.FromRgb(0xEE, 0xEE, 0xF2));

            res["PlayStationBlue"] = new SolidColorBrush(Color.FromRgb(0xEC, 0x48, 0x99));
            res["PlayStationBluePressed"] = new SolidColorBrush(Color.FromRgb(0xDB, 0x27, 0x77));
            res["PlayStationBlueActive"] = new SolidColorBrush(Color.FromRgb(0xBE, 0x18, 0x5D));
            res["TextOnPrimary"] = new SolidColorBrush(Colors.White);
        }

        private static void ApplyOledTheme(ResourceDictionary res)
        {
            res["ConsoleBackdrop"] = CreateLinearGradient(Color.FromRgb(0x0C, 0x0C, 0x0E), Color.FromRgb(0x05, 0x05, 0x06), Color.FromRgb(0x00, 0x00, 0x00));
            res["AmbientLightTopBrush"] = CreateRadialGradient(Color.FromArgb(0x18, 0xFF, 0xFF, 0xFF), Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), new Point(0.88, 0), 0.45, 0.7);
            res["AmbientLightBottomBrush"] = CreateRadialGradient(Color.FromArgb(0x10, 0xFF, 0xFF, 0xFF), Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), new Point(0, 1), 0.4, 0.55);

            res["WindowBackground"] = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            res["CardBackground"] = new SolidColorBrush(Color.FromRgb(0x0C, 0x0C, 0x0E));
            res["CardBorder"] = new SolidColorBrush(Color.FromArgb(0x1F, 0xFF, 0xFF, 0xFF));
            res["FooterBackground"] = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            res["ControlBackground"] = new SolidColorBrush(Color.FromRgb(0x16, 0x16, 0x1A));
            res["ControlBorder"] = new SolidColorBrush(Color.FromRgb(0x2E, 0x2E, 0x38));
            res["TextPrimary"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            res["TextSecondary"] = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));
            res["TextMuted"] = new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B));

            res["TrackBackground"] = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x28));
            res["SurfaceSubtle"] = new SolidColorBrush(Color.FromArgb(0x12, 0xFF, 0xFF, 0xFF));
            res["SurfaceHover"] = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF));
            res["TileHoverBorder"] = new SolidColorBrush(Color.FromArgb(0x55, 0x94, 0xA3, 0xB8));

            res["BadgeActiveBg"] = new SolidColorBrush(Color.FromArgb(0x28, 0x10, 0xB9, 0x81));
            res["BadgeActiveFg"] = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
            res["BadgeStandbyBg"] = new SolidColorBrush(Color.FromArgb(0x28, 0x94, 0xA3, 0xB8));
            res["BadgeStandbyFg"] = new SolidColorBrush(Color.FromRgb(0xCB, 0xD5, 0xE1));

            res["FeaturePillBg"] = new SolidColorBrush(Color.FromArgb(0x18, 0xFF, 0xFF, 0xFF));
            res["FeaturePillBorder"] = Brushes.Transparent;

            res["GamepadFocusBorder"] = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));
            res["GamepadFocusGlow"] = new SolidColorBrush(Color.FromArgb(0x30, 0x94, 0xA3, 0xB8));
            res["GamepadHudBg"] = new SolidColorBrush(Color.FromRgb(0x05, 0x05, 0x06));
            res["GamepadBadgeBg"] = new SolidColorBrush(Color.FromRgb(0x18, 0x18, 0x20));
            res["GamepadBadgeBorder"] = Brushes.Transparent;
            res["GamepadBadgeFg"] = new SolidColorBrush(Color.FromRgb(0xEE, 0xEE, 0xF2));

            res["PlayStationBlue"] = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));
            res["PlayStationBluePressed"] = new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B));
            res["PlayStationBlueActive"] = new SolidColorBrush(Color.FromRgb(0x47, 0x55, 0x69));
            res["TextOnPrimary"] = new SolidColorBrush(Colors.Black);
        }

        private static void ApplyLightTheme(ResourceDictionary res)
        {
            res["ConsoleBackdrop"] = CreateLinearGradient(Color.FromRgb(0xF8, 0xFA, 0xFC), Color.FromRgb(0xF1, 0xF5, 0xF9), Color.FromRgb(0xE2, 0xE8, 0xF0));
            res["AmbientLightTopBrush"] = CreateRadialGradient(Color.FromArgb(0x28, 0x00, 0x70, 0xD1), Color.FromArgb(0x00, 0x00, 0x70, 0xD1), new Point(0.88, 0), 0.45, 0.7);
            res["AmbientLightBottomBrush"] = CreateRadialGradient(Color.FromArgb(0x1A, 0x63, 0x66, 0xF1), Color.FromArgb(0x00, 0x63, 0x66, 0xF1), new Point(0, 1), 0.4, 0.55);

            res["WindowBackground"] = new SolidColorBrush(Color.FromRgb(0xF1, 0xF5, 0xF9));
            res["CardBackground"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            res["CardBorder"] = new SolidColorBrush(Color.FromArgb(0x1A, 0x00, 0x00, 0x00));
            res["FooterBackground"] = new SolidColorBrush(Color.FromRgb(0xEB, 0xEE, 0xF3));
            res["ControlBackground"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));
            res["ControlBorder"] = new SolidColorBrush(Color.FromRgb(0xCB, 0xD5, 0xE1));
            res["TextPrimary"] = new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x2A));
            res["TextSecondary"] = new SolidColorBrush(Color.FromRgb(0x47, 0x55, 0x69));
            res["TextMuted"] = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));

            res["TrackBackground"] = new SolidColorBrush(Color.FromRgb(0xE2, 0xE8, 0xF0));
            res["SurfaceSubtle"] = new SolidColorBrush(Color.FromArgb(0x0A, 0x00, 0x00, 0x00));
            res["SurfaceHover"] = new SolidColorBrush(Color.FromArgb(0x14, 0x00, 0x00, 0x00));
            res["TileHoverBorder"] = new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x70, 0xD1));

            res["BadgeActiveBg"] = new SolidColorBrush(Color.FromArgb(0x1A, 0x16, 0xA3, 0x4A));
            res["BadgeActiveFg"] = new SolidColorBrush(Color.FromRgb(0x15, 0x80, 0x3D));
            res["BadgeStandbyBg"] = new SolidColorBrush(Color.FromArgb(0x14, 0x00, 0x70, 0xD1));
            res["BadgeStandbyFg"] = new SolidColorBrush(Color.FromRgb(0x1D, 0x4E, 0xD8));

            res["FeaturePillBg"] = new SolidColorBrush(Color.FromArgb(0x0A, 0x00, 0x00, 0x00));
            res["FeaturePillBorder"] = new SolidColorBrush(Color.FromArgb(0x0C, 0x00, 0x00, 0x00));

            res["GamepadFocusBorder"] = new SolidColorBrush(Color.FromRgb(0x00, 0x70, 0xD1));
            res["GamepadFocusGlow"] = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x70, 0xD1));
            res["GamepadHudBg"] = new SolidColorBrush(Color.FromRgb(0xEA, 0xEC, 0xF0));
            res["GamepadBadgeBg"] = new SolidColorBrush(Color.FromRgb(0xDE, 0xE2, 0xE8));
            res["GamepadBadgeBorder"] = new SolidColorBrush(Color.FromRgb(0xCD, 0xD2, 0xDC));
            res["GamepadBadgeFg"] = new SolidColorBrush(Color.FromRgb(0x20, 0x22, 0x28));

            res["PlayStationBlue"] = new SolidColorBrush(Color.FromRgb(0x00, 0x70, 0xD1));
            res["PlayStationBluePressed"] = new SolidColorBrush(Color.FromRgb(0x00, 0x64, 0xB7));
            res["PlayStationBlueActive"] = new SolidColorBrush(Color.FromRgb(0x00, 0x4D, 0x8D));
            res["TextOnPrimary"] = new SolidColorBrush(Colors.White);
        }
    }
}
