// The PID calculation.
//
// Kept exactly as in the original sketch: the previous error is never updated (it stays 0) and
// KD is not used, so the result is KP*e + KI*e + e. See system design, known limitation 1,
// before changing it.

const float PREVIOUS_ERROR = 0;

// Returns the PID output limited to PID_MIN..PID_MAX.
float computePid(float setpoint, float measured) {
  float error = setpoint - measured;
  float proportional = error * KP;
  float sumError = error + PREVIOUS_ERROR;
  float integral = KI * sumError;
  float derivative = error - PREVIOUS_ERROR;
  float pid = proportional + integral + derivative;

  if (pid < PID_MIN) {
    pid = PID_MIN;
  }
  if (pid > PID_MAX) {
    pid = PID_MAX;
  }
  return pid;
}
