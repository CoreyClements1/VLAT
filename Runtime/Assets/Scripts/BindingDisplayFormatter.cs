using System.Text.RegularExpressions;
using UnityEngine.InputSystem;

namespace VLAT
{
    /// <summary>
    /// Formats control labels/paths into short UI text for the accessibility menu
    /// (target ~10 characters).
    /// </summary>
    public static class BindingDisplayFormatter
    {
        public const int MaxLength = 10;

        public static string Format(string labelOrPath)
        {
            if (string.IsNullOrEmpty(labelOrPath))
                return string.Empty;

            string formatted = LooksLikePath(labelOrPath)
                ? FormatPath(labelOrPath)
                : FormatLabel(labelOrPath);

            return Truncate(formatted);
        }

        public static string FromAction(InputAction action)
        {
            if (action == null)
                return string.Empty;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                InputBinding binding = action.bindings[i];
                if (binding.isComposite || binding.isPartOfComposite)
                    continue;

                if (string.IsNullOrEmpty(binding.effectivePath))
                    continue;

                return Format(binding.effectivePath);
            }

            return string.Empty;
        }

        private static bool LooksLikePath(string value)
        {
            return value.IndexOf('<') >= 0
                || value.IndexOf('/') >= 0
                || value.IndexOf('{') >= 0;
        }

        private static string FormatLabel(string label)
        {
            string s = label.Replace('—', '-').Replace('–', '-').Trim();
            s = Regex.Replace(s, @"\s+", " ");

            s = ReplaceInsensitive(s, "Left Controller", "L");
            s = ReplaceInsensitive(s, "Right Controller", "R");
            s = ReplaceInsensitive(s, "Left Hand", "L");
            s = ReplaceInsensitive(s, "Right Hand", "R");
            s = ReplaceInsensitive(s, "XInput Controller", "Pad");
            s = ReplaceInsensitive(s, "Xbox Controller", "Pad");
            s = ReplaceInsensitive(s, "Gamepad", "Pad");
            s = ReplaceInsensitive(s, "Keyboard", "Key");
            s = ReplaceInsensitive(s, "Oculus Touch Controller", "");
            s = ReplaceInsensitive(s, "XR Controller", "");

            s = Regex.Replace(s, @"\s*-\s*", " - ");
            s = Regex.Replace(s, @"\s+", " ").Trim();
            s = Regex.Replace(s, @"^-\s*", "").Trim();

            return s;
        }

        private static string FormatPath(string path)
        {
            string lower = path.ToLowerInvariant();
            string control = ExtractControlName(path);

            if (lower.Contains("keyboard"))
            {
                string key = CapitalizeControl(control);
                string withPrefix = "Key - " + key;
                // Longer keys (Space, Enter) drop the prefix to stay short.
                return withPrefix.Length <= MaxLength ? withPrefix : key;
            }

            if (lower.Contains("mouse"))
                return FormatMouse(control);

            if (lower.Contains("xinput") || lower.Contains("gamepad"))
                return "Pad - " + FormatGamepadButton(control);

            bool left = lower.Contains("lefthand") || lower.Contains("{left");
            bool right = lower.Contains("righthand") || lower.Contains("{right");
            if (left)
                return "L - " + FormatXrButton(control, leftHand: true);
            if (right)
                return "R - " + FormatXrButton(control, leftHand: false);

            return CapitalizeControl(control);
        }

        private static string ExtractControlName(string path)
        {
            int slash = path.LastIndexOf('/');
            string control = slash >= 0 ? path.Substring(slash + 1) : path;
            control = control.Replace("{", "").Replace("}", "").Replace("<", "").Replace(">", "");

            if (control.EndsWith("pressed", System.StringComparison.OrdinalIgnoreCase))
                control = control.Substring(0, control.Length - "pressed".Length);

            return control;
        }

        private static string FormatXrButton(string control, bool leftHand)
        {
            string c = control.ToLowerInvariant();

            // Quest Touch face-button convention.
            if (c.Contains("secondary"))
                return leftHand ? "X" : "B";
            if (c.Contains("primary"))
                return leftHand ? "Y" : "A";
            if (c.Contains("trigger"))
                return "Trig";
            if (c.Contains("grip"))
                return "Grip";

            return CapitalizeControl(control);
        }

        private static string FormatGamepadButton(string control)
        {
            string c = control.ToLowerInvariant();
            if (c.Contains("south")) return "A";
            if (c.Contains("east")) return "B";
            if (c.Contains("west")) return "X";
            if (c.Contains("north")) return "Y";
            return CapitalizeControl(control);
        }

        private static string FormatMouse(string control)
        {
            string c = control.ToLowerInvariant();
            if (c.Contains("left")) return "M - L";
            if (c.Contains("right")) return "M - R";
            if (c.Contains("middle")) return "M - Mid";
            return "M - " + CapitalizeControl(control);
        }

        private static string CapitalizeControl(string control)
        {
            if (string.IsNullOrEmpty(control))
                return "?";

            if (control.Length == 1)
                return control.ToUpperInvariant();

            if (string.Equals(control, "space", System.StringComparison.OrdinalIgnoreCase))
                return "Space";
            if (string.Equals(control, "enter", System.StringComparison.OrdinalIgnoreCase))
                return "Enter";
            if (string.Equals(control, "escape", System.StringComparison.OrdinalIgnoreCase))
                return "Esc";
            if (string.Equals(control, "leftButton", System.StringComparison.OrdinalIgnoreCase))
                return "LClk";
            if (string.Equals(control, "rightButton", System.StringComparison.OrdinalIgnoreCase))
                return "RClk";

            return char.ToUpperInvariant(control[0]) + control.Substring(1);
        }

        private static string Truncate(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= MaxLength)
                return value;

            return value.Substring(0, MaxLength);
        }

        private static string ReplaceInsensitive(string input, string oldValue, string newValue)
        {
            return Regex.Replace(input, Regex.Escape(oldValue), newValue, RegexOptions.IgnoreCase);
        }
    }
}
