class Synth;

#pragma once

#include "Bus.hpp"
#include "Node.hpp"
#include "SuperColliderController.hpp"

class Synth : public Node {
    friend class SuperColliderController;

private:
    std::set<std::string> params;

    // Create a new synth based on params from the server
    Synth(SuperColliderController* s, int index, std::set<std::string> params);

    // default constructor exists to be used in future objects
    Synth();

public:
    std::future<std::variant<int, float, Bus>> get(const std::string& param);

    // Associates to /n_set
    void set(const std::string& param, const float v);
    // Associates to /n_set
    void set(const std::string& param, const int v);

    // Associates to /n_map if control, /n_set if audio
    void set(const std::string& param, const Bus& v);
};
