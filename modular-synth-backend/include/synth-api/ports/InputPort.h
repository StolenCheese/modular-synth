//
// Created by bmsyi on 09/02/2023.
//

#ifndef MODULAR_SYNTH_INPUTPORT_H
#define MODULAR_SYNTH_INPUTPORT_H

#include "Port.h"
#include "synth-api/section/_model/PortManager.h"

#include <cstdint>
#include <iterator>
#include <set>

namespace synth_api {

    enum Rate {control, audio, dependent};

    class InputPort : public Port {
        friend class PortManager;
        friend class LogicalBus;

    private:
        uint16_t audioRateRequirement;
        Port *controller;
        uint64_t defaultValue;

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

        /*
         * A shortcut to set up the controller, bus, and subscribers so that `this` has `other` as a controller.
         */
        void follow(Port *other);

        /*
         * Updates the bus of all dependent ports.
         */
        void notify();
    protected:
        std::set<InputPort *> subscribers;
        /*
         * Subscribing to a node will give you updates when its bus changes.
         */
        void subscribe(Port *other) override;

        /*
         * Removing a port from the subscriber list
         */
        void unsubscribe(Port *other) override;

        bool addAudioRateRequirement();     // returns true iff port switched from control to audio
        bool removeAudioRateRequirement();  // returns true iff port switched from audio to control

        void connectToBus(LogicalBus* logicalBus);
        void disconnectFromBus();
    public:
        const Rate rate;

        explicit InputPort(uint64_t defaultValue, Rate rate, const std::list<Port *>::const_iterator identifier) : Port(identifier), controller(nullptr), defaultValue(defaultValue), rate(rate), audioRateRequirement(0) {
            subscribers = std::set<InputPort *>();
        };
        void linkTo(Port *other) override;
        void removeLink(Port *other) override;
        void setDefault(uint64_t value);
    };

}

#endif //MODULAR_SYNTH_INPUTPORT_H
