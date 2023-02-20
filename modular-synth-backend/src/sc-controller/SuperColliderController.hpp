class SuperColliderController;

#pragma once

#include "Group.hpp"
#include "SuperColliderCommander.hpp"
#include "Synth.hpp"
#include <set>

class SuperColliderController : public SuperColliderCommander {
    std::set<std::string> loaded_synthdefs {};

    Group root { this, 0 };

    int next_node_id { 100 };
    int next_bus_id { 10 };

    Bus outL { 0 };
    Bus outR { 1 };
    Bus inL { 2 };
    Bus inR { 3 };

public:
    SuperColliderController(IpEndpointName endpoint);

    // Create a synth from a synthdef
    // Will return when Synth class has been fully populated, as apparently the port mechanism requires
    // all parameters to be known in advance
    std::future<Synth> InstantiateSynth(const std::string& synthdef);

    // Create new bus with no listeners or sources
    // Represents a supercollider control or audio bus
    Bus InstantiateBus();

    void SyncTrees();
};
