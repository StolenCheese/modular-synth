//
// Created by bmsyi on 14/02/2023.
//

#include "synth-api/ports/OutputPort.h"

namespace synth_api {
    void OutputPort::subscribe(synth_api::Port *other) {
        if (auto *otherAsInput = dynamic_cast<InputPort *>(other)) {
            subscribers.insert(otherAsInput);
        }
    }

    void OutputPort::unsubscribe(synth_api::Port *other) {
        auto *otherAsInput = dynamic_cast<InputPort *>(other);
        auto loc = subscribers.find(otherAsInput);
        if (loc != subscribers.cend()) {
            subscribers.erase(otherAsInput);
        }
    }
}