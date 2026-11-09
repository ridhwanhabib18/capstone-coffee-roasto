// Manual control: one command switches one output, any other text is a typed set point.

void handleManualCommand(const String& command) {
  if (command == CMD_BURNER_OFF) {
    setBurner(false);
    roaster.setpoint = 0;
  } else if (command == CMD_BURNER_ON) {
    setBurner(true);
    roaster.setpoint = 0;
  } else if (command == CMD_DRUM_OFF) {
    setDrumMotor(false);
  } else if (command == CMD_DRUM_ON) {
    setDrumMotor(true);
  } else if (command == CMD_BLOWER_OFF) {
    setBlower(false);
  } else if (command == CMD_BLOWER_ON) {
    setBlower(true);
  } else if (command == CMD_COOLER_OFF) {
    setCooler(false);
  } else if (command == CMD_COOLER_ON) {
    setCooler(true);
  } else if (command == CMD_VALVE_CLOSE) {
    setValveAngle(VALVE_CLOSED_ANGLE);
  } else if (command == CMD_VALVE_OPEN) {
    setValveAngle(VALVE_OPEN_ANGLE);
  } else if (command != "") {
    // Not a known command: a temperature typed in the GUI (toFloat gives 0 for text that is not a number).
    roaster.setpoint = command.toFloat();
  }
}

// With a set point present, switches the burner by PID after every new reading.
void holdManualSetpoint() {
  if (roaster.setpoint == 0) {
    return;
  }
  float pid = computePid(roaster.setpoint, beanTemperature);
  switchBurnerByPid(pid);
}
