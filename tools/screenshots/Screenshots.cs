// Draws screenshots of the real Coffee Roasto window without a roaster.
//
// The form is created from CoffeeRoasto_GUI.exe and fed the lines the firmware simulation produced
// (tests/firmware-sim/golden/scenario12.txt: a whole roast following a recorded profile in a closed loop).
// Everything on screen is drawn by the real code; only the serial port is missing, so the connected state
// and the lamps of the running mode are set by hand, the way the button handlers would after a successful send.
//
// usage: Screenshots.exe <trace.txt> <profile.xlsx> <output-dir>
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CoffeeRoasto_GUI;

internal static class Screenshots
{
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static Form_CoffeeRoasto _form;
    private static Assembly _app;
    private static string _outDir;
    private static string _framesDir;
    private static int _frameNumber;

    private class TraceEvent
    {
        public long Millis;
        public string Kind; // "frame", "Y", "Z", "X"
        public string Text;
    }

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: Screenshots.exe <trace.txt> <profile.xlsx> <output-dir>");
            return 2;
        }

        // The app expects an Indonesian culture: the firmware prints a dot as decimal separator.
        var culture = new CultureInfo("id-ID");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;

        _outDir = Path.GetFullPath(args[2]);
        _framesDir = Path.Combine(_outDir, "_frames");
        Directory.CreateDirectory(_outDir);
        Directory.CreateDirectory(_framesDir);
        foreach (var old in Directory.GetFiles(_framesDir, "*.png")) File.Delete(old);

        List<TraceEvent> trace = ReadTrace(args[0]);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        _app = typeof(Form_CoffeeRoasto).Assembly;
        _form = new Form_CoffeeRoasto();
        _form.StartPosition = FormStartPosition.Manual;
        _form.Location = new Point(0, 0);
        _form.Show();
        Pump();

        Shot("01-start");

        Connect("COM3");
        Shot("02-connected");

        LoadProfile(args[1]);
        Shot("03-profile-loaded");

        Replay(trace);
        return 0;
    }

    // ---- Scenes ---------------------------------------------------------------------------------

    private static void Connect(string port)
    {
        var ports = (ComboBox)Control("comboBox_ComPort");
        ports.Items.Clear();
        ports.Items.Add(port);
        ports.SelectedIndex = 0;
        Control("button_Connect").Enabled = false;
        Control("button_Disconnect").Enabled = true;
        ((ProgressBar)Control("progressBar_PanelConnection")).Value = 100;
        Call(_form, "SetControlsEnabled", true);
        Call(_form, "SetConnectionBadge", true);
        Call(Get(_form, "_chart"), "ClearMeasured");
        Pump();
    }

    private static void LoadProfile(string path)
    {
        Type reader = _app.GetType("CoffeeRoasto_GUI.Profiles.ProfileReader");
        object sheets = reader.GetMethod("Read", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { path });
        _form.GetType().GetField("_profileSheets", AnyInstance).SetValue(_form, sheets);

        Control("textBox_FileName").Text = path;
        var combo = (ComboBox)Control("comboBox_Sheet");
        combo.Items.Clear();
        foreach (System.Data.DataTable table in (System.Data.DataTableCollection)sheets)
        {
            combo.Items.Add(table.TableName);
        }
        combo.SelectedIndex = 0; // shows the sheet in the data table
        Pump();
    }

    /// <summary>Replays a whole roast: PID mode, recording, the burner lamp following the Y and Z events.</summary>
    private static void Replay(List<TraceEvent> trace)
    {
        var clock = Get(_form, "_clock");
        var recording = Get(_form, "_recording");
        long start = trace.Find(e => e.Kind == "frame").Millis;

        // PID mode started by the operator: the lamps the button handler sets after a successful send.
        Lamps(on: new[] { "PidMode", "MotorDrum", "BlowerDrum", "MotorValve" }, off: new[] { "Cooler" });
        Call(Get(_form, "_chart"), "ClearAll");
        Call(recording, "Start");
        Lamps(on: new[] { "RecordData" }, off: new string[0]);
        ((DataGridView)Control("dataGridView")).DataSource = null;

        int frames = 0;
        long lastShot = -1;
        foreach (TraceEvent e in trace)
        {
            if (e.Kind == "X")
            {
                // The form shows a message box for X; set the lamps it would set instead.
                Lamps(on: new[] { "MotorDrum", "Cooler" }, off: new[] { "AutoMode", "BlowerDrum", "Burner" });
                continue;
            }

            Call(clock, "FreezeAt", TimeSpan.FromMilliseconds(e.Millis - start));
            Call(_form, "HandleLine", e.Text);
            if (e.Kind != "frame") continue;

            frames++;
            Pump();
            if (frames % 20 == 0)
            {
                SaveFrame();
            }

            // Screenshots at a few moments of the roast.
            long minute = (e.Millis - start) / 60000;
            if (minute != lastShot && (minute == 5 || minute == 15))
            {
                Shot("04-pid-running-" + minute.ToString("00") + "min");
                lastShot = minute;
            }
        }

        // End of the roast: recording stopped, the table shows the samples.
        Call(recording, "Stop");
        ((DataGridView)Control("dataGridView")).DataSource = Get(recording, "Samples");
        Lamps(on: new string[0], off: new[] { "RecordData", "PidMode" });
        Pump();
        SaveFrame();
        Shot("05-roast-finished");
    }

    // ---- Helpers --------------------------------------------------------------------------------

    private static List<TraceEvent> ReadTrace(string path)
    {
        var list = new List<TraceEvent>();
        foreach (string line in File.ReadAllLines(path))
        {
            // t=12345 frame @150.00A160.00B150.00C   |   t=12345 send Y
            if (!line.StartsWith("t=")) continue;
            int space = line.IndexOf(' ');
            long millis = long.Parse(line.Substring(2, space - 2), CultureInfo.InvariantCulture);
            string rest = line.Substring(space + 1);

            if (rest.StartsWith("frame "))
            {
                list.Add(new TraceEvent { Millis = millis, Kind = "frame", Text = rest.Substring(6) });
            }
            else if (rest == "send Y" || rest == "send Z" || rest == "send X")
            {
                string letter = rest.Substring(5);
                list.Add(new TraceEvent { Millis = millis, Kind = letter, Text = letter });
            }
        }
        return list;
    }

    private static void Lamps(string[] on, string[] off)
    {
        object board = Get(_form, "_lamps");
        Type lampType = _app.GetType("CoffeeRoasto_GUI.UI.Lamp");
        Call(board, "On", ToLampArray(lampType, on));
        Call(board, "Off", ToLampArray(lampType, off));
    }

    private static Array ToLampArray(Type lampType, string[] names)
    {
        Array array = Array.CreateInstance(lampType, names.Length);
        for (int i = 0; i < names.Length; i++) array.SetValue(Enum.Parse(lampType, names[i]), i);
        return array;
    }

    private static Control Control(string name)
    {
        FieldInfo field = _form.GetType().GetField(name, AnyInstance);
        return (Control)field.GetValue(_form);
    }

    private static object Get(object target, string field)
    {
        if (target is Form_CoffeeRoasto || target.GetType().GetField(field, AnyInstance) != null)
        {
            return target.GetType().GetField(field, AnyInstance).GetValue(target);
        }
        return target.GetType().GetProperty(field, AnyInstance).GetValue(target, null);
    }

    private static void Call(object target, string method, params object[] args)
    {
        // A lamp list is passed as one "params" array argument.
        MethodInfo m = target.GetType().GetMethod(method, AnyInstance);
        if (m == null) throw new InvalidOperationException("method not found: " + method);
        m.Invoke(target, args.Length == 1 && args[0] is Array && m.GetParameters().Length == 1 ? new object[] { args[0] } : args);
    }

    private static void Pump()
    {
        for (int i = 0; i < 3; i++)
        {
            Application.DoEvents();
            Thread.Sleep(30);
        }
    }

    // Labels that sit on top of the chart. DrawToBitmap paints the chart over them, so they are drawn again afterwards.
    private static readonly string[] OverlayLabels =
    {
        "label_BeanTemperature", "label_DrumTemperature", "label_ReferenceTemperature",
        "label_DataBeanTemperature", "label_DataDrumTemperature", "label_DataReferenceTemperature"
    };

    private static Bitmap Render()
    {
        Pump();
        var bitmap = new Bitmap(_form.Width, _form.Height);
        _form.DrawToBitmap(bitmap, new Rectangle(0, 0, _form.Width, _form.Height));

        // Where the client area starts inside the window (border and title bar).
        Point client = _form.PointToScreen(Point.Empty);
        var offset = new Point(client.X - _form.Left, client.Y - _form.Top);

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            foreach (string name in OverlayLabels)
            {
                Control label = Control(name);
                using (var part = new Bitmap(label.Width, label.Height))
                {
                    label.DrawToBitmap(part, new Rectangle(Point.Empty, label.Size));
                    g.DrawImage(part, label.Left + offset.X, label.Top + offset.Y);
                }
            }
        }
        return bitmap;
    }

    private static void Shot(string name)
    {
        using (Bitmap bitmap = Render())
        {
            bitmap.Save(Path.Combine(_outDir, name + ".png"), ImageFormat.Png);
        }
        Console.WriteLine("saved " + name + ".png");
    }

    private static void SaveFrame()
    {
        using (Bitmap bitmap = Render())
        {
            bitmap.Save(Path.Combine(_framesDir, "frame" + (_frameNumber++).ToString("000") + ".png"), ImageFormat.Png);
        }
    }
}
