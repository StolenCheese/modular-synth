//
// Created by bmsyi on 09/02/2023.
//

#include "InputPort.h"
#include "OutputPort.h"
#include "../exception/LinkException.hpp"

#include <list>

namespace synth_api {
    void InputPort::linkTo(Port *other) {
        // IMPORTANT NOTE: Our model stores links with *directionality*
        // So the DIRECTION of linkage (this->Port::linkTo(other) and other->Port::linkTo(this)) is important!

        // Before we mess with controllers, we check for cycles.
        Port::cyclicCheck(other);

        // dynamic cast outputs nullptr when the cast is invalid on a virtual class
        // this only works with *virtual* classes, however!
        auto * outputPort = dynamic_cast<OutputPort *>(other);
        auto * inputPort = dynamic_cast<InputPort *>(other);
        if (outputPort) {
            if (this->controller) {
                throw AlreadyBoundInputException((char *) "Cannot bound input to output - already bound!", *this, *other);
            }
            this->controller = other;
            other->Port::linkTo(this);
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
                        inputPort->Port::linkTo(this);
                    } else {
                        // else make the other node a root controller
                        // and then link the other node as its controller
                        inputPort->makeRootController();
                        inputPort->follow(this);
                        this->Port::linkTo(inputPort);
                    }
                }
            } else if (this->controller != nullptr && inputPort->controller == nullptr) {
                inputPort->follow(this);
                this->Port::linkTo(inputPort);
            } else {
                this->follow(inputPort);
                inputPort->Port::linkTo(this);
            }
        }
    };

    void InputPort::removeLink(Port *other) {
        auto * otherAsInputPort = dynamic_cast<InputPort *>(other);
        if (this->controller == other) {
            if (otherAsInputPort) {
                otherAsInputPort->unsubscribe(this);
            }
            this->controller = nullptr;
        } else if (otherAsInputPort->controller == this) {
            otherAsInputPort->controller = nullptr;
            this->unsubscribe(otherAsInputPort);
        }
        Port::removeLink(other);
    };

    void InputPort::follow(InputPort *other) {
        if (this->controller != nullptr) {
            // we can't replace an OutputPort with another - if we've gotten here
            // then it's a fatal logic error. the logic should never allow this,
            // and this logic is only here as a safety check
            // Speed is not crucial so the safety check is an added safeguard in a
            // project with a tight timeframe
            if (dynamic_cast<OutputPort *>(this->controller) != nullptr) {
                throw FatalOutputControllerException((char *) "Fatal Logic Error: Attempted to follow another port "
                                                              "while controller is an OutputPort!");
            } else if (auto *previous = dynamic_cast<InputPort *>(this->controller)) {
                previous->unsubscribe(previous);
            }
        }
        this->controller = other;
        {
            this->bus = other->bus;
        }
        other->subscribe(this);
        notify();
    }

    void InputPort::subscribe(InputPort *other) {
        subscribers.insert(other);
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
            // reverse direction of linkage
            next->Port::removeLink(current);
            current->Port::linkTo(next);

            // reverse direction of controller
            next->follow(current);
            current = next;
            next = dynamic_cast<InputPort *>(nextController);
        }
        current->notify();
    }

    void InputPort::notify() {
        // INVARIANT: The graph (viewed as undirected) is acyclic
        std::list<InputPort *> queue;
        queue.insert(queue.begin(), this);
        while (!queue.empty()) {
            InputPort * curr = queue.front();
            queue.pop_front();
            for (InputPort * subscriber : curr->subscribers) {
                subscriber->bus = curr->bus;
                queue.insert(queue.end(), subscriber);
            }
        }
    }

    void InputPort::setDefault(uint64_t value) {
        defaultValue = value;
    }
} // synth_api