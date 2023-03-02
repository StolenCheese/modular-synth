//
// Created by bmsyi on 11/02/2023.
//

#ifndef MODULAR_SYNTH_SECTION_H
#define MODULAR_SYNTH_SECTION_H

#include "sc-controller/Synth.hpp"
#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"
#include "synth-api/section/_model/PortManager.h"

#include <map>
#include <string>
#include <vector>

namespace synth_api {

/*
 * Use this to _model a section on a modular synth.
 * You can query for specific ports via parameters, which will be defined in section definition file
 * (TODO @mp2015: spread knowledge of UI file so we're on the same page about what is actually in it as I'm guessing here)
 */
class Section {
    friend class PortManager;

protected:
    /*
     * (the requirement is on the **CALLER** of this function to ensure the keys of the two maps are **DISJOINT**)
     *
     * Parameters:
     *      std::vector<std::pair<std::string, uint64_t>> inputPortList: Each pair contains the parameter name and default value
     *      std::vector<std::pair<std::string, uint64_t>> outputPortList: Each string is the parameter name for the output port,
     *                   and the uint64_t is the bus number assigned to that output.
     */
    void generatePortModel(Section& parent,
        const std::vector<std::pair<std::string, float>>& inputPortList,
        const std::vector<std::string>& outputPortList);

public:
    Synth* synth;

    // Mapping of parameter name to corresponding Output that provides it
    // Parameter names kept as a convenience to programmers
    std::map<std::string, OutputPort*> outputPorts;

    // Mapping of parameter name to Input that links to it
    std::map<std::string, InputPort*> inputPorts;

    /*
     * Generates a Section object from a given section definition file.
     *
     * Parameters:
     *      std::string& synthdef: known/dir/to/synthdefs/{synthdef}.scsyndef
     *      inputParams: List of parameter names for front-end to use
     */
    explicit Section(const std::string& audio_source, const std::string& control_source);

    ~Section();

    /*
     * Returns the port object for a given parameter, either corresponding to an output or an input.
     */
    Port* getPortFor(const std::string& param);
};
}

#endif // MODULAR_SYNTH_SECTION_H
