#include "sc-controller/MidiSynth.hpp"
#include <chrono>
#include <thread>
#include <fstream>
#include "libremidi/reader.hpp"

void MidiSynth::ControlLoop(std::string const& source)
{
 

	//read file
	std::ifstream infile(source, std::ios_base::binary);

	std::istream_iterator<uint8_t> start(infile), end;
	std::vector<uint8_t> b(start, end);
 
	libremidi::reader r;
	//Parse the file
	auto res = r.parse(b);

	if (res == libremidi::reader::parse_result::invalid) {
		throw std::invalid_argument("MIDI from "+source+" invalid!");
	}

	float tempo = r.startingTempo;

	//Loop it forever, checking state of source object
	while (true) {
		std::cout << "midi tick!" << std::endl;

		// Find the next event that will happen
        for (const auto& track : r.tracks)
        {
            std::cout << "\nNew track\n\n";
            for (const libremidi::track_event& event : track)
            {
                std::cout << "Event at " << event.tick << " : ";
                if (event.m.is_meta_event())
                {
                    std::cout << "Meta event";
                }
                else
                {
                    switch (event.m.get_message_type())
                    {
                    case libremidi::message_type::NOTE_ON:
                        std::cout << "Note ON: "
                            << "channel " << event.m.get_channel() << ' '
                            << "note " << (int)event.m.bytes[1] << ' '
                            << "velocity " << (int)event.m.bytes[2] << ' ';
                        break;
                    case libremidi::message_type::NOTE_OFF:
                        std::cout << "Note OFF: "
                            << "channel " << event.m.get_channel() << ' '
                            << "note " << (int)event.m.bytes[1] << ' '
                            << "velocity " << (int)event.m.bytes[2] << ' ';
                        break;
                    case libremidi::message_type::CONTROL_CHANGE:
                        std::cout << "Control: "
                            << "channel " << event.m.get_channel() << ' '
                            << "control " << (int)event.m.bytes[1] << ' '
                            << "value " << (int)event.m.bytes[2] << ' ';
                        break;
                    case libremidi::message_type::PROGRAM_CHANGE:
                        std::cout << "Program: "
                            << "channel " << event.m.get_channel() << ' '
                            << "program " << (int)event.m.bytes[1] << ' ';
                        break;
                    case libremidi::message_type::AFTERTOUCH:
                        std::cout << "Aftertouch: "
                            << "channel " << event.m.get_channel() << ' '
                            << "value " << (int)event.m.bytes[1] << ' ';
                        break;
                    case libremidi::message_type::POLY_PRESSURE:
                        std::cout << "Poly pressure: "
                            << "channel " << event.m.get_channel() << ' '
                            << "note " << (int)event.m.bytes[1] << ' '
                            << "value " << (int)event.m.bytes[2] << ' ';
                        break;
                    case libremidi::message_type::PITCH_BEND:
                        std::cout << "Poly pressure: "
                            << "channel " << event.m.get_channel() << ' '
                            << "bend " << (int)(event.m.bytes[1] << 7 + event.m.bytes[2]) << ' ';
                        break;
                    default:
                        std::cout << "Unsupported.";
                        break;
                    }
                }
                std::cout << '\n';
            }
        }

		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
	}
}

MidiSynth::MidiSynth(std::string const& source) : control(&MidiSynth::ControlLoop, this, source), Synth(-1, {})
{
	// Create a new thread to handle midi enable/disable commands
	// Can't wait to cause every race condition
	// I love c++
	 
	 
}
