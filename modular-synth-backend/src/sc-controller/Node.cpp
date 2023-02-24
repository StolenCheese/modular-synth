#include "Node.hpp"
#include "SuperColliderController.hpp"

Node::Node( int32_t index) : index(index) {

}

void Node::Run(bool enable)
{
	SuperColliderController::get().n_run({ {index, (int)enable} });
}

Node::~Node()
{
	SuperColliderController::get().n_free({ index });
}
