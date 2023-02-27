//
// Created by bmsyi on 11/02/2023.
//

#ifndef MODULAR_SYNTH_SECTION_H
#define MODULAR_SYNTH_SECTION_H

#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"
#include "synth-api/section/_model/PortManager.h"
#include "sc-controller/Synth.hpp"

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
                           const std::vector<std::pair<std::string, uint64_t>>& inputPortList,
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
     * TODO (@bms53 || @ksw40) && @mp2015: Currently, this just outputs a standard _model with three inputs and two outputs.
     *  We need actual parsing, not this random nonsense that's here currently!
     *
     * NOTE @jw2190: This should be suitable for testing. You can mess around with the dummy section I've set up
     * as the filepath parameter is arbitrary.
     *
     * Parameters:
     *      char * synthdef: known/dir/to/synthdefs/{synthdef}.scsyndef
     *      inputParams: List of parameter names for front-end to use
     */
    explicit Section(const char* synthDef);

    /*
     * Returns the port object for a given parameter, either corresponding to an output or an input.
     */
    Port* getPortFor(const std::string& param);
};
}

#endif // MODULAR_SYNTH_SECTION_H
