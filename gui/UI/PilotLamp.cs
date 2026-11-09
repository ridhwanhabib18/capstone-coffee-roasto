using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CoffeeRoasto_GUI.UI
{
    /// <summary>A round indicator lamp: its BackColor is the lamp colour (dark green = off, lime = on).</summary>
    internal class PilotLamp : Button
    {
        public PilotLamp()
        {
            BackColor = Color.DarkGreen;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            using (var outline = new Pen(BackColor, 1))
            using (var background = new SolidBrush(Color.FromKnownColor(KnownColor.Control)))
            using (var fill = new SolidBrush(BackColor))
            using (var circle = new GraphicsPath())
            {
                Graphics g = pevent.Graphics;
                g.FillRectangle(background, 0, 0, ClientSize.Width, ClientSize.Height);
                g.DrawEllipse(outline, 0, 0, ClientSize.Width, ClientSize.Height);
                g.FillEllipse(fill, 0, 0, ClientSize.Width, ClientSize.Height);

                circle.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
                Region = new Region(circle);
            }
        }
    }
}
