class Synth;

#pragma once
 
#include "Node.hpp"
#include "Bus.hpp"
#include "SuperColliderController.hpp"

class Synth : public Node {
	friend class SuperColliderController;

	std::set<std::string> params;

	Synth(SuperColliderController* s,int index, std::set<std::string> params);
	 
	Synth();
};
 