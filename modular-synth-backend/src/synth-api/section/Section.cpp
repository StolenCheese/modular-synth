//
// Created by bmsyi on 11/02/2023.
//

#include "synth-api/section/Section.h"
#include "synth-api/section/_model/PortManager.h"

namespace synth_api {
    
    void Section::generatePortModel(Section &parent,
                                    const std::vector<std::pair<std::string, uint64_t>>& inputPortList,
                                    const std::vector<std::string>& outputPortList) {

        for (const auto& p : inputPortList) {
            InputPort * newInputPort = PortManager::getNewInputPort(&parent, p.second);
            inputPorts.insert({p.first, newInputPort});
        }

        for (const auto& p : outputPortList) {
            OutputPort * newOutputPort = PortManager::getNewOutputPort(&parent);
            outputPorts.insert({p, newOutputPort});
        }

        for (const auto& outputPair : outputPorts) {
            for (const auto& inputPair : inputPorts) {
                inputPair.second->symbolicLinkTo(outputPair.second);
                outputPair.second->symbolicLinkTo(inputPair.second);
            }
        }
    }

    Port * Section::getPortFor(const std::string& param) {
        if (inputPorts.find(param) != inputPorts.cend()) {
            return inputPorts.find(param)->second;
        } else if (outputPorts.find(param) != outputPorts.cend()) {
            return outputPorts.find(param)->second;
        }
        return nullptr;
    }

    Section::Section(const char *synthDef) : synth(SuperColliderController::get().InstantiateSynth(std::string(synthDef))) {

        std::vector<std::pair<std::string, uint64_t>> inputPortSpecification;
        std::vector<std::string> outputPortSpecification;

        auto size = synth->controls.size();

        for (const auto& key_value_pair : synth->controls) {
            if (key_value_pair.first.rfind("out", 0) == 0) {
                outputPortSpecification.insert(outputPortSpecification.cend(), key_value_pair.first.c_str());
            }
            else {
                inputPortSpecification.insert(inputPortSpecification.cend(),
                                              {key_value_pair.first.c_str(), std::get<int>(key_value_pair.second)});
            }
        }

        generatePortModel(*this, inputPortSpecification, outputPortSpecification);
    }
}