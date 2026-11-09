// The few objects and values shared by all tabs. Defined here, once, so every tab can use them.
#pragma once
#include <Servo.h>
#include <max6675.h>
#include "Config.h"

struct RoasterState {
  float setpoint = 0;     // reference temperature in degrees C; 0 means "none"
  bool burnerOn = false;
};

RoasterState roaster;

Servo valveServo;

// MAX6675(clock, chip select, data out)
MAX6675 drumSensor(DRUM_SENSOR_CLK, DRUM_SENSOR_CS, DRUM_SENSOR_DO);
MAX6675 beanSensor(BEAN_SENSOR_CLK, BEAN_SENSOR_CS, BEAN_SENSOR_DO);

// Latest corrected temperatures, updated by readTemperature().
float beanTemperature = 0;
float drumTemperature = 0;
