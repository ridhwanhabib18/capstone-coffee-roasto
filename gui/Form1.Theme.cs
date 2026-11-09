using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CoffeeRoasto_GUI.UI;

namespace CoffeeRoasto_GUI
{
    // The look of the screen: colours, fonts, a header with the connection status, and cards.
    // Only appearance is changed here; names, handlers and the layout of the controls stay as the designer made them.
    public partial class Form_CoffeeRoasto
    {
        private const int HeaderHeight = 56;
        private Label _connectionBadge;

        private void ApplyTheme()
        {
            SuspendLayout();

            BackColor = Palette.Background;
            ForeColor = Palette.Text;
            StyleFonts(this);
            StyleLabels();
            StyleButtons();
            StyleInputs();
            StyleGroups();
            StyleChart();
            StyleDataTable();
            MakeHeader();        // shifts everything down, so it comes after the styling above
            MakeManualCard();

            ResumeLayout(true);
            SetConnectionBadge(false);
        }

        /// <summary>Shows whether the roaster is connected (the badge in the header).</summary>
        private void SetConnectionBadge(bool connected)
        {
            _connectionBadge.Text = connected ? "●  Connected" : "●  Not connected";
            _connectionBadge.ForeColor = connected ? Color.FromArgb(88, 214, 141) : Palette.HeaderMuted;
        }

        // ---- Pieces ---------------------------------------------------------------------------

        private static IEnumerable<Control> Descendants(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control grandChild in Descendants(child))
                {
                    yield return grandChild;
                }
            }
        }

        private static void StyleFonts(Control root)
        {
            root.Font = Palette.Restyle(root.Font);
            foreach (Control control in Descendants(root))
            {
                if (control is Chart || control is PilotLamp)
                {
                    continue;
                }
                control.Font = Palette.Restyle(control.Font);
            }
        }

        private void StyleLabels()
        {
            foreach (Control control in Descendants(this))
            {
                var label = control as Label;
                if (label != null)
                {
                    label.ForeColor = Palette.Text;
                }
            }

            // The readings keep the colour of their line in the chart.
            Color bean = Color.FromArgb(192, 57, 43);
            Color drum = Palette.Blue;
            Color reference = Palette.Green;
            Colour(label_BeanTemperature, bean);
            Colour(label_DataBeanTemperature, bean);
            Colour(label_DrumTemperature, drum);
            Colour(label_DataDrumTemperature, drum);
            Colour(label_ReferenceTemperature, reference);
            Colour(label_DataReferenceTemperature, reference);

            label_DataBeanTemperature.Font = new Font(Palette.FontNameBold, 20F);
            label_DataDrumTemperature.Font = new Font(Palette.FontNameBold, 20F);
            label_DataReferenceTemperature.Font = new Font(Palette.FontNameBold, 20F);
            label_BeanTemperature.Font = new Font(Palette.FontNameBold, 10F);
            label_DrumTemperature.Font = new Font(Palette.FontNameBold, 10F);
            label_ReferenceTemperature.Font = new Font(Palette.FontNameBold, 10F);

            label3.Font = new Font(Palette.FontNameBold, 12F);
        }

        private static void Colour(Label label, Color color)
        {
            label.ForeColor = color;
            label.BackColor = Palette.Card;
        }

        private void StyleButtons()
        {
            foreach (Control control in Descendants(this))
            {
                var button = control as Button;
                if (button == null || button is PilotLamp)
                {
                    continue;
                }

                string name = button.Name;
                if (name == "button_Connect") Palette.StyleButton(button, Palette.Accent, Palette.AccentHover);
                else if (name == "button_Disconnect") Palette.StyleButton(button, Palette.Neutral, Palette.NeutralHover);
                else if (name == "button_SaveRecordData") Palette.StyleButton(button, Palette.Blue, Palette.BlueHover);
                else if (name == "button_bean" || name.StartsWith("button_ON") || name.StartsWith("button_Start"))
                    Palette.StyleButton(button, Palette.Green, Palette.GreenHover);
                else if (name.StartsWith("button_OFF") || name.StartsWith("button_Stop"))
                    Palette.StyleButton(button, Palette.Red, Palette.RedHover);
                else Palette.StyleButton(button, Palette.Accent, Palette.AccentHover);
            }
        }

        private void StyleInputs()
        {
            textBox_InputTemp.BackColor = Color.White;
            textBox_FileName.BackColor = Color.White;
            comboBox_ComPort.FlatStyle = FlatStyle.Flat;
            comboBox_Sheet.FlatStyle = FlatStyle.Flat;
            comboBox_ComPort.BackColor = Color.White;
            comboBox_Sheet.BackColor = Color.White;
        }

        private void StyleGroups()
        {
            foreach (GroupBox group in new[] { groupBox_PanelConnection, groupBox_AutomaticControl })
            {
                group.BackColor = Palette.Card;
                group.ForeColor = Palette.Accent;
                group.Font = new Font(Palette.FontNameBold, 9F);
                foreach (Control child in group.Controls)
                {
                    if (child is Label)
                    {
                        child.ForeColor = Palette.Text;
                    }
                }
            }
        }

        private void StyleChart()
        {
            chart_Temperature.BackColor = Palette.Card;
            chart_Temperature.BorderlineColor = Palette.Border;
            chart_Temperature.BorderlineDashStyle = ChartDashStyle.Solid;
            chart_Temperature.BorderlineWidth = 1;

            ChartArea area = chart_Temperature.ChartAreas[0];
            area.BackColor = Palette.Card;
            foreach (Axis axis in new[] { area.AxisX, area.AxisY })
            {
                axis.LineColor = Palette.Border;
                axis.MajorGrid.LineColor = Palette.Grid;
                axis.MajorTickMark.LineColor = Palette.Border;
                axis.LabelStyle.Font = new Font(Palette.FontName, 9F);
                axis.LabelStyle.ForeColor = Palette.Muted;
                axis.TitleFont = new Font(Palette.FontName, 9F);
                axis.TitleForeColor = Palette.Muted;
            }

            Title title = chart_Temperature.Titles[0];
            title.Font = new Font(Palette.FontNameBold, 13F);
            title.ForeColor = Palette.Text;

            // The readings beside the chart carry the colour and the name of each line, so the legend is not needed.
            foreach (Legend legend in chart_Temperature.Legends)
            {
                legend.Enabled = false;
            }
            area.Position = new ElementPosition(0, 9, 76, 90);

            Style(chart_Temperature.Series["Bean Temperature"], Color.FromArgb(192, 57, 43), ChartDashStyle.Solid);
            Style(chart_Temperature.Series["Drum Temperature"], Palette.Blue, ChartDashStyle.Solid);
            Style(chart_Temperature.Series["Reference Temperature"], Palette.Green, ChartDashStyle.Dash);
        }

        private static void Style(Series series, Color color, ChartDashStyle dash)
        {
            series.Color = color;
            series.BorderWidth = 2;
            series.BorderDashStyle = dash;
        }

        private void StyleDataTable()
        {
            panel1.BackColor = Palette.Card;
            dataGridView.BackgroundColor = Palette.Card;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.GridColor = Palette.Grid;
            dataGridView.RowHeadersVisible = false;
            dataGridView.EnableHeadersVisualStyles = false;

            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Palette.Header;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(Palette.FontNameBold, 9F);
            dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Palette.Header;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dataGridView.DefaultCellStyle.BackColor = Palette.Card;
            dataGridView.DefaultCellStyle.ForeColor = Palette.Text;
            dataGridView.DefaultCellStyle.SelectionBackColor = Palette.Accent;
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView.DefaultCellStyle.Font = new Font(Palette.FontName, 9F);
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 246, 240);
        }

        /// <summary>A dark band at the top with the name of the app and the connection status.</summary>
        private void MakeHeader()
        {
            foreach (Control control in Controls)
            {
                control.Top += HeaderHeight;
            }
            ClientSize = new Size(ClientSize.Width, ClientSize.Height + HeaderHeight);

            var header = new Panel
            {
                Bounds = new Rectangle(0, 0, ClientSize.Width, HeaderHeight),
                BackColor = Palette.Header,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            header.Controls.Add(new Label
            {
                Text = "Coffee Roasto",
                Font = new Font(Palette.FontNameBold, 15F),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 7)
            });
            header.Controls.Add(new Label
            {
                Text = "Drum roaster monitoring and control",
                Font = new Font(Palette.FontName, 8.5F),
                ForeColor = Palette.HeaderMuted,
                AutoSize = true,
                Location = new Point(22, 33)
            });

            _connectionBadge = new Label
            {
                Font = new Font(Palette.FontNameBold, 9.5F),
                AutoSize = false,
                Size = new Size(170, 24),
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(header.Width - 170 - 20, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            header.Controls.Add(_connectionBadge);

            Controls.Add(header);
            header.SendToBack();
        }

        /// <summary>A white card behind the manual controls, so they read as one group like the two group boxes.</summary>
        private void MakeManualCard()
        {
            Control[] members =
            {
                label_Burner, pilotLamp_Burner, button_ONBurner, button_OFFBurner,
                label_MotorDrum, pilotLamp_MotorDrum, button_ONMotorDrum, button_OFFMotorDrum,
                label_BlowerDrum, pilotLamp_BlowerDrum, button_ONBlowerDrum, button_OFFBlowerDrum,
                label_Cooler, pilotLamp_Cooler, button_ONCooler, button_OFFCooler,
                label1, pilotLampMotorValve, button_ONMotorValve, button_OFFMotorValve,
                label_RecordData, pilotLamp_RecordData,
                button_StartRecordData, button_StopRecordData, button_SaveRecordData
            };

            Rectangle union = members[0].Bounds;
            foreach (Control member in members)
            {
                union = Rectangle.Union(union, member.Bounds);
            }

            // Stay between the connection box on the left and the automatic box on the right.
            int left = Math.Max(union.Left - 8, groupBox_PanelConnection.Right + 4);
            int right = Math.Min(union.Right + 8, groupBox_AutomaticControl.Left - 4);
            int top = union.Top - 10;
            int bottom = union.Bottom + 10;

            var card = new Panel
            {
                Bounds = new Rectangle(left, top, right - left, bottom - top),
                BackColor = Palette.Card
            };
            card.Paint += (sender, e) =>
            {
                using (var pen = new Pen(Palette.Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };
            Controls.Add(card);

            foreach (Control member in members)
            {
                member.Location = new Point(member.Left - card.Left, member.Top - card.Top);
                card.Controls.Add(member);
            }
            card.SendToBack();
        }
    }
}
