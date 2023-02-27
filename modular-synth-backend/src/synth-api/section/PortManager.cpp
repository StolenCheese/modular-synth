//
// Created by bmsyi on 13/02/2023.
//

#include "synth-api/section/_model/PortManager.h"
#include "synth-api/section/Section.h"

#include <unordered_map>
#include <list>

namespace synth_api {
    std::unordered_map<Port*, Section*> PortManager::parentMap = std::unordered_map<Port*, Section*>();

    InputPort *PortManager::getNewInputPort(Section *parent, uint64_t defaultValue) {
        auto * inp = new InputPort(defaultValue, Rate::control);
        parentMap[inp] = parent;
        return inp;
    }

    OutputPort *PortManager::getNewOutputPort(Section *parent) {
        auto * out = new OutputPort();
        parentMap[out] = parent;
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