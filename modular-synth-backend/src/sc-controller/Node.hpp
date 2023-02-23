
#pragma once 
#include <cstdint>
class SuperColliderController;

class Node {
public:
	int32_t index;
	SuperColliderController* s;
	Node(SuperColliderController* s, int32_t index);

	void Run(bool enable);
};

