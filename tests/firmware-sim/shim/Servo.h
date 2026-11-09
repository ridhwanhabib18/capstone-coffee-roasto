#pragma once
#include "Arduino.h"
void traceServo(int angle);
class Servo {
 public:
  void attach(int) {}
  void write(int angle) { traceServo(angle); }
};
