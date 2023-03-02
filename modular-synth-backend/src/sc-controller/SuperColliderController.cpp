#include "sc-controller/SuperColliderController.hpp"
#include "sc-controller/MidiSynth.hpp"
#include "include/sc-controller/exceptions/UninitializedConnectionException.h"
#include <fstream>
#include <map>

void print(osc::ReceivedMessage& m)
{
	std::cout << m.AddressPattern() << ": ";

	for (auto it = m.ArgumentsBegin(); it != m.ArgumentsEnd(); ++it) {
		auto& element = *it;

		switch (element.TypeTag()) {
		case 'i':
			std::cout << element.AsInt32() << " ";
			break;

		default:
			std::cout << " (type: " << element.TypeTag() << ") ";
		}
	}

	std::cout << std::endl;
}

SuperColliderController::SuperColliderController(IpEndpointName endpoint)
	: SuperColliderCommander(endpoint)
{
}

void SuperColliderController::Connect(IpEndpointName endpoint)
{
	s = new SuperColliderController(endpoint);
}

SuperColliderController& SuperColliderController::get()
{
	if (s == nullptr) {
		throw std::exception("Invalid connection to supercollider server");
	}
	return *s;
}

std::string extractSynthdef(std::string const& path) {
	auto dot = path.find('.');
	auto slash = path.find_last_of('\\');
	if (dot != std::string::npos && slash != std::string::npos)
		return path.substr(slash + 1, dot - slash - 1);
	return "";
}

bool  SuperColliderController::loadSynthDefFile(const std::string& source, SuperColliderPacketBuilder* completion) {

	if (!loaded_synthdefs.count(source)) {
		auto dot = source.find('.');
		auto slash = source.find_last_of('\\');

		if (dot == std::string::npos || slash == std::string::npos) {
			throw std::invalid_argument("source path invalid");
		}

		auto file_type = source.substr(dot);
		 

		if (file_type == ".scsyndef") {

			auto synthdef = source.substr(slash + 1, dot - slash - 1);

			std::cout << "Loading" << source << "for the first time" << std::endl;

			std::ifstream def;
			def.open(source);

			if (def) {

				std::string str((std::istreambuf_iterator<char>(def)),
					std::istreambuf_iterator<char>());

				if (completion == nullptr) {
					auto m = d_recv(str);
				}
				else {
					auto m = d_recv(str, *completion);
				}

				def.close();

				loaded_synthdefs.insert(source);

				return true;
			}
			else {
				throw std::invalid_argument("Could not open " + source);
			}
		}
		else {
			throw std::invalid_argument("Could not open " + source + ": Incorrect file format " + file_type);
		}
	}
	else {
		// File good, but already exists in the server's collection
		return false;
	}
}

MidiSynth* SuperColliderController::InstantiateMidiSynth(const std::string& midi_source)
{
	std::cout << "Loading midi" << std::endl;
	return new MidiSynth(midi_source);
}

int SuperColliderController::allocateSynthID() {
	//Find a free node id
	while (root.subtree.count(next_node_id)) {
		next_node_id++;
	}
	return next_node_id++;
}

Synth* SuperColliderController::InstantiateSynth(const std::string& audio_source, const std::string& control_source)
{
	//Find a free node id
	while (root.subtree.count(next_node_id)) {
		next_node_id++;
	}
	const int id = allocateSynthID();


	std::array<char, 100> completion_buf{};

	SuperColliderPacketBuilder completion{ completion_buf.data(), completion_buf.size() };

	auto audio_synth = extractSynthdef(audio_source);
	auto control_synth = extractSynthdef(control_source);

	auto s = new Synth(id, audio_synth, control_synth);
	root.subtree[id] = s;

	if (audio_synth != "") {

		completion.s_new(audio_synth, id, 0, 0, {});

		if (!loadSynthDefFile(audio_source, &completion)) {
			s_new(audio_synth, id, 0, 0, {});
		}


		// Of course possible to have both. This could be included in completion, but I'm lazy
		if (control_synth != "") {
			loadSynthDefFile(control_source, nullptr);
		}
	}
	else if (control_synth != "") {
		completion.s_new(control_synth, id, 0, 0, {});

		if (!loadSynthDefFile(control_source, &completion)) {
			s_new(audio_synth, id, 0, 0, {});
		}
	}
	else {
		throw std::invalid_argument("Synth must have either valid control or audio synthdef to be instantiated!");
	}

	SyncGroup(&root);

	return s;
}

Bus SuperColliderController::InstantiateBus()
{
	// Max polyphony of 8 - maybe
	next_bus_id += 8;
	return Bus(next_bus_id);
}

bool SuperColliderController::overridePacketReception(osc::ReceivedMessage& msg) {
	if (std::string("/fail").compare(msg.AddressPattern()) == 0) {

		std::cout << "Intercepted Failure" << std::endl;

		return true;
	}
	else if (msg.AddressPattern() == "/g_queryTree.reply") {


		return false;
	}

	return false;
}

void SuperColliderController::SyncGroup(Group* g)
{
	auto m = g_queryTree({ { g->index, 1 } });
	 
	auto it = m.ArgumentsBegin();

	int32_t flag = (it++)->AsInt32();
	int32_t gNodeID = (it++)->AsInt32();

	assert(gNodeID == g->index);

	int32_t children = (it++)->AsInt32();

	for (size_t i = 0; i < children; i++)
	{


		int32_t nodeID = (it++)->AsInt32();
		int32_t children = (it++)->AsInt32();

		if (children == -1) {
			//This is a synth

			auto synthdef = (it++)->AsString();
			std::map<std::string, std::variant<  int, float, Bus>> controls;

			if (flag) {
				int controlCount = (it++)->AsInt32();
				for (size_t j = 0; j < controlCount; j++)
				{
					auto cs = (it++)->AsString();
					if (it->IsFloat()) {
						controls.insert({ cs,it->AsFloat() });
					}
					else {
						controls.insert({ cs,Bus(std::stoi(it->AsString() + 1)) });
					}
					it++;
				}
			}

			static_cast<Synth*>(g->subtree[nodeID])->loadControls(controls);

		}
		else {
			if (!g->subtree.count(nodeID))
				g->subtree.insert({ nodeID, new Group(nodeID) });
		}


		//else, This is another group

	}

}


