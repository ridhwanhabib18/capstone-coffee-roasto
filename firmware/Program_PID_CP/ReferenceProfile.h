// The reference temperatures (set points) sent by the GUI, ten at a time.
//
// A new line is first stored as "pending". It only becomes "active" when the ten values in use
// are finished, so a line that arrives early cannot overwrite set points still being followed.
// One set point is used per averaged reading (~4 s), so ten last about 40 s.
#pragma once
#include <Arduino.h>
#include "Config.h"

class ReferenceProfile {
 public:
  ReferenceProfile() : index(PROFILE_GROUP_SIZE - 1) {
    for (int i = 0; i < PROFILE_GROUP_SIZE; i++) {
      pending[i] = 0;
      active[i] = 0;
    }
  }

  // Stores a line such as "140,142,145,...". Missing or empty values become 0, which ends the roast.
  void load(const String& line) {
    for (int i = 0; i < PROFILE_GROUP_SIZE; i++) {
      pending[i] = splitField(line, ',', i).toFloat();
    }
  }

  // The next set point. When the active group is used up, the pending group becomes active;
  // if no new line arrived, the same group is used again.
  float next() {
    if (index == PROFILE_GROUP_SIZE - 1) {
      for (int i = 0; i < PROFILE_GROUP_SIZE; i++) {
        active[i] = pending[i];
      }
      index = -1;
    }
    index++;
    return active[index];
  }

 private:
  float pending[PROFILE_GROUP_SIZE];
  float active[PROFILE_GROUP_SIZE];
  int index;

  // The field with the given number (0 = first) of a line separated by 'separator'.
  static String splitField(const String& data, char separator, int wanted) {
    int fieldNumber = 0;
    String field = "";
    for (unsigned int i = 0; i < data.length(); i++) {
      if (data[i] == separator) {
        fieldNumber++;
      } else if (fieldNumber == wanted) {
        field += data[i];
      }
    }
    return field;
  }
};
