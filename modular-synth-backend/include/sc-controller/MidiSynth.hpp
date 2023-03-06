
class MidiSynth;
#pragma once

#include "sc-controller/Bus.hpp"
#include "sc-controller/Synth.hpp"
#include <array>
#include <thread>

class MidiSynth : public Synth {
private:
    std::thread control;
    [[noreturn]] void ControlLoop(std::string const& source);

public:
    std::array<std::array<std::string, 2>, 16> channels {};
    MidiSynth(std::string const& source);
};