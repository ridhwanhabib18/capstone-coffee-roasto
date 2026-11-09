using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CoffeeRoasto_GUI
{
    class PilotLamp : Button
    {
        public PilotLamp()
        {
            BackColor = Color.DarkGreen;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Pen myPen = new Pen(this.BackColor, 1);
            Brush myBrush = new SolidBrush(Color.FromKnownColor(KnownColor.Control));
            Brush brushInside = new SolidBrush(this.BackColor);

            Graphics g = pevent.Graphics;
            g.FillRectangle(myBrush, 0, 0, ClientSize.Width, ClientSize.Height);
            g.DrawEllipse(myPen, 0, 0, ClientSize.Width, ClientSize.Height);
            g.FillEllipse(brushInside, 0, 0, ClientSize.Width, ClientSize.Height);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new Region(path);
        }
    }
}
