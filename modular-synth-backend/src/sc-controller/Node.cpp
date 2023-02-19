#include "Node.hpp"
#include "SuperColliderController.hpp"

Node::Node(SuperColliderController* s,int index) :s(s), index(index) {

}

void Node::Run(bool enable)
{
	s->n_run({ {index, (int)enable} });
}
