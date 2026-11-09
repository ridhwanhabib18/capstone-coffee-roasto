// Minimal host-side stand-in for the Arduino core, used only to compare two firmware versions.
#pragma once
#include <cstdint>
#include <iso646.h>
#include <cstdio>
#include <cstdlib>
#include <string>
#include <vector>
#include <map>

typedef unsigned char byte;
#define LOW 0
#define HIGH 1
#define OUTPUT 1
#define abs(x) ((x) > 0 ? (x) : -(x))

unsigned long millis();
void delay(unsigned long ms);
void pinMode(uint8_t pin, uint8_t mode);
void digitalWrite(uint8_t pin, uint8_t value);

class String {
 public:
  String() {}
  String(const char* s) : s_(s) {}
  String(float f) { char b[40]; snprintf(b, sizeof b, "%.2f", f); s_ = b; }
  unsigned int length() const { return (unsigned int)s_.size(); }
  char operator[](unsigned int i) const { return s_[i]; }
  String& operator+=(char c) { s_ += c; return *this; }
  bool operator==(const char* o) const { return s_ == o; }
  bool operator!=(const char* o) const { return s_ != o; }
  float toFloat() const { return (float)atof(s_.c_str()); }
 private:
  std::string s_;
};

struct SerialClass {
  void begin(long) {}
  bool available();
  String readStringUntil(char);
  void print(const char* s);
  void print(float f);
  void println(const char* s);
};
extern SerialClass Serial;
