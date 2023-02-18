//
// Created by bmsyi on 09/02/2023.
//

#ifndef MODULAR_SYNTH_OUTPUTPORT_H
#define MODULAR_SYNTH_OUTPUTPORT_H

#include "Port.h"
#include "InputPort.h"

#include <set>

namespace synth_api {
    class OutputPort : public Port {
        friend InputPort;
    private:
        std::set<InputPort *> subscribers;
    protected:
        void subscribe(Port *other) override;
        void unsubscribe(Port *other) override;
    public:
        explicit OutputPort(std::list<Port *>::const_iterator identifier) : Port(identifier) {
            subscribers = std::set<InputPort *>();
        };
    };
}
#endif //MODULAR_SYNTH_OUTPUTPORT_H
