//
// Created by misho on 20/04/19.
//

#include <Arduino.h>
#include "Multiplexer.h"
#include "main.h"

void Multiplexer::setup() {

    pinMode(S0, OUTPUT);
    pinMode(S1, OUTPUT);
    pinMode(S2, OUTPUT);
    pinMode(S3, OUTPUT);
    pinMode(SIG, INPUT);
}

int* Multiplexer::readAllInputs() {
    int results[input_count];
    for(int i = 0; i < input_count; i++){
        results[i] = readInput(i);
    }
    return results;

}


int Multiplexer::readInput(unsigned int number) {
    for(unsigned int j = 0; j < 4; j++){
        digitalWrite(select_pins[j], (number >> j) & 1);
    }
    return analogRead(SIG);
}

