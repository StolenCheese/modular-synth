//
// Created by bmsyi on 09/02/2023.
//

#include "Port.h"
#include "../exception/LinkException.hpp"

#include <set>
#include <list>

namespace synth_api {
    void Port::linkTo(Port *other) {
        // Ensure that the requested link is not already in the chain for this element.
        cyclicCheck(other);
        this->outgoingConnections.insert(other);
    }

    void Port::removeLink(Port *other) {
        auto loc = this->outgoingConnections.find(other);
        if (loc == this->outgoingConnections.end()) {
            throw new NoSuchConnectionException((char *) "No such connection exists!", *this, *other);
        }
        this->outgoingConnections.erase(other);
    }

    void Port::cyclicCheck(Port *target) {
        // INVARIANT: There are no existing cycles (assuming all links are undirected)
        // Complexity: O(n) where n is the number of ports across sections.
        std::list<Port *> queue;
        queue.insert(queue.end(), target);
        while (!queue.empty()) {
            Port *next_node = queue.front();
            queue.pop_front();

            // Scan over both *real* and *symbolic* links
            for (auto it = next_node->outgoingConnections.begin(); it != next_node->outgoingConnections.end(); ++it) {
                Port * outgoingLink = *it;

                // if our target is matched, then introducing the link would raise a cycle,
                // so we throw an exception to alert front-end the user is doing something bad
                if (outgoingLink == target) {
                    throw new CyclicLinksException((char *) "Cyclic link detected!", *this, *target);
                }

                // by the precondition, we will never insert the same item twice
                // into the queue
                queue.insert(queue.cend(), outgoingLink);
            }

            for (auto it = next_node->outgoingSymbolicLinks.begin(); it != next_node->outgoingSymbolicLinks.end(); ++it) {
                Port * symLink = *it;
                if (symLink == target) {
                    throw new CyclicLinksException((char *) "Cyclic SYMBOLIC link detected!", *this, *target);
                }
                queue.insert(queue.end(), symLink);
            }
        }
    }
}