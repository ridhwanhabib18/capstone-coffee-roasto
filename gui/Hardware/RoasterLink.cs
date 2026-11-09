using System;
using System.IO;
using System.IO.Ports;

namespace CoffeeRoasto_GUI.Hardware
{
    /// <summary>The serial connection to the Arduino: open, close, send a command, read a line.</summary>
    internal sealed class RoasterLink
    {
        private const int BaudRate = 9600; // must match Serial.begin() in the firmware
        private readonly SerialPort _port;

        public RoasterLink(SerialPort port)
        {
            _port = port;
        }

        /// <summary>Raised when a write fails, with a message that can be shown to the operator.</summary>
        public event Action<string> Error;

        public bool IsOpen
        {
            get { return _port.IsOpen; }
        }

        public static string[] AvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

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
        public void SendLine(string text)
        {
            _port.WriteLine(text);
        }

        /// <summary>Reads one line (without the newline). Returns null when the port was closed meanwhile.</summary>
        public string TryReadLine()
        {
            try
            {
                return _port.ReadTo("\n");
            }
            catch (Exception ex) when (IsPortFailure(ex))
            {
                return null;
            }
        }

        private static bool IsPortFailure(Exception ex)
        {
            return ex is InvalidOperationException
                || ex is IOException
                || ex is TimeoutException
                || ex is UnauthorizedAccessException;
        }
    }
}
