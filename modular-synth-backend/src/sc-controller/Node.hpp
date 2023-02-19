
#pragma once 

class SuperColliderController;

class Node {
public:
	int index;
	SuperColliderController* s;
	Node(SuperColliderController* s,int index);

	void Run(bool enable);
};

