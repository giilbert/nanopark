#include <Wire.h>

static bool isController = false;

typedef struct ControllerState
{
    uint8_t controllerId;
    int8_t x;
    int8_t y;
    bool isButtonPressed;
};

typedef struct TwiceControllerState
{
    ControllerState controllerOne;
    ControllerState controllerTwo;
};

void setup()
{
    if (Serial)
    {
        isController = true;
    }

    Serial.begin(115200);
    Wire.begin(1);
}

void loop()
{
    // TwiceControllerState controllerState = requestControllerState(2);

    // Serial.println(String("Controller 1: ") + controllerState.controllerOne.x + ", " + controllerState.controllerOne.y + ", " + controllerState.controllerOne.isButtonPressed);
    // Serial.println(String("Controller 2: ") + controllerState.controllerTwo.x + ", " + controllerState.controllerTwo.y + ", " + controllerState.controllerTwo.isButtonPressed);

    int available = Serial.available();
    if (available > 0)
    {
        uint8_t *buffer = new uint8_t[available];
        Serial.readBytes(buffer, available);
        handleMessage(buffer, available);
        delete buffer;
    }
}

void serializeWriteControllerState(ControllerState state)
{
    Serial.print(state.controllerId);
    Serial.print(",");
    Serial.print(state.x);
    Serial.print(",");
    Serial.print(state.y);
    Serial.print(",");
    Serial.print(state.isButtonPressed ? 1 : 0);
    Serial.print("\n");
}

TwiceControllerState requestControllerState(int from)
{
    Wire.requestFrom(from, 8);

    return TwiceControllerState{
        .controllerOne = ControllerState{
            .controllerId = (uint8_t)Wire.read(),
            .x = (int8_t)Wire.read(),
            .y = (int8_t)Wire.read(),
            .isButtonPressed = (bool)Wire.read(),
        },
        .controllerTwo = ControllerState{
            .controllerId = (uint8_t)Wire.read(),
            .x = (int8_t)Wire.read(),
            .y = (int8_t)Wire.read(),
            .isButtonPressed = (bool)Wire.read(),
        },
    };
}

void respondWithControllerStates()
{
    // Start tag
    Serial.write((uint8_t)0x00);
    Serial.write(0x01);

    TwiceControllerState secondaryOne = requestControllerState(2);
    serializeWriteControllerState(secondaryOne.controllerOne);
    serializeWriteControllerState(secondaryOne.controllerTwo);

    // End tag
    Serial.write((uint8_t)0x00);
    Serial.flush();
}

// From the computer
void handleMessage(uint8_t buffer[], size_t bufferSize)
{
    if (buffer[0] == 0x01)
        respondWithControllerStates();
}