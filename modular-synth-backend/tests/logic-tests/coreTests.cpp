#include "synth-api/section/Section.h"
#include "sc-controller/SuperColliderController.hpp"
#include <iostream>
#include <string>
#include <Windows.h>

#define ADDRESS "127.0.0.1"
#define PORT 58000


using namespace synth_api;

void panTest(std::string  synthPath) {
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

void daisyChainTest(std::string synthPath) {
	Section *sinmod = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *sina = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *sinb = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *sinc = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	Section *sind = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	std::cout << "Created instances of sound sections" << std::endl;

    Section* mixer = new Section(synthPath + "mixer-ar" + ".scsyndef", synthPath + "mixer-kr" + ".scsyndef");
	std::cout << "Created instance of mixer section" << std::endl;

	Section *speaker = new Section(synthPath + "speaker-ar" + ".scsyndef", "");
	std::cout << "Created instance of speaker section" << std::endl;

    SuperColliderController::get().g_dumpTree({ {0,1} });

    sina->getPortFor("freq")->setDefault(220);
    sina->getPortFor("mul")->setDefault(0.2);
    sinb->getPortFor("freq")->setDefault(330);
    sinb->getPortFor("mul")->setDefault(0.2);
    sinc->getPortFor("freq")->setDefault(440);
    sinc->getPortFor("mul")->setDefault(0.2);
    sind->getPortFor("freq")->setDefault(550);
    sind->getPortFor("mul")->setDefault(0.2);
	std::cout << "Set params of sins" << std::endl;

    sinmod->getPortFor("mul")->setDefault(0.25);
    sinmod->getPortFor("add")->setDefault(0);
    sinmod->getPortFor("freq")->setDefault(2);
	std::cout << "Set params of sinmod" << std::endl;

    sina->getPortFor("mul")->linkTo(sinb->getPortFor("mul"));
    //sinb->getPortFor("mul")->linkTo(sina->getPortFor("mul"));
    sinb->getPortFor("mul")->linkTo(sinc->getPortFor("mul"));
    sinc->getPortFor("mul")->linkTo(sind->getPortFor("mul"));
	std::cout << "Daisy-chained sins' muls together" << std::endl;

    sind->getPortFor("mul")->linkTo(sinmod->getPortFor("out"));
	std::cout << "Connected sinmod to a mul" << std::endl;

    sina->getPortFor("out")->linkTo(mixer->getPortFor("in1"));
    sinb->getPortFor("out")->linkTo(mixer->getPortFor("in2"));
    sinc->getPortFor("out")->linkTo(mixer->getPortFor("in3"));
    sind->getPortFor("out")->linkTo(mixer->getPortFor("in4"));
	std::cout << "Connected sins to mixer" << std::endl;

    speaker->getPortFor("inl")->linkTo(mixer->getPortFor("out"));
    speaker->getPortFor("inr")->linkTo(mixer->getPortFor("out"));
	std::cout << "Connected speaker to mixer" << std::endl;

    SuperColliderController::get().g_dumpTree({ {0,1} });
}

void nullTest(std::string synthPath) {
	Section *sinout = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	std::cout << "Created instance of sound section" << std::endl;

	//Section *speaker = new Section(synthPath + "speaker-ar" + ".scsyndef", "");
	//std::cout << "Created instance of speaker section" << std::endl;

	//Section *decay = new Section(synthPath + "decay2-ar" + ".scsyndef", "");
	//std::cout << "Created instance of decay2 section" << std::endl;

    Section* pan = new Section(synthPath + "pan-ar" + ".scsyndef", synthPath + "pan-kr" + ".scsyndef");
	std::cout << "Created instance of pan section" << std::endl;

    //pan->getPortFor("outl")->linkTo(decay->getPortFor("ain"));
    //decay->getPortFor("ain")->linkTo(pan->getPortFor("outl"));

    //pan->getPortFor("outl")->linkTo(sinout->getPortFor("freq"));
    sinout->getPortFor("freq")->linkTo(pan->getPortFor("outl"));
	std::cout << "Made the fatal link!" << std::endl;

    SuperColliderController::get().g_dumpTree({ {0,1} });
}

void audioInputTest(std::string synthPath) {
	Section *sinmod = new Section(synthPath + "sin-ar" + ".scsyndef", synthPath + "sin-kr" + ".scsyndef");
	std::cout << "Created instance of sinmod section" << std::endl;

	Section *speaker = new Section(synthPath + "speaker-ar" + ".scsyndef", "");
	std::cout << "Created instance of speaker section" << std::endl;

	Section *decay = new Section(synthPath + "decay2-ar" + ".scsyndef", "");
	std::cout << "Created instance of decay2 section" << std::endl;

    Section* pan = new Section(synthPath + "pan-ar" + ".scsyndef", synthPath + "pan-kr" + ".scsyndef");
	std::cout << "Created instance of pan section" << std::endl;

    Section* mic = new Section(synthPath + "mic-ar" + ".scsyndef", synthPath + "mic-ar" + ".scsyndef");
	std::cout << "Created instance of mic section" << std::endl;

    sinmod->getPortFor("freq")->setDefault(1);

    decay->getPortFor("attackTime")->setDefault(0);
    decay->getPortFor("decayTime")->setDefault(5);

    mic->getPortFor("outl")->linkTo(decay->getPortFor("in"));

    pan->getPortFor("in")->linkTo(decay->getPortFor("out"));

    pan->getPortFor("pos")->linkTo(sinmod->getPortFor("out"));

    speaker->getPortFor("inl")->linkTo(pan->getPortFor("outl"));
    pan->getPortFor("outr")->linkTo(speaker->getPortFor("inr"));

    SuperColliderController::get().g_dumpTree({ {0,1} });
}

int main(void) {
    std::string synthPath = "S:\\University\\Part-IB\\Group-Project\\modular-synth\\modular-synth-backend\\synthdefs\\";

	std::cout << "Starting" << std::endl;

    auto endpoint = IpEndpointName(ADDRESS, PORT);
      
    SuperColliderController::Connect(endpoint);
    auto& server = SuperColliderController::get();

    server.g_deepFree({ 0 });
    server.dumpOSC(1);

    //panTest(synthPath);
    //daisyChainTest(synthPath);
    //nullTest(synthPath);
    audioInputTest(synthPath);
}