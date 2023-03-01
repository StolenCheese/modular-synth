# pragma once

#include "sc-controller/Synth.hpp"
#include "sc-controller/Bus.hpp"
#include <thread>
#include <array>

class MidiSynth : public Synth {
private:
	std::thread control;
	[[ noreturn ]]void ControlLoop(std::string const& source);
public:
	std::array<std::array<Bus,2>, 16> channels{};
	MidiSynth(std::string const& source); 
};