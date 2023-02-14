//
// Created by bmsyi on 08/02/2023.
//

#ifndef MODULAR_SYNTH_PORT_H
#define MODULAR_SYNTH_PORT_H

#include <cstdint>
#include <list>
#include <set>

namespace synth_api {

    class Port {
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

    protected:
        // SuperCollider bus identifier
        uint64_t bus;

        // Holds all actual front-end created outgoingConnections (models wires)
        std::set<Port *> outgoingConnections;

        // Holds all back-end created outgoingConnections (models ports on the same section)
        std::set<Port *> outgoingSymbolicLinks;

        explicit Port(uint64_t bus, std::list<Port *>::iterator identifier) : bus(bus), identifier(identifier) {};

        /*
         * Checks for cycles existing in the tree - these can cause problems
         * (e.g. infinite stall, feedback loop) in super collider!
         */
        void cyclicCheck(Port *target);

        /*
         *
         */
        virtual void subscribe(Port *) = 0;
        virtual void unsubscribe(Port *) = 0;

    private:
        std::list<Port *>::iterator identifier;
    };

}

#endif //MODULAR_SYNTH_PORT_H
