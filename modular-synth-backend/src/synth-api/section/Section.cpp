//
// Created by bmsyi on 11/02/2023.
//

#include "section/Section.h"
#include "section/_model/PortManager.h"

namespace synth_api {
    
    void Section::generatePortModel(const std::vector<std::pair<std::string, uint64_t>>& inputPortList, const std::vector<std::pair<std::string, uint64_t>>& outputPortList) {

        for (const auto& p : inputPortList) {
            InputPort * newInputPort = PortManager::getNewInputPort(p.second);
            inputPorts.insert({p.first, newInputPort});
        }

        for (const auto& p : outputPortList) {
            OutputPort * newOutputPort = PortManager::getNewOutputPort(p.second);
            outputPorts.insert({p.first, newOutputPort});
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

    Section::Section(SuperColliderController* s, char *synthdef) : synth(s->InstantiateSynth(std::string(synthdef))) {
        //TODO: this whole constructor should be private, and objects created in async by a factory, in the same way as synths are created async here
        

        std::vector<std::pair<std::string, uint64_t>> inputPortList = {{"pitch", 10}, {"amplification", 20}, {"frequency", 300}};
        std::vector<std::pair<std::string, uint64_t>> outputPortList = {{"reverb", 1}, {"tonality", 2}};
        generatePortModel(inputPortList, outputPortList);
    }
}