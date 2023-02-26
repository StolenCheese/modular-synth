
#pragma once 
#include <cstdint>
class SuperColliderController;

class Node {
public:
	int32_t index; 
	Node(  int32_t index);

	void Run(bool enable);

	virtual ~Node();
};

