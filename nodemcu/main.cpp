//
// Created by misho on 20/04/19.
//

#include "main.h"

#include <Arduino.h>
#include "Multiplexer.h"

#define INPUT_COUNT 11

Multiplexer multiplexer(INPUT_COUNT);

void setup() {
    Serial.begin(9600);
}

void loop() {
    int values[INPUT_COUNT];
    memcpy(values, multiplexer.readAllInputs(), INPUT_COUNT);

}
