#include "sc-controller/Synth.hpp"


class MidiSynth : public Synth {
private:
	std::thread control;
	[[ noreturn ]]void ControlLoop(std::string const& source);
public:
	MidiSynth(std::string const& source);
};