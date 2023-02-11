//
// Created by bmsyi on 09/02/2023.
//

#ifndef MODULAR_SYNTH_INPUTPORT_H
#define MODULAR_SYNTH_INPUTPORT_H

#include "Port.h"

#include <cstdint>
#include <set>

namespace synth_api {

    class InputPort : public Port {
    private:
        std::set<InputPort *> subscribers;
        Port *controller;
        void subscribe(InputPort *other);
        void unsubscribe(InputPort *other);

        /*
         * Makes this port a root controller.
         * A root controller is a node with no controller - i.e. it provides the bus.
         * We view the dependency graph as a 'reverse' tree, with nodes only having
         * pointers upwards further up into the tree. To make the node a root
         * controller, we reverse the direction of every controller link as it goes
         * up further into the tree
         *
         * Throws:
         *  FatalOutputPortControllerException: If you call this function when
         *      the top of the dependency graph is an OutputPort. An OutputPort cannot
         *      have its direction of control reversed (as it is an independent Port).
         *      If this exception is thrown, then there is an issue with back-end logic.
         *      Contact Jamie in the event this exception is ever raised.
         */
        void makeRootController();

        void follow(InputPort *other);
        void notify();
    public:
        void linkTo(Port *other) override;
        void removeLink(Port *other) override;
    };

}

#endif //MODULAR_SYNTH_INPUTPORT_H
