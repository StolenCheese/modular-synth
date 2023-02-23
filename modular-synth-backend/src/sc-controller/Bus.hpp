class Bus;

#pragma once

#include<string>

#include<variant>

#include<future>

enum class BusRate {
    CONTROL,
    AUDIO
};

class SuperColliderController; 

class Bus {
    friend class SuperColliderController; 

private:
    SuperColliderController* s;

    Bus(SuperColliderController* s, int index);

public:
    BusRate rate{ BusRate::CONTROL };
   const int index;
    // a symbol consisting of the letter 'c' or 'a' (for control or audio) followed by the bus's index. This may be used when setting a synth node's control inputs to map the input to the control bus.
    std::string asMap();

    std::future<std::variant<int, float>> get();

    // Associates to /b_set
    void set(const float v);
    // Associates to /b_set
    void set(const int v);
};
