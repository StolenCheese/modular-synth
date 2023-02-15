//
// Created by bmsyi on 13/02/2023.
//

#include "synth-api/section/_model/PortManager.h"

#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"

namespace synth_api {
    InputPort *PortManager::getNewInputPort(uint64_t defaultValue) {
        auto * inp = new InputPort(0, defaultValue, ports.cend());
        ports.insert(ports.cend(), inp);
        return inp;
    }

    OutputPort *PortManager::getNewOutputPort(uint64_t defaultBus) {
        auto * out = new OutputPort(defaultBus, ports.cend());
        ports.insert(ports.cend(), out);
        return out;
    }
}