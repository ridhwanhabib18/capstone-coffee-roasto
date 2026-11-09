// Function prototypes the Arduino IDE would add automatically. Add a line here when a tab gets a new function.
#pragma once
void initActuators();
void setBurner(bool);
void setDrumMotor(bool);
void setBlower(bool);
void setCooler(bool);
void setValveAngle(int);
void setValveFromPid(float);
void switchBurnerByPid(float);
bool readTemperature();
void sendFrame(float, float, float);
float computePid(float, float);
void handleManualCommand(const String&);
void holdManualSetpoint();
void runPidMode();
void runHeatingMode();
void stopRoast();
void completeRoast();
String readCommand();
