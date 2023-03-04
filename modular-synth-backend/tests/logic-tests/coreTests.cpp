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

	Section *sinmod = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *sinout = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	std::cout << "Created instance of sin sections" << std::endl;

    SuperColliderController::get().g_dumpTree({ {0,1} });

    sinmod->getPortFor("mul")->setDefault(20);
    sinmod->getPortFor("add")->setDefault(440);
    sinmod->getPortFor("freq")->setDefault(2);
	std::cout << "Set params of sinmod" << std::endl;

    // synthetic speaker
    //auto* speaker =  new InputPort(SetterFunctor("",nullptr),synth_api::Rate::audio,0);
    //speaker->logicalBus = new LogicalBus(nullptr);
    //speaker->logicalBus->bus.index = 0;

    sinout->getPortFor("freq")->setDefault(330);
	std::cout << "Set params of sinout" << std::endl;

    std::cout << "Linking sinout output to speaker" << std::endl;
    //sin2->getPortFor("out")->linkTo(speaker);
    sinout->synth->set("out", 0);

    Sleep(1000);

    std::cout << "Setting default value of sinout's freq" << std::endl;
    sinout->getPortFor("freq")->setDefault(550);

    Sleep(1000);

    std::cout << "Linking sinout input to sinmod output" << std::endl;
    sinout->getPortFor("freq")->linkTo(sinmod->getPortFor("out"));

    //std::cout << "Linking sinmod output to sinout input" << std::endl;
    //sinmod->getPortFor("out")->linkTo(sinout->getPortFor("freq"));

    Sleep(1500);

    SuperColliderController::get().g_dumpTree({ {0,1} });

    Sleep(1500);

    //std::cout << "Removing link from sinout input to sinmod output" << std::endl;
    //sinout->getPortFor("freq")->removeLink(sinmod->getPortFor("out"));

    std::cout << "Removing link from sinmod output to sinout input" << std::endl;
    sinmod->getPortFor("out")->removeLink(sinout->getPortFor("freq"));

    SuperColliderController::get().g_dumpTree({ {0,1} });

}