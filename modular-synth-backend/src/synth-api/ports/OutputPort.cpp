//
// Created by bmsyi on 14/02/2023.
//

#include "synth-api/ports/OutputPort.h"
#include "synth-api/exception/LinkException.hpp"

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

    void OutputPort::linkTo(Port *other) {
        Port* otherCopy(other);
        if (dynamic_cast<OutputPort *>(otherCopy) != nullptr) {
            throw OutputToOutputException((char *) "Cannot connect two outputs directly!", *this, *other);
        } else if (dynamic_cast<InputPort *>(other)) {
                other->linkTo(this);
        } else {
            throw FatalOutputControllerException((char *) "Cannot link a port to a null pointer!");
        }
    }

    void OutputPort::removeLink(Port *other) {
        Port* otherCopy(other);
        auto *otherAsInput = dynamic_cast<InputPort *>(otherCopy);
        if (otherAsInput) {
            otherAsInput->removeLink(this);
        }
        else {
            throw FatalOutputControllerException((char *) "Detected an existing OutputPort <--> OutputPort connection. This should have been illegal and it means there is a logical error in backend's code!");
        }
    }

    OutputPort::~OutputPort() {
        for (const auto &p : outgoingConnections) {
            Port::removeLink(p);
        }
        delete logicalBus;
    }
}