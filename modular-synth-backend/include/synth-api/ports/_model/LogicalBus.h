//
// Created by kofi on 18/02/23.
//

#ifndef MODULAR_SYNTH_LOGICALBUS_H
#define MODULAR_SYNTH_LOGICALBUS_H

#include "synth-api/ports/OutputPort.h"
#include "synth-api/ports/InputPort.h"

#include <cstdint>

namespace synth_api {
    class LogicalBus {
    private:
        uint16_t audioRateRequirement;
        OutputPort *writer;
        //SCOOP Bus *bus;

        void propagateRateChangeForward();  // when the bus's rate changes, tell the writer and all listeners by
                                            // traversing the graph forwards (in the direction of control), starting
                                            // from the writing `OutputPort`

    public:
        explicit LogicalBus(OutputPort *writer) : writer(writer) {
            //SCOOP bus = new Bus(rate=control)
        };

        void connect(InputPort *inputPort);  // connects inputPort to the bus, and iteratively traverses the graph in
                                            // the backwards direction (towards audio sources), updating the rates of
                                            // depended-upon ports and buses

        bool addAudioRateRequirement();     // returns true iff bus switched from control to audio
        bool removeAudioRateRequirement();  // returns true iff bus switched from audio to control
    };
}

#endif //MODULAR_SYNTH_LOGICALBUS_H
