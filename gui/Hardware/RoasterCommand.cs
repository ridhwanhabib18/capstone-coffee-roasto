namespace CoffeeRoasto_GUI.Hardware
{
    /// <summary>
    /// Single-character commands understood by the firmware.
    /// The full contract is in docs/en/serial-protocol.md; change both sides together.
    /// </summary>
    internal static class RoasterCommand
    {
        /// <summary>Sent once after connecting; the firmware reads it as set point 0.</summary>
        public const string ResetOnConnect = "#";

        public const string BurnerOn = "n";
        public const string BurnerOff = "k";
        public const string DrumMotorOn = "t";
        public const string DrumMotorOff = "d";
        public const string BlowerOn = "y";
        public const string BlowerOff = "b";
        public const string CoolerOn = "p";
        public const string CoolerOff = "c";
        public const string ValveOpen = "v";
        public const string ValveClose = "x";

        public const string HeatingStart = "h";
        public const string AutoStart = "m";
        public const string PidStart = "q";

        /// <summary>Stops the running heating, PID or auto loop.</summary>
        public const string Stop = "s";
    }
}
