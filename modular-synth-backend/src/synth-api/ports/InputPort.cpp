//
// Created by bmsyi on 09/02/2023.
//

#include "InputPort.h"
#include "OutputPort.h"
#include "../exception/LinkException.hpp"

namespace synth_api {
    void InputPort::linkTo(Port *other) {

        // dynamic cast outputs nullptr when the cast is invalid on a virtual class
        // this only works with *virtual* classes, however!
        auto * outputPort = dynamic_cast<OutputPort *>(other);
        auto * inputPort = dynamic_cast<InputPort *>(other);
        if (outputPort) {
            if (this->controller) {
                throw AlreadyBoundInputException((char *) "Cannot bound input to output - already bound!", *this, *other);
            }
            this->controller = other;
        } else if (inputPort) {
            // both are bound with controllers. It's possible we have an input daisy chain, so
            // we need to scan for this
            if (this->controller && inputPort->controller) {
                // we only have output-to-output if two buses
                // are linked together
                if (this->bus && inputPort->bus) {
                    throw OutputToOutputException((char *) "Binding two inputs linked to outputs", *this, *other);
                } else {
                    if (!this->bus) {
                        // flipping the direction will make this node a
                        // root controller node, and its controller
                        // will become null
                        makeRootController();
                        this->follow(inputPort);
                    } else {
                        // else make the other node a root controller
                        // and then link the other node as its controller
                        inputPort->makeRootController();
                        inputPort->follow(this);
                    }
                }
            } else if (this->controller != nullptr && inputPort->controller == nullptr) {
                inputPort->follow(this);
            } else {
                this->follow(inputPort);
            }
        }
        Port::linkTo(other);
    };
    void InputPort::removeLink(Port *other) {};


    void InputPort::subscribe(InputPort *other) {
        subscribers.insert(other);
    }

    void InputPort::follow(InputPort *other) {
        if (this->controller != nullptr) {
            // we can't replace an OutputPort with another - if we've gotten here
            // then it's a fatal logic error. the logic should never allow this,
            // and this logic is only here as a safety check
            // Speed is not crucial so the safety check is an added safeguard in a
            // project with a tight timeframe
            if (auto * isOutput = dynamic_cast<OutputPort *>(this->controller)) {
                throw FatalOutputControllerException((char *) "Fatal Logic Error: Attempted to follow another port "
                                                              "while controller is an OutputPort!");
            } else if (auto *previous = dynamic_cast<InputPort *>(this->controller)) {
                previous->unsubscribe(previous);
            }
        }
        this->controller = other;
        this->bus = other->bus;
        other->subscribe(this);
    }

    void InputPort::unsubscribe(InputPort *other) {
        auto loc = subscribers.find(other);
        if (loc != subscribers.end()) {
            subscribers.erase(loc);
        }
    }

    void InputPort::makeRootController() {
        InputPort * current = this;
        InputPort * next;
        next = dynamic_cast<InputPort *>(this->controller);
        if (dynamic_cast<OutputPort *>(this->controller)) {
            throw FatalOutputControllerException((char *) "Fatal Logic Error: Attempted to make an "
                                                          "InputPort the root controller in a dependency with an "
                                                          "OutputPort!");
        }

        // can only make the InputPort a root controller when there are no OutputPorts in the dependency graph
        current->controller = nullptr;
        current->bus = 0;
        while (next != nullptr) {
            auto * nextController = next->controller;
            if (dynamic_cast<OutputPort *>(nextController)) {
                throw FatalOutputControllerException((char *) "Fatal Logic Error: Attempted to make an "
                                                              "InputPort the root controller in a dependency with an "
                                                              "OutputPort!");
            }
            next->follow(current);
            current = next;
            next = dynamic_cast<InputPort *>(nextController);
        }
        current->notify();
    }
} // synth_api