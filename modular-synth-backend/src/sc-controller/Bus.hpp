#pragma once

#include "SuperColliderController.hpp"

enum class BusRate {
    CONTROL,
    AUDIO
};

class Bus {
    friend class SuperColliderController;

private:
    int index;
    SuperColliderController* s;
    BusRate rate { BusRate::CONTROL };

    Bus::Bus(SuperColliderController* s, int index);

public:
    // a symbol consisting of the letter 'c' or 'a' (for control or audio) followed by the bus's index. This may be used when setting a synth node's control inputs to map the input to the control bus.
    std::string asMap();

    std::future<std::variant<int, float>> get();

    // Associates to /b_set
    void set(const float v);
    // Associates to /b_set
    void set(const int v);
};
