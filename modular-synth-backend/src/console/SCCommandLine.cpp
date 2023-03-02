#include "sc-controller/SuperColliderController.hpp"

#include "osc/OscOutboundPacketStream.h"
#include "osc/OscReceivedElements.h"
#include "osc/OscPacketListener.h"
#include "ip/UdpSocket.h"

#include <iostream>
#include <WinSock2.h>
#include <vector>
#include <utility>
#include <variant>

#define ADDRESS "127.0.0.1"
#define PORT 58000

#define OUTPUT_BUFFER_SIZE 1024

void midi_test() {
    auto& server = SuperColliderController::get();

    auto s3 = server.InstantiateMidiSynth("A:\\Documents\\synth\\midi\\Bonetrousle.mid");


    for (size_t i = 0; i < 16; i++)
    {
        // two organs per channel
        auto s1 = server.InstantiateSynth("A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\organ.scsyndef", "");

        auto b1 = s3->get("out" + std::to_string(i) + "_0");
        s1->set("freq", std::get<Bus>(b1));
        std::get<Bus>(b1).set(0);
       
        auto s2 = server.InstantiateSynth("A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\organ.scsyndef", "");

        auto b2 = s3->get("out" + std::to_string(i) + "_1");
        s2->set("freq", std::get<Bus>(b2));
        std::get<Bus>(b2).set(0);
    }



    std::string s;
    std::cin >> s;

    delete s3; 
}

void osc_test() {

    auto& server = SuperColliderController::get();

    server.g_deepFree({ 0 });

    //Create two sin wave synths
    auto s1 = server.InstantiateSynth("A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\sin-ar.scsyndef", "A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\sin-kr.scsyndef");


    assert(s1->getOutputRate() == BusRate::AUDIO);

    std::cout << *s1 << std::endl;

    auto s2 = server.InstantiateSynth("A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\sin-ar.scsyndef", "A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\sin-kr.scsyndef");

    assert(s2->getOutputRate() == BusRate::AUDIO);

    std::cout << *s2 << std::endl;

    auto b = server.InstantiateBus();

    std::cout << "Linking synths:" << std::endl;
    s1->set("out", b.index);
    b.rate = BusRate::AUDIO;

    s1->set("add", 440);
    s1->set("mul", 100);
    s1->set("freq", 1);

    s2->set("freq", b);

    using namespace std::chrono_literals;

    std::this_thread::sleep_for(1000ms);

   // At this point both are audio - convert one to control
  
    s1->setOutputRate(BusRate::CONTROL);

    assert(s1->getOutputRate() == BusRate::CONTROL);

    b.rate = BusRate::CONTROL;

    s2->set("freq", b);


    std::cout << *s1 << std::endl;
    std::cout << *s2 << std::endl;

    std::string s;
    std::cin >> s;



    delete s1;
    delete s2;

}


int main(int argc, char* argv[])
{

    // https://doc.sccode.org/Reference/Server-Command-Reference.html
    // cd "C:\Program Files\SuperCollider-3.13.0-rc1"; ./scsynth.exe -u 58000

    std::variant<int, float, std::string> intFloatString;

    (void)argc; // suppress unused parameter warnings
    (void)argv; // suppress unused parameter warnings

    auto endpoint = IpEndpointName(ADDRESS, PORT);
      
    SuperColliderController::Connect(endpoint);
    auto& server = SuperColliderController::get();
    // transmitSocket.Bind(IpEndpointName(IpEndpointName::ANY_ADDRESS, PORT + 1));

    std::cout << "Transmitting test packets" << std::endl;
     
    server.dumpOSC(1);
    // reset everything
    server.g_deepFree({ 0 });

    server.root.syncTree();

    std::cout << "Version:" << std::endl;
    std::cout << server.version().AddressPattern() << std::endl;



    //midi_test();
    osc_test();
     
}



