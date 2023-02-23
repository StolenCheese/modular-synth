class SuperColliderController;

#pragma once

#include "SuperColliderCommander.hpp"
#include "Synth.hpp"
#include "Group.hpp"
#include "Bus.hpp"
#include <set>

class SuperColliderController : public SuperColliderCommander {
    std::set<std::string> loaded_synthdefs {};


    int next_node_id { 100 };
    int next_bus_id { 10 };

    Bus outL = Bus(this, 0);
    Bus outR = Bus(this, 1);
    Bus inL = Bus(this, 2);
    Bus inR = Bus( this,3 );

public:

    Group root{ this, 0 };


    SuperColliderController(IpEndpointName endpoint);

    // Create a synth from a synthdef
    // Will return when Synth class has been fully populated, as apparently the port mechanism requires
    // all parameters to be known in advance
    std::future<Synth*> InstantiateSynth(const std::string& synthdef);

    // Create new bus with no listeners or sources
    // Represents a supercollider control or audio bus
    Bus InstantiateBus();

    //Synconsize a group to the server
    // Will instanciate any new synths or buses found
    void SyncGroup( Group * g);
     
};
