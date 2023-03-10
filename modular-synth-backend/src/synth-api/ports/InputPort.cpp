//
// Created by bmsyi on 09/02/2023.
//

#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"
#include "synth-api/ports/Port.h"
#include "synth-api/section/_model/PortManager.h"

#include "synth-api/exception/LinkException.hpp"

#include <list>

namespace synth_api {
    void InputPort::linkTo(Port *other) {
        // IMPORTANT NOTE: Our _model stores links with *directionality*
        // So the DIRECTION of linkage (this->Port::linkTo(other) and other->Port::linkTo(this)) is important!

        // Before we mess with controllers, we check for cycles.
        Port::cyclicCheck(other);

        // dynamic cast outputs nullptr when the cast is invalid on a virtual class
        // this only works with *virtual* classes, however!
        auto * otherCopy(other);
        auto * outputPort = dynamic_cast<OutputPort *>(otherCopy);
        auto * inputPort = dynamic_cast<InputPort *>(other);
        if (outputPort) {
            if (this->controller) {
                if (!this->logicalBus) {
                    makeRootController();
                }
                else {
                    throw AlreadyBoundInputException((char*)"Cannot bound input to output - already bound!", *this, *outputPort);
                }
            }
			follow(outputPort);
            this->Port::linkTo(outputPort);
			PortManager::reorder(outputPort);
        } else if (inputPort) {
            // both are bound with controllers. It's possible we have an input daisy chain, so
            // we need to scan for this
            if (this->controller && inputPort->controller) {
                // we only have output-to-output if two buses
                // are linked together
                if (this->logicalBus && inputPort->logicalBus) {
                    throw OutputToOutputException((char *) "Binding two inputs linked to outputs", *this, *inputPort);
                } else {
                    if (!this->logicalBus) {
                        // flipping the direction will make this node a
                        // root controller node, and its controller
                        // will become null
                        makeRootController();
                        this->follow(inputPort);
                        this->Port::linkTo(inputPort);
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
                this->Port::linkTo(inputPort);
            }
        }
    }

    void InputPort::removeLink(Port *other) {
        auto* otherCopy(other);
        auto * otherAsInputPort = dynamic_cast<InputPort *>(otherCopy);
        // unsubscribe, then disconnect, so as not to get irrelevant bus rate change information whilst disconnecting
        if (this->controller == other) {
            if (otherAsInputPort) {
                otherAsInputPort->unsubscribe(this);
            }
            this->controller = nullptr;
            if (this->logicalBus) {
                disconnectFromBus();
                this->notify();
            }
        } else if (otherAsInputPort && otherAsInputPort->controller == this) {
            otherAsInputPort->controller = nullptr;
            this->unsubscribe(otherAsInputPort);
            if (otherAsInputPort->logicalBus) {
                otherAsInputPort->disconnectFromBus();
                otherAsInputPort->notify();
            }
        }
        Port::removeLink(other);
    }

    void InputPort::follow(Port *other) {
        auto* thisControllerCopy1(this->controller);
        auto* thisControllerCopy2(this->controller);
        if (this->controller != nullptr) {
            if (auto * previousInput = dynamic_cast<InputPort *>(thisControllerCopy1)) {
                previousInput->unsubscribe(this);
            } else if (auto * previousOutput = dynamic_cast<OutputPort *>(thisControllerCopy2)) {
                previousOutput->unsubscribe(this);
            }
        }
        this->controller = other;
        auto* otherCopy1(other);
        auto* otherCopy2(other);
        // connect, then subscribe, so as not to get bus rate information twice
        if (auto * otherAsInput = dynamic_cast<InputPort *>(otherCopy1)) {
            connectToBus(otherAsInput->logicalBus);
            otherAsInput->subscribe(this);
        } else if (auto * otherAsOutput = dynamic_cast<OutputPort *>(otherCopy2)) {
            connectToBus(otherAsOutput->logicalBus);
        }
        notify();
    }

    void InputPort::subscribe(Port *other) {
        auto* otherCopy(other);
        auto *otherAsInputPort = dynamic_cast<InputPort *>(otherCopy);
        if (otherAsInputPort) {
            subscribers.insert(otherAsInputPort);
        }
    }

    void InputPort::unsubscribe(Port *other) {
        auto* otherCopy(other);
        auto *otherAsInputPort = dynamic_cast<InputPort *>(otherCopy);
        auto loc = subscribers.find(otherAsInputPort);
        if (loc != subscribers.end()) {
            subscribers.erase(loc);
        }
    }

    void InputPort::makeRootController() {
        InputPort * current = this;
        InputPort * next;
        auto* thisController(this->controller);
        auto* thisControllerCopy(this->controller);
        next = dynamic_cast<InputPort *>(thisController);
        if (dynamic_cast<OutputPort *>(thisControllerCopy)) {
            throw FatalOutputControllerException((char *) "Fatal Logic Error: Attempted to make an "
                                                          "InputPort the root controller in a dependency with an "
                                                          "OutputPort!");
        }

        // can only make the InputPort a root controller when there are no OutputPorts in the dependency graph
        if (next) {
            next->unsubscribe(this);
        }
        current->controller = nullptr;
        while (next != nullptr) {
            auto * nextController(next->controller);
            auto* nextControllerCopy(next->controller);
            if (dynamic_cast<OutputPort *>(nextControllerCopy)) {
                throw FatalOutputControllerException((char *) "Fatal Logic Error: Attempted to make an "
                                                              "InputPort the root controller in a dependency with an "
                                                              "OutputPort!"); 
            }

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
                if (curr->logicalBus) {
                    subscriber->connectToBus(logicalBus);
                } else {
                    subscriber->disconnectFromBus();
                }
                queue.insert(queue.end(), subscriber);
            }
        }
    }

    void InputPort::setDefault(float value) {
        defaultValue = value;
        if (logicalBus == nullptr) setParam(value);
    }

    bool InputPort::addAudioRateRequirement() {
        this->audioRateRequirement++;
        if (this->audioRateRequirement == 1) {   // we're switching from control to audio
            return true;
        }
        return false;
    }

    bool InputPort::removeAudioRateRequirement() {
        this->audioRateRequirement--;
        if (this->audioRateRequirement == 0) {   // we're switching from audio to control
            return true;
        }
        return false;
    }

    void InputPort::connectToBus(LogicalBus* logicalBus) {
        this->logicalBus = logicalBus;
        if (logicalBus) {
            logicalBus->addListener(this);
        }
    }

    void InputPort::disconnectFromBus() {
        if (this->logicalBus) {
            this->logicalBus->removeListener(this);
        }
        this->logicalBus = nullptr;
    }

    void InputPort::clearConnections() {
        if (!outgoingConnections.size()) {
            return;
        }
        std::vector<Port*> toRemove;
        for (const auto& p : outgoingConnections) {
            toRemove.push_back(p);
        }
        for (const auto& p : toRemove) {
            removeLink(p);     
        }
    }

    InputPort::~InputPort() {
        clearConnections();
    }
} // synth-api