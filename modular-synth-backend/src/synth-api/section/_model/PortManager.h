//
// Created by bmsyi on 13/02/2023.
//

#ifndef MODULAR_SYNTH_PORTMANAGER_H
#define MODULAR_SYNTH_PORTMANAGER_H

#include <list>
#include "../../ports/Port.h"
#include "../../ports/InputPort.h"
#include "../../ports/OutputPort.h"

namespace synth_api {
    class PortManager {
    private:
        static std::list<Port *> ports;
    public:
        static InputPort * getNewInputPort();
        static OutputPort * getNewOutputPort();
    };

}

#endif //MODULAR_SYNTH_PORTMANAGER_H
