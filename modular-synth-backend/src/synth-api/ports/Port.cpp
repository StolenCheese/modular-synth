//
// Created by bmsyi on 09/02/2023.
//

#include "synth-api/ports/Port.h"
#include "synth-api/exception/LinkException.hpp"

#include <set>
#include <list>
#include <iostream>

namespace synth_api {
    void Port::linkTo(Port *other) {
        // Ensure that the requested link is not already in the chain for this element.
        cyclicCheck(other);
         

        this->outgoingConnections.insert(other);
        other->outgoingConnections.insert(this);
    }

    void Port::removeLink(Port *other) {
        auto loc = this->outgoingConnections.find(other);

        // front-end has messed up if they are trying to remove a non-existent connection
        // we crash as this is the first time an error has been detected.
        if (loc == this->outgoingConnections.end()) {
            throw NoSuchConnectionException((char *) "No such connection exists!", *this, *other);
        }
        this->outgoingConnections.erase(other);

        other->outgoingConnections.erase(other->outgoingConnections.find(this));
    }

    void Port::cyclicCheck(Port *target) {
        // INVARIANT: There are no existing cycles in a UAG (undirected acyclic graph)
        // Time & Space Complexity: O(n) where n is the number of ports across sections.
        std::list<Port *> queue;
        std::set<Port *> visited;
        queue.insert(queue.cend(), this);
        visited.insert(this);
        while (!queue.empty()) {
            Port *next_node = queue.front();

            // deletes without returning
            queue.pop_front();

            // Scan over both *real* and *symbolic* links
            for (auto outgoingLink : next_node->outgoingConnections) {

                // if we've already visited the node, we skip it
                // the visited check ensure we never add the same item to the queue for checking twice
                // (this should guarantee termination)
                if (visited.find(outgoingLink) != visited.cend()) {
                    continue;
                }
                // otherwise mark as visited
                visited.insert(outgoingLink);

                // if our target is matched, then introducing the link would raise a cycle,
                // so we throw an exception to alert front-end the user is doing something bad
                if (outgoingLink == target) {
                    throw CyclicLinksException((char *) "Cyclic link detected!", *this, *target);
                }

                queue.insert(queue.cend(), outgoingLink);
            }

            for (auto symLink : next_node->outgoingSymbolicLinks) {
                if (visited.find(symLink) != visited.cend()) {
                    continue;
                }
                visited.insert(symLink);

                if (symLink == target) {
                    throw CyclicLinksException((char *) "Cyclic SYMBOLIC link detected!", *this, *target);
                }
                queue.insert(queue.end(), symLink);
            }
        }
    }

    void Port::symbolicLinkTo(Port *other) {
        outgoingSymbolicLinks.insert(other);
    }

    void Port::setDefault(float value) {
        throw std::runtime_error("Cannot set default value of non-InputPort!");
    }

    float Port::getValue() {
        return 100;
    }

    Port::~Port() {
        for (const auto &p : outgoingConnections) {
            Port::removeLink(p);
        }
    };
}