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

    Section::Section(char *synthDef, const std::vector<std::string>& inputParams,
                     const std::vector<std::string>& outputParams
                     ) : synth(SuperColliderController::get().InstantiateSynth(std::string(synthDef))) {

        std::vector<std::pair<std::string, uint64_t>> inputPortSpecification;

        for (const std::string& param : inputParams) {
            inputPortSpecification.insert(inputPortSpecification.cend(), {param, 0});
        }

        generatePortModel(*this, inputPortSpecification, outputParams);
    }
}