//
// Created by bmsyi on 11/02/2023.
//

#ifndef MODULAR_SYNTH_SECTION_H
#define MODULAR_SYNTH_SECTION_H

#include "../ports/OutputPort.h"
#include "../ports/InputPort.h"

#include <map>
#include <string>
#include <vector>

namespace synth_api {
    class Section {
    private:
        // Mapping of parameter name to corresponding Output that provides it
        // Parameter names kept as a convenience to programmers
        std::map<std::string, OutputPort *> outputPorts;

        // Mapping of parameter name to Input that links to it
        std::map<std::string, InputPort *> inputPorts;

    protected:
        /*
         * (the requirement is on the **CALLER** of this function to ensure the keys of the two maps are **DISJOINT**)
         *
         * Parameters:
         *      std::vector<std::pair<std::string, uint64_t>> inputPortList: Each pair contains the parameter name and default value
         *      std::vector<std::pair<std::string, uint64_t>> outputPortList: Each string is the parameter name for the output port,
         *                   and the uint64_t is the bus number assigned to that output.
         */
        void generatePortModel(std::vector<std::pair<std::string, uint64_t>> inputPortList, std::vector<std::pair<std::string, uint64_t>> outputPortList);

    public:
        /*
         * Returns the port object for a given parameter, either corresponding to an output or an input.
         */
        Port * getPortFor(std::string param);
    };
}

#endif //MODULAR_SYNTH_SECTION_H
