
#include "libremidi/reader.hpp"
#include <chrono>
#include <thread>
#include <fstream>
#include "sc-controller/MidiSynth.hpp"
#include "sc-controller/SuperColliderController.hpp"


float from_midi_note(int note) {
	 return 440.0 * std::exp2((note - 69) / 12.0);
}

void MidiSynth::ControlLoop(std::string const& source)
{ 

	//read file
	std::ifstream infile{ source, std::ios_base::binary };

	std::vector<uint8_t> bytes;
	bytes.assign(std::istreambuf_iterator<char>(infile), std::istreambuf_iterator<char>());

	libremidi::reader r{true};
	//Parse the file
	auto res = r.parse(bytes);

	if (res == libremidi::reader::parse_result::invalid) {
		throw std::invalid_argument("MIDI from " + source + " invalid!");
	}

	if (r.tracks.size() == 0) {
		throw std::invalid_argument("MIDI from " + source + " has no tracks!");
	}

	float bps = r.startingTempo / 60.0;


	std::array<int, 16> track_clock{};
	std::array<std::array<int, 2>, 16> playing{};

	int tick = 0;

	std::chrono::steady_clock clock{};

	auto last_clock = clock.now();

	//Loop it forever, checking state of source object
	while (true) {

		int next = -1;
		int next_tick = 10000000;
		libremidi::message msg;

		for (size_t i = 0; i < r.tracks.size(); i++)
		{
			if (r.tracks[i].size() > track_clock[i] && r.tracks[i][track_clock[i]].tick < next_tick) {
				next = i;
				next_tick = r.tracks[i][track_clock[i]].tick;
				msg = r.tracks[i][track_clock[i]].m;
			}
		}

		if (next == -1) {
			// loop!
			track_clock = {};
			playing = {};
			tick = 0; 
			bps = r.startingTempo / 60.0;
			continue;
		}

		track_clock[next]++;
		
		// Wait for the event to happen, minus the time that has elapsed since the last event
		auto process_time = clock.now() - last_clock;

		std::this_thread::sleep_for(std::chrono::microseconds((int)(1000000 * (next_tick - tick)/ r.ticksPerBeat / bps)) - process_time);
		//std::this_thread::sleep_for(std::chrono::milliseconds(100));
		last_clock = clock.now();

		tick = next_tick;

		int note;

		std::cout << "Event at " << tick << " : ";
		if (msg.is_meta_event())
		{
			switch (msg.get_meta_event_type())
			{
			case libremidi::meta_event_type::TEMPO_CHANGE:
			{
				int len = msg.bytes[2];

				int tempo = 0;

				std::cout << "Tempo change to ";
				for (size_t i = 0; i < len; i++)
				{
					std::cout << std::to_string(msg.bytes[3 + i]) << " ";
					tempo <<= 8;
					tempo += msg.bytes[3 + i];
				}
				std::cout << " = " << tempo;

				bps = (60000000.0 / tempo) / 60;

				std::cout << ", bps = " << bps;

				break;
			}

			default:
				std::cout << "Meta event";
				break;
			}
		}
		else
		{
			switch (msg.get_message_type())
			{
			case libremidi::message_type::NOTE_ON:
				note = (int)msg.bytes[1];
				std::cout << "Note ON: "
					<< "channel " << msg.get_channel() << ' '
					<< "note " << note << ' '
					<< "velocity " << (int)msg.bytes[2] << ' ';

				if (playing[next][0] == 0) {
					playing[next][0] = note;
					channels[next][0].set(from_midi_note(note));
				}
				else if (playing[next][1] == 0) {
					playing[next][1] = note;
					channels[next][1].set(from_midi_note(note));
				}
				else {
					std::cout << "- Missed the note!";
				}

				break;
			case libremidi::message_type::NOTE_OFF:
				
				note = (int)msg.bytes[1];
				std::cout << "Note OFF: "
					<< "channel " << msg.get_channel() << ' '
					<< "note " << note << ' '
					<< "velocity " << (int)msg.bytes[2] << ' ';

				if (playing[next][0] == note) {
					playing[next][0] = 0;
					channels[next][0].set(0);
				}else if (playing[next][1] == note) {
					playing[next][1] = 0;
					channels[next][1].set(0);
				}

				break;
			case libremidi::message_type::CONTROL_CHANGE:
				std::cout << "Control: "
					<< "channel " << msg.get_channel() << ' '
					<< "control " << (int)msg.bytes[1] << ' '
					<< "value " << (int)msg.bytes[2] << ' ';
				break;
			case libremidi::message_type::PROGRAM_CHANGE:
				std::cout << "Program: "
					<< "channel " << msg.get_channel() << ' '
					<< "program " << (int)msg.bytes[1] << ' ';
				break;
			case libremidi::message_type::AFTERTOUCH:
				std::cout << "Aftertouch: "
					<< "channel " << msg.get_channel() << ' '
					<< "value " << (int)msg.bytes[1] << ' ';
				break;
			case libremidi::message_type::POLY_PRESSURE:
				std::cout << "Poly pressure: "
					<< "channel " << msg.get_channel() << ' '
					<< "note " << (int)msg.bytes[1] << ' '
					<< "value " << (int)msg.bytes[2] << ' ';
				break;
			case libremidi::message_type::PITCH_BEND:
				std::cout << "Poly pressure: "
					<< "channel " << msg.get_channel() << ' '
					<< "bend " << (int)(msg.bytes[1] << 7 + msg.bytes[2]) << ' ';
				break;
			default:
				std::cout << "Unsupported.";
				break;
			}
		}
		std::cout << '\n';

	}
}

MidiSynth::MidiSynth(std::string const& source) : control(&MidiSynth::ControlLoop, this, source), Synth(-1, "", "")
{
	// Create a new thread to handle midi enable/disable commands
	// Can't wait to cause every race condition
	// I love c++

	for (size_t i = 0; i < 16; i++)
	{
		for (size_t j = 0; j < 2; j++)
		{
			channels[i][j] = SuperColliderController::get().InstantiateBus();
			//cheese
			controls["out" + std::to_string(i) + "_" + std::to_string(j)] = channels[i][j];
		}

	}
}
