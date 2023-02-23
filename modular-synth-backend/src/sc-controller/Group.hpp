#pragma once

#include "Node.hpp"
#include <cstdint>
#include <map>
#include <string>
#include <variant>


class Group :public  Node {
    friend class SuperColliderController;

private:
    bool dirty{true};
public:
    int32_t children{0};
    std::map<int32_t, Node*> subtree{};

    void syncTree();

	Group(SuperColliderController* s, int index);
};