//
// Created by bmsyi on 11/02/2023.
//

#include "Section.h"

namespace synth_api {
    void Section::generatePortModel(std::vector<std::pair<std::string, uint64_t>> inputPortList, std::vector<std::pair<std::string, uint64_t>> outputPortList) {
        for (auto p : inputPortList) {
            InputPort * newInputPort = new InputPort(0, p.second);
            inputPorts.insert({p.first, newInputPort});
        }
        for (auto p : outputPortList) {
            OutputPort * newOutputPort = new OutputPort(p.second);
            outputPorts.insert({p.first, newOutputPort});
        }
    }

    Port * Section::getPortFor(std::string param) {
        if (inputPorts.find(param) != inputPorts.cend()) {
            return inputPorts.find(param)->second;
        } else if (outputPorts.find(param) != outputPorts.cend()) {
            return outputPorts.find(param)->second;
        }
        return nullptr;
    }
}