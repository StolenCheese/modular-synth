class SuperColliderController;

#pragma once

#include "sc-controller/SuperColliderCommander.hpp"
#include "sc-controller/Synth.hpp"
#include "sc-controller/Group.hpp"
#include "sc-controller/Bus.hpp"

#include <set>

class SuperColliderController : public SuperColliderCommander {
    std::set<std::string> loaded_synthdefs {};


    int next_node_id { 100 };
    int next_bus_id { 10 };

    Bus outL = Bus( 0);
    Bus outR = Bus(1);
    Bus inL = Bus(2);
    Bus inR = Bus(3);


    SuperColliderController(const SuperColliderController&) = delete;
    SuperColliderController(IpEndpointName endpoint);

    inline static SuperColliderController* s = nullptr;

public:

     Group root{ 0 };


     static void Connect(IpEndpointName endpoint);

     static SuperColliderController& get();



    // Create a synth from a synthdef
    // Will return when Synth class has been fully populated, as apparently the port mechanism requires
    // all parameters to be known in advance
    Synth *InstantiateSynth(const std::string& synthdef);

    // Create new bus with no listeners or sources
    // Represents a supercollider control or audio bus
    Bus InstantiateBus();

    //Synconsize a group to the server
    // Will instanciate any new synths or buses found
    void SyncGroup(Group * g);
     
};
