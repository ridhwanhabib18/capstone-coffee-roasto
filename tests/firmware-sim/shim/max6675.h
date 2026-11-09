#pragma once
#include "Arduino.h"
float fakeRawCelsius(int chipSelect);
class MAX6675 {
 public:
  MAX6675(int8_t clk, int8_t cs, int8_t) : cs_(cs) { (void)clk; }
  float readCelsius() { return fakeRawCelsius(cs_); }
 private:
  int cs_;
};
