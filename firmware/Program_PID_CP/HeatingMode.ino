// Heating mode ("h"): pre-heats the drum to WARMUP_SETPOINT for HEATING_DURATION_S seconds
// with the valve fully open. Ends after that time or when the GUI sends "s".

void runHeatingMode() {
  bool starting = true;
  unsigned long startMillis = 0;

  while (true) {
    setDrumMotor(true);
    setBlower(true);
    setCooler(false);

    float pid = computePid(roaster.setpoint, beanTemperature);
    setValveAngle(VALVE_OPEN_ANGLE);

    if (starting) {
      startMillis = millis();
      starting = false;
      roaster.setpoint = WARMUP_SETPOINT;
    }
    // Taken after startMillis so the elapsed time can never be negative.
    unsigned long now = millis();

    readTemperature();

    if (readCommand() == CMD_STOP) {
      setBurner(false);
      setDrumMotor(true);
      setBlower(false);
      setCooler(false);
      roaster.setpoint = 0;
      setValveAngle(VALVE_OPEN_ANGLE);
      break;
    }

    unsigned long elapsedMs = now - startMillis;
    bool withinHeatingTime = elapsedMs <= 1000UL * HEATING_DURATION_S;

    if (!roaster.burnerOn && pid >= PID_MAX && withinHeatingTime) {
      setBurner(true);
      setValveAngle(VALVE_OPEN_ANGLE);
      Serial.println(EVENT_BURNER_ON);
    } else if (roaster.burnerOn && pid <= PID_MIN) {
      setBurner(false);
      setValveAngle(VALVE_OPEN_ANGLE);
      Serial.println(EVENT_BURNER_OFF);
    }

    if (elapsedMs >= 1000UL * HEATING_DURATION_S) {
      Serial.println(EVENT_HEATING_DONE);
      setBurner(false);
      delay(100);
      setDrumMotor(true);
      setBlower(false);
      setCooler(false);
      setValveAngle(VALVE_OPEN_ANGLE);
      Serial.println(EVENT_BURNER_OFF);
      break;
    }
  }
}
