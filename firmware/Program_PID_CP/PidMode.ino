// PID mode ("q"): follows the reference profile sent by the GUI.
//
// Starts with a set point of WARMUP_SETPOINT until the first profile line arrives. Ends when the
// profile reaches a set point of 0 (roast done), when the GUI sends "s", or when the bean
// temperature reaches OVER_TEMPERATURE.

// Stop requested by the operator or the over-temperature limit.
void stopRoast() {
  roaster.setpoint = 0;
  Serial.println(EVENT_ROAST_DONE);
  setBurner(false);
  Serial.println(EVENT_BURNER_OFF);
  setBlower(false);
  setDrumMotor(true);
  setCooler(false);
  setValveAngle(VALVE_OPEN_ANGLE);
}

// The profile ended: the burner and blower stop, the drum keeps turning and the cooler starts.
void completeRoast() {
  Serial.println(EVENT_ROAST_DONE);
  setBurner(false);
  setValveAngle(VALVE_OPEN_ANGLE);
  delay(100);
  setBlower(false);
  Serial.println(EVENT_BURNER_OFF);
  setDrumMotor(true);
  setCooler(true);
}

void runPidMode() {
  ReferenceProfile profile;
  bool waitingForProfile = true;

  while (true) {
    bool newReading = readTemperature();

    setDrumMotor(true);
    setBlower(true);
    setCooler(false);

    float pid = computePid(roaster.setpoint, beanTemperature);
    setValveFromPid(pid);

    String command = readCommand();
    if (command == CMD_STOP || beanTemperature >= OVER_TEMPERATURE) {
      stopRoast();
      break;
    } else if (command != "") {
      // A line of set points from the GUI.
      profile.load(command);
      waitingForProfile = false;
    }

    if (waitingForProfile) {
      roaster.setpoint = WARMUP_SETPOINT;
    }
    if (newReading && !waitingForProfile) {
      roaster.setpoint = profile.next();
    }

    switchBurnerByPid(pid);

    if (roaster.setpoint == 0) {
      completeRoast();
      break;
    }
  }
}
