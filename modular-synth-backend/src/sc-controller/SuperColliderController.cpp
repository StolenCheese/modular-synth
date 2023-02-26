#include "sc-controller/SuperColliderController.hpp"
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
        throw UnitializedConnectionException("Invalid connection to supercollider server");
    }
    return *s;
}

Synth* SuperColliderController::InstantiateSynth(const std::string& synthdef)
{

    //Find a free node id
    while (root.subtree.count(next_node_id)) {
        next_node_id++; 
    }
   const int id = next_node_id++; 

    if (!loaded_synthdefs.count(synthdef)) {
        std::cout << "Loading for the first time" << std::endl;

        std::ifstream def;
        // TODO: @mp2015 to make this address be computer independent
        auto f = "synthdefs\\" + synthdef + ".scsyndef";
        def.open(f);

        if (def) {

            std::string str((std::istreambuf_iterator<char>(def)),
                std::istreambuf_iterator<char>());

            std::array<char, 100> completion_buf {};

            loaded_synthdefs.insert(synthdef);

            SuperColliderPacketBuilder completion { completion_buf.data(), completion_buf.size() };

            completion.s_new(synthdef, id, 0, 0, {});

            auto m = d_recv(str, completion);

            print(m);

            def.close();

        } else {
            std::cout << "Could not open " << f << std::endl;
        }
    } else {
        std::cout << "Already loaded" << std::endl;
        s_new(synthdef, id, 0, 0, {});
    }

    SyncGroup(&root);
     
    return static_cast<Synth*>(root.subtree[id]);
}

Bus SuperColliderController::InstantiateBus()
{
    return Bus(this, next_bus_id++);
}

void SuperColliderController::SyncGroup( Group * g)
{
    auto m = g_queryTree({ { g->index, 1 } });
    std::cout << m.AddressPattern() << std::endl;
    auto it = m.ArgumentsBegin();

    int32_t flag = (it++)->AsInt32();
    int32_t gNodeID = (it++)->AsInt32();

    assert(gNodeID == g->index);

    int32_t children = (it++)->AsInt32();

    for (size_t i = 0; i < children; i++)
    {

        Node* data;

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
                        controls.insert({ cs,Bus(this, std::stoi(it->AsString())) });
                    }
                    it++;
                }
            }

            if (!g->subtree.count(nodeID))
                data = new Synth( nodeID, controls);
        }
        else {
            if (!g->subtree.count(nodeID))
                data = new Group( nodeID);
        }

        g->subtree.insert({ nodeID, data });

        //else, This is another group

    }

}


