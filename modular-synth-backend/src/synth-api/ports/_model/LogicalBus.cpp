//
// Created by kofi on 18/02/23.
//

#include "synth-api/ports/_model/LogicalBus.h"
#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"

#include <list>

namespace synth_api {
    void LogicalBus::addListener(InputPort *inputPort) {
        // INVARIANT: inputPort->bus == this
        std::list<InputPort *> queue;
        if (inputPort->rate == audio || inputPort->audioRateRequirement) {
            queue.insert(queue.begin(), inputPort);
        }

        while (!queue.empty()) {
            InputPort *curr = queue.front();
            queue.pop_front();
            if (curr->logicalBus && curr->logicalBus->addAudioRateRequirement()) {  // if this made the bus audio rate
                curr->logicalBus->propagateRateChangeForward();

                for (Port *symbolic : curr->logicalBus->writer->outgoingSymbolicLinks) {
                    if (auto *symbolicInputPort = dynamic_cast<InputPort *>(symbolic)) {

                        if (symbolicInputPort->rate == dependent) {
                            if (symbolicInputPort->addAudioRateRequirement()) { // if this made the InputPort audio rate
                                queue.insert(queue.begin(), symbolicInputPort);
                            }
                        }
                    }
                }
            }
        }
    }

    void LogicalBus::removeListener(InputPort *inputPort) {
        // INVARIANT: inputPort->bus == this
        std::list<InputPort *> queue;
        if (inputPort->rate == audio || inputPort->audioRateRequirement) {
            queue.insert(queue.begin(), inputPort);
        }

        while (!queue.empty()) {
            InputPort *curr = queue.front();
            queue.pop_front();
            if (curr->logicalBus->removeAudioRateRequirement()) {  // if this made the bus control rate
                curr->logicalBus->propagateRateChangeForward(); // !!! will also propagate change to the chain that's leaving!

                for (Port *symbolic : curr->logicalBus->writer->outgoingSymbolicLinks) {
                    if (auto *symbolicInputPort = dynamic_cast<InputPort *>(symbolic)) {

                        if (symbolicInputPort->rate == dependent) {
                            if (symbolicInputPort->removeAudioRateRequirement()) { // if this made the InputPort control rate
                                queue.insert(queue.begin(), symbolicInputPort);
                            }
                        }
                    }
                }
            }
        }
    }

    bool LogicalBus::addAudioRateRequirement() {
        this->audioRateRequirement++;
        if (this->audioRateRequirement == 1) {   // we're switching from control to audio
            //SCOOP free(this.bus) // call the destructor
            //SCOOP this.bus = new Bus(rate=audio)
            propagateRateChangeForward();
            return true;
        }
        return false;
    }

    bool LogicalBus::removeAudioRateRequirement() {
        this->audioRateRequirement--;
        if (this->audioRateRequirement == 0) {   // we're switching from audio to control
            //SCOOP free(this.bus) // call the destructor
            //SCOOP this.bus = new Bus(rate=control)
            propagateRateChangeForward();
            return true;
        }
        return false;
    }

    void LogicalBus::propagateRateChangeForward() {
        // INVARIANT: The graph (viewed as undirected) is acyclic
        //SCOOP this->writer.setOutputBus(this.bus)

        std::list<InputPort *> queue;
        for (InputPort *subscriber : this->writer->subscribers) {
            queue.insert(queue.end(), subscriber);
        }

        while (!queue.empty()) {
            InputPort *curr = queue.front();
            queue.pop_front();
            //SCOOP curr.setInputBus(this.bus)
            for (InputPort *subscriber : curr->subscribers) {
                queue.insert(queue.end(), subscriber);
            }
        }
    }

}