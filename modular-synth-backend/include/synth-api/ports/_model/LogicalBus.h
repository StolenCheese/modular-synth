//
// Created by kofi on 18/02/23.
//

#ifndef MODULAR_SYNTH_LOGICALBUS_H
#define MODULAR_SYNTH_LOGICALBUS_H

#include "Bus.hpp"
#include "ports/InputPort.h"
#include <cstdint>
#include "SuperColliderController.hpp"

namespace synth_api {
class OutputPort;
class LogicalBus  {
private:
    uint16_t audioRateRequirement;
    OutputPort* writer; 
    Bus bus;

    void
    propagateRateChangeForward(); // when the bus's rate changes, tell the writer and all listeners by
                                  // traversing the graph forwards (in the direction of control), starting
                                  // from the writing `OutputPort`

    bool addAudioRateRequirement(); // returns true iff bus switched from control to audio
    bool removeAudioRateRequirement(); // returns true iff bus switched from audio to control

public:
    explicit LogicalBus(SuperColliderController* server, OutputPort* writer)
        : writer(writer)
        , audioRateRequirement(0), bus(server->InstantiateBus()) {
        };

    void addListener(InputPort* inputPort); // connects inputPort to the bus, and iteratively traverses the graph in
                                            // the backwards direction (towards audio sources), updating the rates of
                                            // depended-upon ports and buses

    void removeListener(InputPort* inputPort);
};
}

#endif // MODULAR_SYNTH_LOGICALBUS_H
