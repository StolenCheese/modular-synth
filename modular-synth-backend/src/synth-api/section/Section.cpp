//
// Created by bmsyi on 11/02/2023.
//

#include "synth-api/section/Section.h"
#include "synth-api/section/_model/PortManager.h"
#include "sc-controller/SetterFunctor.h"

namespace synth_api {

    void Section::generatePortModel(Section &parent,
                                    const std::vector<std::pair<std::string, float>>& inputPortList,
                                    const std::vector<std::string>& outputPortList) {

        for (const auto& p : inputPortList) {
            SetterFunctor setter = SetterFunctor(p.first, synth);
            InputPort * newInputPort = PortManager::getNewInputPort(&parent, setter, p.second);
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

    Section::Section(std::string& synthDef) : synth(SuperColliderController::get().InstantiateSynth(synthDef)), outputPorts(), inputPorts() {

        std::vector<std::pair<std::string, float>> inputPortSpecification;
        std::vector<std::string> outputPortSpecification;

        for (const auto& key_value_pair : synth->controls) {
            if (key_value_pair.first.rfind("out", 0) == 0) {
                outputPortSpecification.insert(outputPortSpecification.cend(), key_value_pair.first.c_str());
            }
            else {
                inputPortSpecification.insert(inputPortSpecification.cend(),
                                              {key_value_pair.first.c_str(), std::get<float>(key_value_pair.second)});
            }
        }

        generatePortModel(*this, inputPortSpecification, outputPortSpecification);
    }

    Section::~Section() {
        delete synth;
        for (const auto& p : outputPorts) {
            delete p.second;
        }
        for (const auto& p : inputPorts) {
            delete p.second;
        }
    }
}