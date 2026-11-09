using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CoffeeRoasto_GUI.UI
{
    /// <summary>A round indicator lamp. Its BackColor is the lamp colour: Palette.LampOn or Palette.LampOff.</summary>
    internal class PilotLamp : Button
    {
        public PilotLamp()
        {
            BackColor = Palette.LampOff;
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.Clear(Parent != null ? Parent.BackColor : SystemColors.Control);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(1, 1, ClientSize.Width - 3, ClientSize.Height - 3);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(bounds);

                // A lit lamp has a soft glow; the shading goes from a light centre to the lamp colour.
                using (var shade = new PathGradientBrush(path))
                {
                    shade.CenterPoint = new PointF(bounds.Left + bounds.Width * 0.38f, bounds.Top + bounds.Height * 0.32f);
                    shade.CenterColor = Mix(BackColor, Color.White, 0.55f);
                    shade.SurroundColors = new[] { BackColor };
                    g.FillPath(shade, path);
                }

                using (var rim = new Pen(Mix(BackColor, Color.Black, 0.35f), 1.5f))
                {
                    g.DrawPath(rim, path);
                }
            }
        }

        private static Color Mix(Color from, Color to, float amount)
        {
            return Color.FromArgb(
                (int)(from.R + (to.R - from.R) * amount),
                (int)(from.G + (to.G - from.G) * amount),
                (int)(from.B + (to.B - from.B) * amount));
        }
    }
}
