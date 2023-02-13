//
// Created by bmsyi on 09/02/2023.
//

#ifndef MODULAR_SYNTH_OUTPUTPORT_H
#define MODULAR_SYNTH_OUTPUTPORT_H

#include "Port.h"

namespace synth_api {
    class OutputPort : public Port {
    public:
        explicit OutputPort(uint64_t bus) : Port(bus) { };
    };
}
#endif //MODULAR_SYNTH_OUTPUTPORT_H
