"""Draws the PID characteristic chart used in the README.

Reads KP, KI, PID_MIN, PID_MAX and the servo angles from firmware/Program_PID_CP/Config.h and applies the same
arithmetic as computePid() in Pid.ino, so the picture follows the firmware. Standard library only.

Run:  python tools/pid-charts/make_charts.py
Out:  docs/assets/pid/pid-characteristic.svg
"""
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
CONFIG = ROOT / "firmware" / "Program_PID_CP" / "Config.h"
OUT = ROOT / "docs" / "assets" / "pid" / "pid-characteristic.svg"


def const(name):
    text = CONFIG.read_text(encoding="utf-8")
    match = re.search(r"\b%s\s*=\s*(-?[0-9.]+)" % name, text)
    if not match:
        raise SystemExit("constant %s not found in Config.h" % name)
    return float(match.group(1))


KP, KI = const("KP"), const("KI")
PID_MIN, PID_MAX = const("PID_MIN"), const("PID_MAX")
CLOSED = const("VALVE_CLOSED_ANGLE")
GAIN = KP + KI + 1.0           # computePid(): P + I + D with previous error = 0
E_ON = PID_MAX / GAIN          # error at which PID reaches the maximum (burner on)
E_OFF = PID_MIN / GAIN         # error at which PID reaches the minimum (burner off)


def pid(e):
    return max(PID_MIN, min(PID_MAX, GAIN * e))


# Colours: reference palette slots 1 and 2 (validated), light surface.
SURFACE, INK, INK2, GRID = "#fcfcfb", "#0b0b0b", "#52514e", "#e3e2de"
BLUE, ORANGE, BAND = "#2a78d6", "#eb6834", "#f0efec"

W, H = 780, 700
X0, X1 = 80, 740               # plot area, left and right
E_MIN, E_MAX = -2.0, 4.0       # error range drawn, in degrees C


def sx(e):
    return X0 + (e - E_MIN) / (E_MAX - E_MIN) * (X1 - X0)


def panel_y(top, bottom, lo, hi):
    return lambda v: bottom - (v - lo) / (hi - lo) * (bottom - top)


parts = []
add = parts.append
add('<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 %d %d" width="%d" height="%d" '
    'font-family="Segoe UI, Arial, sans-serif" role="img" aria-labelledby="t d">' % (W, H, W, H))
add('<title id="t">PID output, valve angle and burner state against the temperature error</title>')
add('<desc id="d">PID output rises with a gain of %.3f from 0 at an error of 0 degrees to 40 at an error of %.2f '
    'degrees and stays at 40 above that. The valve angle is 40 minus the PID output. The burner turns on at an '
    'error of %.2f degrees and off at 0 degrees; between them it keeps its last state.</desc>' % (GAIN, E_ON, E_ON))
add('<rect width="%d" height="%d" fill="%s"/>' % (W, H, SURFACE))
add('<text x="%d" y="30" font-size="17" font-weight="600" fill="%s">PID characteristic of the firmware '
    '(KP %.3f, KI %.1f, KD not used)</text>' % (X0, INK, KP, KI))
add('<text x="%d" y="50" font-size="12.5" fill="%s">error e = set point - bean temperature; effective gain '
    'KP + KI + 1 = %.3f</text>' % (X0, INK2, GAIN))

# ---- Panel A: PID output and valve angle -------------------------------------------------------
A_TOP, A_BOT = 96, 360
ya = panel_y(A_TOP, A_BOT, 0, 40)
add('<text x="%d" y="%d" font-size="13" font-weight="600" fill="%s">A. Controller output and valve angle '
    '(0 to 40)</text>' % (X0, A_TOP - 14, INK))
add('<rect x="%.1f" y="%d" width="%.1f" height="%d" fill="%s"/>'
    % (sx(E_OFF), A_TOP, sx(E_ON) - sx(E_OFF), A_BOT - A_TOP, BAND))
for v in (0, 10, 20, 30, 40):
    add('<line x1="%d" x2="%d" y1="%.1f" y2="%.1f" stroke="%s"/>' % (X0, X1, ya(v), ya(v), GRID))
    add('<text x="%d" y="%.1f" font-size="11.5" text-anchor="end" fill="%s">%d</text>' % (X0 - 8, ya(v) + 4, INK2, v))
add('<text transform="translate(%d %d) rotate(-90)" font-size="12" text-anchor="middle" fill="%s">PID output / '
    'servo angle (degrees)</text>' % (24, (A_TOP + A_BOT) // 2, INK2))

steps = 240
pts_pid, pts_servo = [], []
for i in range(steps + 1):
    e = E_MIN + (E_MAX - E_MIN) * i / steps
    pts_pid.append("%.1f,%.1f" % (sx(e), ya(pid(e))))
    pts_servo.append("%.1f,%.1f" % (sx(e), ya(CLOSED - pid(e))))
add('<polyline points="%s" fill="none" stroke="%s" stroke-width="2.5" stroke-linejoin="round"/>'
    % (" ".join(pts_pid), BLUE))
add('<polyline points="%s" fill="none" stroke="%s" stroke-width="2.5" stroke-dasharray="7 4" '
    'stroke-linejoin="round"/>' % (" ".join(pts_servo), ORANGE))
for e in (E_OFF, E_ON):
    add('<line x1="%.1f" x2="%.1f" y1="%d" y2="%d" stroke="%s" stroke-dasharray="2 3"/>'
        % (sx(e), sx(e), A_TOP, A_BOT, INK2))

# direct labels
add('<text x="%.1f" y="%.1f" font-size="12.5" font-weight="600" fill="%s">PID output = %.3f x e</text>'
    % (sx(2.4), ya(40) - 8, BLUE, GAIN))
add('<text x="%.1f" y="%.1f" font-size="12.5" font-weight="600" fill="%s">valve servo angle = 40 - PID</text>'
    % (sx(2.2), ya(0) - 8, ORANGE))
add('<text x="%.1f" y="%.1f" font-size="11.5" text-anchor="middle" fill="%s">only the valve moves</text>'
    % ((sx(E_OFF) + sx(E_ON)) / 2, ya(20) - 2, INK2))
add('<text x="%.1f" y="%.1f" font-size="11.5" text-anchor="middle" fill="%s">(proportional band</text>'
    % ((sx(E_OFF) + sx(E_ON)) / 2, ya(20) + 13, INK2))
add('<text x="%.1f" y="%.1f" font-size="11.5" text-anchor="middle" fill="%s">%.2f degrees wide)</text>'
    % ((sx(E_OFF) + sx(E_ON)) / 2, ya(20) + 27, INK2, E_ON - E_OFF))
add('<text x="%.1f" y="%.1f" font-size="11.5" fill="%s">valve at 0: open, gas flows</text>'
    % (sx(E_ON) + 8, ya(40) + 20, INK2))
add('<text x="%.1f" y="%.1f" font-size="11.5" fill="%s">valve at 40: closed</text>'
    % (X0 + 8, ya(0) - 24, INK2))

# ---- Panel B: burner relay with hysteresis ---------------------------------------------------
B_TOP, B_BOT = 440, 600
yb = panel_y(B_TOP, B_BOT, 0, 1)
add('<text x="%d" y="%d" font-size="13" font-weight="600" fill="%s">B. Burner relay: switches only at the two '
    'limits and keeps its state in between</text>' % (X0, B_TOP - 26, INK))
add('<rect x="%.1f" y="%d" width="%.1f" height="%d" fill="%s"/>'
    % (sx(E_OFF), B_TOP, sx(E_ON) - sx(E_OFF), B_BOT - B_TOP, BAND))
for v, label in ((0, "OFF"), (1, "ON")):
    add('<line x1="%d" x2="%d" y1="%.1f" y2="%.1f" stroke="%s"/>' % (X0, X1, yb(v), yb(v), GRID))
    add('<text x="%d" y="%.1f" font-size="11.5" text-anchor="end" fill="%s">%s</text>' % (X0 - 8, yb(v) + 4, INK2, label))
for e in (E_OFF, E_ON):
    add('<line x1="%.1f" x2="%.1f" y1="%d" y2="%d" stroke="%s" stroke-dasharray="2 3"/>'
        % (sx(e), sx(e), B_TOP, B_BOT, INK2))
# error rising (cooler to hotter is falling error; draw both directions)
# bean heating up: error falls from large to negative -> relay turns off at e <= 0
add('<polyline points="%.1f,%.1f %.1f,%.1f %.1f,%.1f %.1f,%.1f" fill="none" stroke="%s" stroke-width="2.5" '
    'stroke-linejoin="round"/>' % (sx(E_MAX), yb(1) + 4, sx(E_OFF), yb(1) + 4, sx(E_OFF), yb(0) - 4, sx(E_MIN), yb(0) - 4, BLUE))
# bean cooling down: error rises from negative -> relay turns on at e >= E_ON
add('<polyline points="%.1f,%.1f %.1f,%.1f %.1f,%.1f %.1f,%.1f" fill="none" stroke="%s" stroke-width="2.5" '
    'stroke-dasharray="7 4" stroke-linejoin="round"/>' % (sx(E_MIN), yb(0) + 8, sx(E_ON), yb(0) + 8, sx(E_ON), yb(1) + 8, sx(E_MAX), yb(1) + 8, ORANGE))
add('<text x="%.1f" y="%.1f" font-size="12" font-weight="600" fill="%s">bean heating up (e falling): turns OFF at '
    'e &lt;= 0</text>' % (sx(0.2), yb(1) - 12, BLUE))
add('<text x="%.1f" y="%.1f" font-size="12" font-weight="600" fill="%s">bean cooling down (e rising): turns ON at '
    'e &gt;= %.2f</text>' % (sx(-1.9), yb(0) - 30, ORANGE, E_ON))

# ---- shared x axis -----------------------------------------------------------------------------
for panel_bottom in (A_BOT, B_BOT):
    add('<line x1="%d" x2="%d" y1="%d" y2="%d" stroke="%s"/>' % (X0, X1, panel_bottom, panel_bottom, INK2))
for t in range(int(E_MIN), int(E_MAX) + 1):
    add('<text x="%.1f" y="%d" font-size="11.5" text-anchor="middle" fill="%s">%d</text>' % (sx(t), B_BOT + 18, INK2, t))
for e, name in ((E_OFF, "0"), (E_ON, "%.2f" % E_ON)):
    add('<text x="%.1f" y="%d" font-size="11.5" text-anchor="middle" font-weight="600" fill="%s">%s</text>'
        % (sx(e), A_BOT + 16, INK, name))
add('<text x="%d" y="%d" font-size="12" text-anchor="middle" fill="%s">error e = set point - bean temperature '
    '(degrees C)</text>' % ((X0 + X1) // 2, B_BOT + 40, INK2))
add('<text x="%d" y="%d" font-size="12" fill="%s">Example: with a set point of 150 C the burner turns on below '
    '%.1f C and off at 150 C or above.</text>' % (X0, B_BOT + 70, INK2, 150 - E_ON))
add('</svg>')

OUT.parent.mkdir(parents=True, exist_ok=True)
OUT.write_text("\n".join(parts) + "\n", encoding="utf-8")
print("wrote %s (gain %.3f, burner on at e >= %.3f, off at e <= %.3f)" % (OUT.relative_to(ROOT), GAIN, E_ON, E_OFF))
