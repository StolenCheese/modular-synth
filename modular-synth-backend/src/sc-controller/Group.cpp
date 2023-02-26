#include "sc-controller/Group.hpp"
#include "sc-controller/SuperColliderController.hpp"

#include <iostream>

void Group::syncTree()
{
    if (!dirty) return;

    SuperColliderController::get().SyncGroup(this);

    dirty = false;
    // print(m);
}

Group::Group( int index) : Node(index){}

std::ostream& operator<<(std::ostream& os, const Group& g)
{
    os << "Group " << g.index << ": children" << g.children;
    return os;
}
 