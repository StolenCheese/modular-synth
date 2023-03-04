//
// Created by bmsyi on 08/02/2023.
//

#ifndef MODULAR_SYNTH_PORT_H
#define MODULAR_SYNTH_PORT_H

#include <cstdint>
#include <list>
#include <set>
#include <stdexcept>

namespace synth_api {

    class LogicalBus;
    class Port {
        friend class LogicalBus;
        friend class Section;
        friend class PortManager;
    public:
        /*
         * Ports can be bound to each other. This is analogous to a
         	wire being placed connecting one to the other on an
         	actual modular synth.
         	The connection is *bi-directional*. A --> B <==> B --> A
         	The direction electricity flows can reverse as the user
         	changes other wires.

         	Parameters:
         	    Port other: Port to create link to

         	Throws: AddedLinkException (see subclasses)
         */
        virtual void linkTo(Port *other);

        /*
         * If two ports are connected, calling this method on one
          of them will remove the shared connection.

          Parameters:
                Port other: Port to remove connection from

          Throws: NoSuchConnectionException when no such link exists
         */
        virtual void removeLink(Port *other);

        /*
         * Adds a symbolic link to another wire (this is only for cyclic checking!)
         *
         * Front-end: Only use this if you know something that the section definition file cannot communicate. The
         * Section class automatically creates all symbolic links between inputs and outputs that should be needed.
         *
         * Parameters:
         *      Port other: Port to connect to
         */
        void symbolicLinkTo(Port *other);

        /*
         * Sets the default value of the port if the port is an InputPort
         */
        virtual void setDefault(float value);

        /*
         * Returns the value in the bus of this port
         * TODO @mp2015 || @ksw40: Implement this in relevant subclasses
         */
        virtual float getValue();

        virtual ~Port();

    //protected:
    public: //TODO: @ksw40 @mp2015 revert to protected

        explicit Port() : logicalBus(nullptr) {};

        // Logical bus to represent bus connections at a high-level. Abstracts away audio/control rate details.
        LogicalBus *logicalBus;

        // Holds all actual front-end created outgoingConnections (models wires)
        // TODO @bms53: Rename to imply it is bi-directional
        std::set<Port *> outgoingConnections;

        // Holds all back-end created outgoingConnections (models ports on the same section)
        // TODO @bms53: Rename to imply it is bi-directional
        std::set<Port *> outgoingSymbolicLinks;

        /*
         * Checks for cycles existing in the tree - these can cause problems
         * (e.g. infinite stall, feedback loop) in super collider!
         */
        void cyclicCheck(Port *target);

        /*
         * See InputPort
         * TODO @bms53: move docstrings
         */
        virtual void subscribe(Port *) = 0;
        virtual void unsubscribe(Port *) = 0;
    };

}

#endif //MODULAR_SYNTH_PORT_H
