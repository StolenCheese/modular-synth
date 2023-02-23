#include "Group.hpp"
#include <iostream>

#include "SuperColliderController.hpp"

void Group::syncTree()
{
    if (!dirty) return;

    s->SyncGroup(this);

    dirty = false;
    // print(m);
}

Group::Group(SuperColliderController* s, int index) : Node(s,index){}

std::ostream& operator<<(std::ostream& os, const Group& g)
{
    os << "Group " << g.index << ": children" << g.children;
    return os;
}
 