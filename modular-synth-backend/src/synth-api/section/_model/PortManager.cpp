//
// Created by bmsyi on 13/02/2023.
//

#include "synth-api/section/_model/PortManager.h"

#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"
#include "synth-api/section/Section.h"

#include <unordered_map>
#include <list>

namespace synth_api {
    InputPort *PortManager::getNewInputPort(uint64_t defaultValue) {
        auto * inp = new InputPort(defaultValue, control, ports.cend());
        ports.insert(ports.cend(), inp);
        return inp;
    }

    OutputPort *PortManager::getNewOutputPort(uint64_t defaultBus) {
        auto * out = new OutputPort(ports.cend());
        ports.insert(ports.cend(), out);
        return out;
    }

    void PortManager::reorder(OutputPort *root) {
        std::unordered_map<Section *, Stage> stages;
        std::list<Section *> stack;
        std::list<Section *> order;

        Section *parent = parentMap[root];
        stack.insert(stack.begin(), parent);
        stages[parent] = OnStack;

        while (!stack.empty()) {
            Section *curr = stack.back();
            switch (stages[curr]) {
                case OnStack:   // put all unstacked neighbours on stack
                    for (const auto& pair : curr->inputPorts) {
                        InputPort *inputPort = pair.second;
                        if (inputPort->logicalBus) {
                            Section *next = parentMap[inputPort->logicalBus->writer];
                            if (stages.find(next) == stages.end()) {    // if unstacked
                                stack.insert(stack.end(), next);
                                stages[next] = OnStack;
                            }
                        }
                    }
                    stages[curr] = Explored;
                    break;

                case Explored:  // add to order
                    stack.pop_back();
                    order.insert(order.end(), curr);
                    break;
}
        }
        // SCOOP server.pushNodesToStartOfEvalOrder(order); // i.e. the /n_order command with add action 0, use root group
    }
}