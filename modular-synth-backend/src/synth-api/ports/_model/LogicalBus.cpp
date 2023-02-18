//
// Created by kofi on 18/02/23.
//

#include "synth-api/ports/_model/LogicalBus.h"
#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"

#include <list>

namespace synth_api {
    void LogicalBus::addListener(InputPort *inputPort) {
        std::list<InputPort *> queue;
        // inputPort should already be disconnected ; front-end logic should enforce this (i.e. can't connect to a new
        // bus whilst we're already connected to one)
        inputPort->logicalBus = this;   // perhaps replace with inputPort.setBus(logicalBus);
        if (inputPort->rate == audio || inputPort->audioRateRequirement) { // perhaps replace with inputPort.getRate()
            queue.insert(queue.begin(), inputPort);
        }

        while (!queue.empty()) {
            InputPort *curr = queue.front();
            queue.pop_front();
            if (curr->logicalBus->addAudioRateRequirement()) {  // if this made the bus audio rate
                curr->logicalBus->propagateChangeForward();
                for (InputPort *symbolic : curr->logicalBus->writer->symbolicLinks) {
                    if (symbolic->rate == dependent) {
                        if (symbolic->addAudioRateRequirement()) { // if this made the InputPort audio rate
                            queue.insert(queue.begin(), symbolic);
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