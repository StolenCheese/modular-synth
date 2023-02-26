#include "../../include/sc-controller/Node.hpp"
#include "../../include/sc-controller/SuperColliderController.hpp"

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
