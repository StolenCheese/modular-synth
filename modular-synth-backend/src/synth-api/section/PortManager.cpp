//
// Created by bmsyi on 13/02/2023.
//

#include "synth-api/section/_model/PortManager.h"
#include "synth-api/section/Section.h"
#include "sc-controller/SetterFunctor.h"
#include "sc-controller/SuperColliderController.hpp"

#include <unordered_map>
#include <list>
#include <vector>

namespace synth_api {
    std::unordered_map<Port*, Section*> PortManager::parentMap = std::unordered_map<Port*, Section*>();

    InputPort *PortManager::getNewInputPort(Section *parent, SetterFunctor setter, float defaultValue) {
        auto * inp = new InputPort(setter, Rate::control, defaultValue);
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
        std::vector<int> order;

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
                    order.push_back(curr->synth->index);
                    break;
}
        }
        // SCOOP server.pushNodesToStartOfEvalOrder(order); // i.e. the /n_order command with add action 0, use root group
        SuperColliderController::get().n_order(0, 0, order);
    }
}