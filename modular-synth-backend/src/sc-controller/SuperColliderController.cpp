#include "SuperColliderController.hpp"
#include <fstream>
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

std::future<Synth> SuperColliderController::InstantiateSynth(const std::string& synthdef)
{
    if (!loaded_synthdefs.contains(synthdef)) {
        std::cout << "Loading for the first time" << std::endl;

        std::ifstream def;
        auto f = "A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\" + synthdef + ".scsyndef";
        def.open(f);

        if (def) {

            std::string str((std::istreambuf_iterator<char>(def)),
                std::istreambuf_iterator<char>());

            std::array<char, 100> completion_buf {};

            loaded_synthdefs.insert(synthdef);

            SuperColliderPacketBuilder completion { completion_buf.data(), completion_buf.size() };

            completion.s_new(synthdef, next_node_id, 0, 0, {});

            auto m = d_recv(str, completion).get();

            print(m);

            def.close();

        } else {
            std::cout << "Could not open " << f << std::endl;
        }
    } else {
        std::cout << "Already loaded" << std::endl;
        s_new(synthdef, next_node_id, 0, 0, {});
    }

    SyncTrees();

    std::promise<Synth> tmpPromise; // default construct promise

    std::future<Synth> tmp = tmpPromise.get_future(); // get future associated with promise

    tmpPromise.set_value(Synth(this, next_node_id, std::set<std::string>())); // set future value

    next_node_id++;
    return tmp;
}

Bus SuperColliderController::InstantiateBus()
{
    return Bus(this, next_bus_id++);
}

void SuperColliderController::SyncTrees()
{
    std::cout << "Syncing" << std::endl;
    auto m = g_queryTree({ { 0, 1 } }).get();

    print(m);
}
