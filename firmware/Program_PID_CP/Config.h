// Coffee Roasto firmware - all fixed values in one place.
// Pins and wiring: docs/en/hardware.md. Serial contract: docs/en/serial-protocol.md.
#pragma once
#include <Arduino.h>

// ---- Serial ----------------------------------------------------------------------------------
const long SERIAL_BAUD = 9600;               // must match the GUI

// ---- Output pins (relays are active LOW: HIGH = off, LOW = on) --------------------------------
const uint8_t PIN_BURNER = 9;
const uint8_t PIN_COOLER = 10;
const uint8_t PIN_VALVE_SERVO = 11;
const uint8_t PIN_DRUM_MOTOR = 12;
const uint8_t PIN_BLOWER = 13;

const uint8_t RELAY_ON = LOW;
const uint8_t RELAY_OFF = HIGH;

// ---- Gas valve (servo angle in degrees) -------------------------------------------------------
const int VALVE_OPEN_ANGLE = 0;
const int VALVE_CLOSED_ANGLE = 40;

// ---- Thermocouple amplifiers (MAX6675) ---------------------------------------------------------
const uint8_t DRUM_SENSOR_DO = 3;
const uint8_t DRUM_SENSOR_CS = 4;
const uint8_t DRUM_SENSOR_CLK = 5;
const uint8_t BEAN_SENSOR_DO = 6;
const uint8_t BEAN_SENSOR_CS = 7;
const uint8_t BEAN_SENSOR_CLK = 8;

// ---- Sampling ---------------------------------------------------------------------------------
const unsigned long SAMPLE_INTERVAL_MS = 1000;  // one reading per sensor per second
const int SAMPLES_PER_READING = 4;              // averaged, so one data frame every ~4 s
const float DEAD_BAND = 0.25;                   // MAX6675 resolution; smaller changes are ignored

// Linear-regression correction measured against a reference thermometer (ADR-004).
// Redo the regression when a probe or amplifier is replaced.
const float BEAN_OFFSET = 3.4921;
const float BEAN_SLOPE = 0.9194;
const float DRUM_OFFSET = 6.8969;
const float DRUM_SLOPE = 0.8711;

// ---- Control ----------------------------------------------------------------------------------
const float KP = 21.095;   // gains from the MATLAB simulation
const float KI = 0.2;
// KD (= 30 in the simulation) is not used by the firmware; see system design, known limitation 1.
const float PID_MIN = 0;
const float PID_MAX = 40;  // same range as the valve servo angle

const float WARMUP_SETPOINT = 150;              // degrees C, used by heating mode and before a profile arrives
const float OVER_TEMPERATURE = 250;             // degrees C, PID mode stops at this bean temperature
const unsigned long HEATING_DURATION_S = 600;   // heating mode length

// ---- Reference profile ------------------------------------------------------------------------
const int PROFILE_GROUP_SIZE = 10;              // set points per line sent by the GUI
