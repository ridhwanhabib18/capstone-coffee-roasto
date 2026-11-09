// Relays and the gas valve. All outputs start off; the valve starts fully open.

void initActuators() {
  valveServo.attach(PIN_VALVE_SERVO);
  pinMode(PIN_BURNER, OUTPUT);
  pinMode(PIN_DRUM_MOTOR, OUTPUT);
  pinMode(PIN_BLOWER, OUTPUT);
  pinMode(PIN_COOLER, OUTPUT);

  digitalWrite(PIN_BURNER, RELAY_OFF);
  digitalWrite(PIN_DRUM_MOTOR, RELAY_OFF);
  digitalWrite(PIN_BLOWER, RELAY_OFF);
  digitalWrite(PIN_COOLER, RELAY_OFF);
  valveServo.write(VALVE_OPEN_ANGLE);
  roaster.burnerOn = false;
}

void setBurner(bool on) {
  digitalWrite(PIN_BURNER, on ? RELAY_ON : RELAY_OFF);
  roaster.burnerOn = on;
}

void setDrumMotor(bool on) {
  digitalWrite(PIN_DRUM_MOTOR, on ? RELAY_ON : RELAY_OFF);
}

void setBlower(bool on) {
  digitalWrite(PIN_BLOWER, on ? RELAY_ON : RELAY_OFF);
}

void setCooler(bool on) {
  digitalWrite(PIN_COOLER, on ? RELAY_ON : RELAY_OFF);
}

void setValveAngle(int angle) {
  valveServo.write(angle);
}

// The PID output is the valve angle measured from "fully open": the larger the error,
// the more the valve closes.
void setValveFromPid(float pid) {
  valveServo.write((int)(VALVE_CLOSED_ANGLE - pid));
}

// Switches the burner by the PID output: on at the maximum, off at the minimum,
// and tells the GUI. Does nothing in between.
void switchBurnerByPid(float pid) {
  if (!roaster.burnerOn && pid == PID_MAX) {
    setBurner(true);
    setValveFromPid(pid);
    Serial.println(EVENT_BURNER_ON);
  } else if (roaster.burnerOn && pid <= PID_MIN) {
    setBurner(false);
    setValveFromPid(pid);
    Serial.println(EVENT_BURNER_OFF);
  }
}
