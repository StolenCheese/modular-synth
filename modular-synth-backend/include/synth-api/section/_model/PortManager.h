//
// Created by bmsyi on 13/02/2023.
//

#ifndef MODULAR_SYNTH_PORTMANAGER_H
#define MODULAR_SYNTH_PORTMANAGER_H

#include "synth-api/ports/InputPort.h"
#include "synth-api/ports/OutputPort.h"
#include "synth-api/ports/Port.h"
#include "sc-controller/SetterFunctor.h"

#include <list>
#include <unordered_map>

namespace synth_api {
    class Section;

    class PortManager {
    private:
        enum Stage {OnStack, Explored};
        static std::unordered_map<Port*, Section*> parentMap;

    public:
        Port* in;

        Port* out;

        // TODO @bms53: Make these thread safe with locks!
        static InputPort* getNewInputPort(Section *parent, SetterFunctor setter, float defaultValue);
        static OutputPort* getNewOutputPort(Section *parent);

        /*
         * Update the server-side order-of-evaluation of the synths so that new synth-to-synth dependencies are respected.
         * To do this, we walk backwards from the root, towards the sources, finding a local topological ordering of the
         * synths that the root depends on.
         */
        static void reorder(OutputPort *root);
    };

}

#endif // MODULAR_SYNTH_PORTMANAGER_H
