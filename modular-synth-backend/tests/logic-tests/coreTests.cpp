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
	Section *sinmod2 = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *sinout = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *noiseout = new Section(synthPath + "pinknoise-ar" + ".scsyndef", synthPath + "pinknoise-kr" + ".scsyndef");
	std::cout << "Created instances of sound sections" << std::endl;

    Section* pan = new Section(synthPath + "pan-ar" + ".scsyndef", synthPath + "pan-kr" + ".scsyndef");
	std::cout << "Created instance of pan section" << std::endl;

    Section* mixer = new Section(synthPath + "mixer-ar" + ".scsyndef", synthPath + "mixer-kr" + ".scsyndef");
	std::cout << "Created instance of mixer section" << std::endl;

	Section *speaker = new Section(synthPath + "speaker-ar" + ".scsyndef", "");
	std::cout << "Created instance of speaker section" << std::endl;

    SuperColliderController::get().g_dumpTree({ {0,1} });

    sinmod->getPortFor("mul")->setDefault(20);
    sinmod->getPortFor("add")->setDefault(440);
    sinmod->getPortFor("freq")->setDefault(2);
	std::cout << "Set params of sinmod" << std::endl;

    sinmod2->getPortFor("mul")->setDefault(1);
    sinmod2->getPortFor("add")->setDefault(0);
    sinmod2->getPortFor("freq")->setDefault(1);
	std::cout << "Set params of sinmod2" << std::endl;

    sinout->getPortFor("freq")->setDefault(330);
	std::cout << "Set params of sinout" << std::endl;

    std::cout << "Linking sinout output to mixer" << std::endl;
    sinout->getPortFor("out")->linkTo(mixer->getPortFor("in1"));

    std::cout << "Linking noiseout output to pan input" << std::endl;
    noiseout->getPortFor("out")->linkTo(pan->getPortFor("in"));

    std::cout << "Linking sinmod2 output to pan control input" << std::endl;
    sinmod2->getPortFor("out")->linkTo(pan->getPortFor("pos"));

    std::cout << "Linking pan output l to speaker input l" << std::endl;
    pan->getPortFor("outl")->linkTo(speaker->getPortFor("inl"));

    std::cout << "Linking pan output r to mixer input" << std::endl;
    pan->getPortFor("outr")->linkTo(mixer->getPortFor("in2"));

    std::cout << "Linking mixer output to speaker input r" << std::endl;
    mixer->getPortFor("out")->linkTo(speaker->getPortFor("inr"));

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