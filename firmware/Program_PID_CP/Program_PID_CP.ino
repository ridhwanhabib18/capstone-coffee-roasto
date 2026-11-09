// Coffee Roasto firmware.
//
// Tabs (files of this sketch):
//   Config.h, Commands.h, Globals.h  fixed values, protocol characters, shared objects
//   Actuators.ino                    relays and the gas valve
//   TemperatureSensors.ino           sampling, calibration and the data frame sent to the GUI
//   Pid.ino                          the PID calculation
//   ReferenceProfile.h               the set points received from the GUI
//   ManualMode.ino                   single commands and a typed set point
//   HeatingMode.ino, PidMode.ino     the two loops that run until they finish or are stopped
//
// Behaviour is the same as the original single-file sketch; only the structure changed.

#include <max6675.h>
#include <Servo.h>
#include "Config.h"
#include "Commands.h"
#include "Globals.h"
#include "ReferenceProfile.h"

void setup() {
  initActuators();
  Serial.begin(SERIAL_BAUD);
  delay(500);
}

void loop() {
  String command = readCommand();
  bool newReading = readTemperature();

  handleManualCommand(command);
  if (newReading) {
    holdManualSetpoint();
  }

  // "q" and "h" also pass through handleManualCommand above, which resets the set point to 0.
  if (command == CMD_PID) {
    runPidMode();
  }
  if (command == CMD_HEATING) {
    runHeatingMode();
  }
}

// Reads one command line from the GUI, or returns an empty string when nothing was sent.
String readCommand() {
  if (!Serial.available()) {
    return "";
  }
  delay(2);
  return Serial.readStringUntil('\n');
}
