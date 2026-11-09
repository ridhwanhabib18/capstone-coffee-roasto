// Reads both thermocouples, averages and corrects them, and sends one data frame to the GUI.

// Call this often. It takes one sample per SAMPLE_INTERVAL_MS and, after SAMPLES_PER_READING
// samples, updates beanTemperature and drumTemperature, sends the frame and returns true.
bool readTemperature() {
  static unsigned long lastSampleMillis = 0;
  static int sampleCount = 0;
  static float drumSum = 0;
  static float beanSum = 0;
  static float previousBean = 0;
  static float previousDrum = 0;

  if ((millis() - lastSampleMillis) >= SAMPLE_INTERVAL_MS) {
    drumSum += drumSensor.readCelsius();
    beanSum += beanSensor.readCelsius();
    sampleCount++;
    lastSampleMillis = millis();
  }

  if (sampleCount != SAMPLES_PER_READING) {
    return false;
  }

  beanTemperature = (beanSum / SAMPLES_PER_READING - BEAN_OFFSET) / BEAN_SLOPE;
  drumTemperature = (drumSum / SAMPLES_PER_READING - DRUM_OFFSET) / DRUM_SLOPE;

  // The MAX6675 only resolves 0.25 degrees; ignore smaller changes to stop the value flickering.
  if (abs(beanTemperature - previousBean) < DEAD_BAND) {
    beanTemperature = previousBean;
  }
  if (abs(drumTemperature - previousDrum) < DEAD_BAND) {
    drumTemperature = previousDrum;
  }

  sendFrame(beanTemperature, drumTemperature, roaster.setpoint);

  previousBean = beanTemperature;
  previousDrum = drumTemperature;
  drumSum = 0;
  beanSum = 0;
  sampleCount = 0;
  return true;
}

// "@<bean>A<drum>B<reference>C\n", for example "@37.11A38.67B150.00C".
void sendFrame(float bean, float drum, float reference) {
  Serial.print("@");
  Serial.print(bean);
  Serial.print("A");
  Serial.print(drum);
  Serial.print("B");
  Serial.print(reference);
  Serial.print("C");
  Serial.print("\n");
}
