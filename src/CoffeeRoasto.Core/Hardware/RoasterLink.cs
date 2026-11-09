using System.IO.Ports;

namespace CoffeeRoasto.Hardware;

/// <summary>The serial connection to the Arduino: open, close, send a command, receive lines.</summary>
public sealed class RoasterLink : IDisposable
{
    private const int BaudRate = 9600; // must match Serial.begin() in the firmware
    private readonly SerialPort _port = new();

    public RoasterLink()
    {
        _port.DataReceived += OnDataReceived;
    }

    /// <summary>Raised on a serial-port thread for every line received (without the newline).</summary>
    public event Action<string>? LineReceived;

    /// <summary>Raised when a write fails, with a message that can be shown to the operator.</summary>
    public event Action<string>? Error;

    public bool IsOpen => _port.IsOpen;

    public static string[] AvailablePorts() => SerialPort.GetPortNames();

    /// <summary>Opens the port and sends the reset character. Throws when the port cannot be opened.</summary>
    public void Open(string portName)
    {
        _port.PortName = portName;
        _port.BaudRate = BaudRate;
        _port.Open();
        _port.Write(RoasterCommand.ResetOnConnect);
    }

    public void Close()
    {
        if (_port.IsOpen)
        {
            _port.Close();
        }
    }

    /// <summary>Sends a command or a set point. Returns false when the port is closed or the write fails.</summary>
    public bool Send(string text)
    {
        if (!_port.IsOpen)
        {
            return false;
        }

        try
        {
            _port.Write(text);
            return true;
        }
        catch (Exception ex) when (IsPortFailure(ex))
        {
            Error?.Invoke(ex.Message);
            return false;
        }
    }

    /// <summary>Sends a line ending with a newline. Used for profile data; throws on failure.</summary>
    public void SendLine(string text) => _port.WriteLine(text);

    public void Dispose()
    {
        _port.DataReceived -= OnDataReceived;
        _port.Dispose();
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            // The firmware ends every line with "\n"; one event may carry more than one line.
            while (_port.IsOpen && _port.BytesToRead > 0)
            {
                string line = _port.ReadTo("\n");
                LineReceived?.Invoke(line.TrimEnd('\r'));
            }
        }
        catch (Exception ex) when (IsPortFailure(ex))
        {
            // The port was closed while reading.
        }
    }

    private static bool IsPortFailure(Exception ex) =>
        ex is InvalidOperationException or IOException or TimeoutException or UnauthorizedAccessException;
}
