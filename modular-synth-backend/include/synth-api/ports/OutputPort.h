//
// Created by bmsyi on 09/02/2023.
//

#ifndef MODULAR_SYNTH_OUTPUTPORT_H
#define MODULAR_SYNTH_OUTPUTPORT_H

#include "Port.h"

#include "synth-api/ports/_model/LogicalBus.h"

#include <set>

namespace synth_api {
    class OutputPort : public Port {
        friend class InputPort;
        friend class LogicalBus;
    private:
        std::set<InputPort *> subscribers;
    protected:
        void subscribe(Port *other) override;
        void unsubscribe(Port *other) override;
    public:
        explicit OutputPort() {
            subscribers = std::set<InputPort *>();
            logicalBus = new LogicalBus(this);
        };
    };
}
#endif //MODULAR_SYNTH_OUTPUTPORT_H
