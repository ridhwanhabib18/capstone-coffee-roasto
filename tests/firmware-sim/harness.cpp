// Host-side simulation of the Coffee Roasto firmware.
//
// The sketch is compiled together with a minimal stand-in for the Arduino core (shim/) and run against
// a scripted scenario: operator commands arrive at set times and a simulated roaster provides the
// thermocouple readings. The program prints a trace of everything the firmware does (pin changes,
// servo angles, serial lines). run.ps1 compares each trace with the one in golden/.
//
// Clock: virtual. It only moves when the firmware calls Serial.available() (+7 ms) or delay().
#include "Arduino.h"
#include <cmath>
#include <fstream>
#include <stdexcept>

static unsigned long g_now = 0;
static std::map<int, int> g_pin;
static int g_servo = -1;
static std::string g_line;  // serial text of the data frame being built
static std::vector<std::pair<unsigned long, std::string>> g_input;
static size_t g_next = 0;
static long g_availableCalls = 0;
static long g_maxCalls = 0;
static int g_scenario = 0;
SerialClass Serial;

static void trace(const std::string& s) { printf("t=%lu %s\n", g_now, s.c_str()); }

unsigned long millis() { return g_now; }
void delay(unsigned long ms) { g_now += ms; }
void pinMode(uint8_t, uint8_t) {}

void digitalWrite(uint8_t pin, uint8_t v) {
  auto it = g_pin.find(pin);
  if (it == g_pin.end() || it->second != v) {  // only changes are traced
    g_pin[pin] = v;
    trace("pin " + std::to_string(pin) + "=" + std::to_string(v));
  }
}

void traceServo(int angle) {
  if (angle != g_servo) {
    g_servo = angle;
    trace("servo " + std::to_string(angle));
  }
}

bool SerialClass::available() {
  g_now += 7;
  if (++g_availableCalls > g_maxCalls) throw std::runtime_error("end of scenario");
  return g_next < g_input.size() && g_input[g_next].first <= g_now;
}

String SerialClass::readStringUntil(char) {
  String s(g_input[g_next].second.c_str());
  trace("recv '" + g_input[g_next].second + "'");
  g_next++;
  return s;
}

void SerialClass::print(const char* s) {
  if (s[0] == '\n') {
    trace("frame " + g_line);
    g_line.clear();
  } else {
    g_line += s;
  }
}
void SerialClass::print(float f) {
  char b[40];
  snprintf(b, sizeof b, "%.2f", f);
  g_line += b;
}
void SerialClass::println(const char* s) { trace(std::string("send ") + s); }

// ---- The simulated roaster --------------------------------------------------------------------

// Closed loop for scenario 12: the temperature rises while the burner relay (pin 9) is on and falls
// back towards the room temperature otherwise.
static double g_bean = 140, g_drum = 152, g_lastSec = 0;
static void stepRoaster(double sec) {
  double dt = sec - g_lastSec;
  if (dt <= 0) return;
  bool burnerOn = g_pin.count(9) && g_pin[9] == LOW;
  g_bean += ((burnerOn ? 0.9 : 0.0) - 0.004 * (g_bean - 25)) * dt;
  g_drum += ((burnerOn ? 0.95 : 0.0) - 0.0035 * (g_drum - 25)) * dt;
  g_lastSec = sec;
}

// Raw reading of a MAX6675 (bean on chip select 7, drum on chip select 4), before the firmware's calibration.
float fakeRawCelsius(int cs) {
  double sec = g_now / 1000.0;
  if (g_scenario == 12) {
    stepRoaster(sec);
    return (float)(cs == 7 ? 0.9194 * g_bean + 3.4921 : 0.8711 * g_drum + 6.8969);
  }
  if (g_scenario >= 9) {  // hovers around the set point, so the PID leaves saturation
    double centre = g_scenario == 11 ? 100.0 : 150.0;
    double t = centre + 2.5 * std::sin(sec / 15.0) + 0.3 * std::sin(sec / 3.1);
    return (float)(cs == 7 ? 0.9194 * t + 3.4921 : 0.8711 * (t + 5) + 6.8969);
  }
  double bean = 25 + 0.30 * sec + 1.5 * std::sin(sec / 7.0);
  double drum = 30 + 0.35 * sec + 1.0 * std::sin(sec / 5.0);
  if (g_scenario == 4 && sec > 60) bean += 400;  // over-temperature
  return (float)(cs == 7 ? bean : drum);
}

#include SKETCH

// The profile as the GUI sends it: groups of ten values, a shorter last group keeps its trailing comma.
static void scheduleProfile(const char* path, unsigned long firstAtMs, unsigned long everyMs) {
  std::ifstream in(path);
  std::vector<std::string> values;
  for (std::string v; std::getline(in, v);) {
    if (!v.empty() && v.back() == '\r') v.pop_back();
    if (!v.empty()) values.push_back(v);
  }
  std::string payload;
  unsigned long at = firstAtMs;
  for (size_t i = 0; i < values.size(); i++) {
    if ((i + 1) % 10 == 0) {
      g_input.push_back({at, payload + values[i]});
      payload.clear();
      at += everyMs;
    } else {
      payload += values[i] + ",";
    }
  }
  if (!payload.empty()) g_input.push_back({at, payload});
}

int main(int argc, char** argv) {
  g_scenario = argc > 1 ? atoi(argv[1]) : 1;
  switch (g_scenario) {
    case 1:  // manual outputs and a typed set point
      g_maxCalls = 60000;
      g_input = {{2000, "n"}, {3000, "t"}, {4000, "y"}, {5000, "p"}, {6000, "v"}, {7000, "x"}, {8000, "k"},
                 {9000, "d"}, {10000, "b"}, {11000, "c"}, {20000, "120"}, {60000, "0"}, {70000, "s"}};
      break;
    case 2:  // heating until it times out
      g_maxCalls = 120000;
      g_input = {{3000, "h"}};
      break;
    case 3:  // heating stopped by the operator
      g_maxCalls = 20000;
      g_input = {{3000, "h"}, {120000, "s"}};
      break;
    case 4:  // PID mode ends on over-temperature
      g_maxCalls = 30000;
      g_input = {{3000, "q"}, {20000, "140,142,145,148,150,152,155,158,160,162"}};
      break;
    case 5:  // PID mode with two lines, the last one short: ends by itself
      g_maxCalls = 60000;
      g_input = {{3000, "q"}, {30000, "60,70,80,90,100,110,120,130,140,150"}, {70000, "160,170,0"}};
      break;
    case 6:  // PID mode stopped by the operator
      g_maxCalls = 30000;
      g_input = {{3000, "q"}, {30000, "100,110,120,130,140,150,160,170,180,190"}, {90000, "s"}};
      break;
    case 7:  // the profile repeats (no short last line) until stopped
      g_maxCalls = 40000;
      g_input = {{3000, "q"}, {30000, "100,110,120,130,140,150,160,170,180,190"}, {200000, "s"}};
      break;
    case 8:  // set point typed, then a PID run
      g_maxCalls = 40000;
      g_input = {{2000, "n"}, {5000, "130"}, {40000, "q"}, {60000, "150,150,150,150,150,150,150,150,150,150"}, {120000, "s"}};
      break;
    case 9:  // heating with temperatures near the set point
      g_maxCalls = 120000;
      g_input = {{3000, "h"}};
      break;
    case 10:  // PID around the profile
      g_maxCalls = 60000;
      g_input = {{3000, "q"}, {20000, "148,150,152,150,149,151,150,150,152,148"}, {140000, "150,151,149,0"}};
      break;
    case 11:  // manual set point 100
      g_maxCalls = 40000;
      g_input = {{2000, "100"}};
      break;
    case 12:  // a whole roast: PID mode following the recorded profile in a closed loop (also the demo data)
      g_maxCalls = 262000;
      g_input = {{3000, "q"}};
      scheduleProfile(argc > 2 ? argv[2] : "demo-profile.txt", 10000, 40000);
      break;
    default:
      fprintf(stderr, "unknown scenario %d\n", g_scenario);
      return 2;
  }

  try {
    setup();
    for (;;) loop();
  } catch (const std::exception&) {
    // end of the scenario
  }
  return 0;
}
