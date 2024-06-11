#include <Wire.h>

const int ID = 3;

const int CONTROLLER1_ID = 3;
const int CONTROLLER2_ID = 4;

typedef struct ControllerState
{
    uint8_t controllerId;
    int8_t x;
    int8_t y;
    bool isButtonPressed;
};

void setup()
{
    Serial.begin(115200);
    Wire.begin(ID);
    Wire.onRequest(handleRequest);

    pinMode(2, INPUT_PULLUP);
    pinMode(3, INPUT_PULLUP);
    pinMode(A0, INPUT);
    pinMode(A1, INPUT);
    pinMode(A2, INPUT);
    pinMode(A3, INPUT);
}

void loop()
{
}

void handleRequest()
{
    Wire.write((uint8_t)CONTROLLER1_ID);
    Wire.write((int8_t)map(analogRead(A0), 0, 1023, -127, 127));
    Wire.write((int8_t)map(analogRead(A1), 0, 1023, -127, 127));
    Wire.write((uint8_t)digitalRead(2) == LOW);

    Wire.write((uint8_t)CONTROLLER2_ID);
    Wire.write((int8_t)map(analogRead(A2), 0, 1023, -127, 127));
    Wire.write((int8_t)map(analogRead(A3), 0, 1023, -127, 127));
    Wire.write((uint8_t)digitalRead(3) == LOW);
}
