//
// Created by misho on 20/04/19.
//
#include <Arduino.h>
#include "Multiplexer.h"

void Multiplexer::setup() {

    pinMode(S0, OUTPUT);
    pinMode(S1, OUTPUT);
    pinMode(S2, OUTPUT);
    pinMode(S3, OUTPUT);
    pinMode(SIG, INPUT);
}

void Multiplexer::readAllInputs(int results[]) {
    for(int i = 0; i < input_count; i++){
        results[i] = readInput(i);
    }
}


int Multiplexer::readInput(unsigned int number) {
    for(unsigned int j = 0; j < 4; j++){
        digitalWrite(select_pins[j], (number >> j) & 1);
    }
    return analogRead(SIG);
}

