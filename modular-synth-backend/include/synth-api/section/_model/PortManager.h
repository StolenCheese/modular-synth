//
// Created by bmsyi on 13/02/2023.
//

#ifndef MODULAR_SYNTH_PORTMANAGER_H
#define MODULAR_SYNTH_PORTMANAGER_H

#include "../../ports/InputPort.h"
#include "../../ports/OutputPort.h"
#include "../../ports/Port.h"
#include <list>

namespace synth_api {
class PortManager {
private:
    static std::list<Port*> ports;

public:
    Port* in;

    Port* out;

    // TODO @bms53: Make these thread safe with locks!
    static InputPort* getNewInputPort(uint64_t defaultValue);
    static OutputPort* getNewOutputPort(uint64_t defaultBus);

    // TODO @ksw40: Add single-pass "toposort" logic
};

}

#endif // MODULAR_SYNTH_PORTMANAGER_H
