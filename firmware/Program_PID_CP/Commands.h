// Characters exchanged with the GUI. See docs/en/serial-protocol.md.
#pragma once

// ---- GUI -> firmware --------------------------------------------------------------------------
const char* const CMD_BURNER_ON = "n";
const char* const CMD_BURNER_OFF = "k";
const char* const CMD_DRUM_ON = "t";
const char* const CMD_DRUM_OFF = "d";
const char* const CMD_BLOWER_ON = "y";
const char* const CMD_BLOWER_OFF = "b";
const char* const CMD_COOLER_ON = "p";
const char* const CMD_COOLER_OFF = "c";
const char* const CMD_VALVE_OPEN = "v";
const char* const CMD_VALVE_CLOSE = "x";
const char* const CMD_HEATING = "h";
const char* const CMD_PID = "q";
const char* const CMD_STOP = "s";

// ---- firmware -> GUI --------------------------------------------------------------------------
const char* const EVENT_ROAST_DONE = "X";
const char* const EVENT_HEATING_DONE = "V";
const char* const EVENT_BURNER_ON = "Y";
const char* const EVENT_BURNER_OFF = "Z";
