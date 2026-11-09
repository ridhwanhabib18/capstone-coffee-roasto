using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeRoasto_GUI.UI
{
    /// <summary>The colours and fonts of the screen: warm paper, coffee brown and three signal colours.</summary>
    internal static class Palette
    {
        public static readonly Color Background = Color.FromArgb(245, 240, 232);
        public static readonly Color Card = Color.White;
        public static readonly Color Header = Color.FromArgb(59, 42, 34);
        public static readonly Color HeaderMuted = Color.FromArgb(201, 182, 160);
        public static readonly Color Accent = Color.FromArgb(139, 90, 43);
        public static readonly Color AccentHover = Color.FromArgb(160, 108, 58);
        public static readonly Color Text = Color.FromArgb(59, 42, 34);
        public static readonly Color Muted = Color.FromArgb(122, 106, 94);
        public static readonly Color Border = Color.FromArgb(224, 214, 200);
        public static readonly Color Grid = Color.FromArgb(236, 228, 216);

        public static readonly Color Green = Color.FromArgb(46, 139, 87);
        public static readonly Color GreenHover = Color.FromArgb(60, 160, 104);
        public static readonly Color Red = Color.FromArgb(181, 72, 58);
        public static readonly Color RedHover = Color.FromArgb(201, 91, 76);
        public static readonly Color Blue = Color.FromArgb(47, 109, 181);
        public static readonly Color BlueHover = Color.FromArgb(66, 129, 200);
        public static readonly Color Neutral = Color.FromArgb(107, 91, 79);
        public static readonly Color NeutralHover = Color.FromArgb(128, 111, 98);

        public static readonly Color Disabled = Color.FromArgb(232, 225, 214);
        public static readonly Color DisabledText = Color.FromArgb(160, 148, 136);

        public static readonly Color LampOn = Color.FromArgb(46, 204, 113);
        public static readonly Color LampOff = Color.FromArgb(63, 98, 76);

        public const string FontName = "Segoe UI";
        public const string FontNameBold = "Segoe UI Semibold";

        /// <summary>The same font in Segoe UI, keeping the size and style the designer gave it.</summary>
        public static Font Restyle(Font font)
        {
            return new Font(FontName, font.Size, font.Style, font.Unit);
        }

        /// <summary>Flat coloured button that turns grey while it is disabled.</summary>
        public static void StyleButton(Button button, Color back, Color hover)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            button.Font = new Font(FontNameBold, 9F);

            Action apply = () =>
            {
                bool enabled = button.Enabled;
                button.BackColor = enabled ? back : Disabled;
                button.ForeColor = enabled ? Color.White : DisabledText;
                button.FlatAppearance.MouseOverBackColor = hover;
                button.FlatAppearance.MouseDownBackColor = back;
                button.Cursor = enabled ? Cursors.Hand : Cursors.Default;
            };
            button.EnabledChanged += (sender, e) => apply();
            apply();
        }
    }
}
