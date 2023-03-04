#include "synth-api/section/Section.h"
#include "sc-controller/SuperColliderController.hpp"
#include <iostream>
#include <string>
#include <Windows.h>

#define ADDRESS "127.0.0.1"
#define PORT 58000

using namespace synth_api;

int main(void) {
	std::string synthPath = "S:\\University\\Part-IB\\Group-Project\\modular-synth\\modular-synth-backend\\synthdefs\\";

	std::cout << "Starting" << std::endl;

    auto endpoint = IpEndpointName(ADDRESS, PORT);
      
    SuperColliderController::Connect(endpoint);
    auto& server = SuperColliderController::get();

    server.g_deepFree({ 0 });
    server.dumpOSC(1);

    //Sleep(1000);

	std::cout << "Running" << std::endl;
	Section *sinmod = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *sinout = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	std::cout << "Created instance of sin sections" << std::endl;
    SuperColliderController::get().g_dumpTree({ {0,1} });
    //return 0;

    //sin1->synth->set("mul", 1);
    sinmod->getPortFor("mul")->setDefault(1);
    sinmod->getPortFor("add")->setDefault(0);
    //sin1->synth->set("freq", 1);
    sinmod->getPortFor("freq")->setDefault(1);
	std::cout << "Set params of sin1" << std::endl;

    sinmod->synth->setOutputRate(BusRate::CONTROL);
    sinmod->getPortFor("out")->logicalBus->bus.rate = BusRate::CONTROL;
   
    // synthetic speaker
    auto* speaker =  new InputPort(SetterFunctor("",nullptr),synth_api::Rate::audio,0);
    speaker->logicalBus = new LogicalBus(nullptr);
    speaker->logicalBus->bus.index = 0;

	//std::cout << "Changed out-rate of sin1" << std::endl;
    //auto v = sin1->synth->get("out");
    //Bus newBus = std::get<Bus>(v);
	//std::cout << "Got new control bus" << std::endl;
    sinout->getPortFor("freq")->setDefault(550);
    sinout->getPortFor("mul")->linkTo(sinmod->getPortFor("out"));
    std::cout << "Linking output to speaker" << std::endl;
   // sin2->getPortFor("out")->linkTo(speaker);
    sinout->synth->set("out", 0);

    std::cout << *sinmod->synth << std::endl;
    std::cout << *sinout->synth << std::endl;
    //sin2->synth->set("freq", 550);
    //sin2->synth->set("mul", newBus);
	//std::cout << "Set sin2's mul to read from new bus" << std::endl;
    SuperColliderController::get().g_dumpTree({ {0,1} });
    //while (true) {
        //std::cout << 
    //}

    //Create two sin wave synths
    //auto s1 = server.InstantiateSynth(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");

    //assert(s1->getOutputRate() == BusRate::AUDIO);

    //std::cout << *s1 << std::endl;

    //std::this_thread::sleep_for(1000ms);

    //auto s2 = server.InstantiateSynth(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");

    //assert(s2->getOutputRate() == BusRate::AUDIO);

    //s2->set("freq", 550);

    //std::cout << *s2 << std::endl;

    /*
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
    */

    while (1) {}
}