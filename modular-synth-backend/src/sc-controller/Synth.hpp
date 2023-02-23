class Synth;

#pragma once

#include "Bus.hpp"
#include "Node.hpp"
#include "SuperColliderController.hpp"
#include <map>

class Synth : public Node {
    friend class SuperColliderController;
    friend std::ostream& operator<<(std::ostream& os, const Synth& g);

private:
    std::map<std::string, std::variant<int, float, Bus>> controls{};

    // Create a new synth based on params from the server
    Synth(SuperColliderController* s, int32_t index, std::map<std::string, std::variant< int, float, Bus>> controls);

    // default constructor exists to be used in future objects
    Synth();

public:
    // Get returns cached values, but, if everything is working, that's fine
    std::variant<int, float, Bus> get(const std::string& param);

    // Associates to /n_set
    void set(const std::string& param, const float v);
    // Associates to /n_set
    void set(const std::string& param, const int v);

    // Associates to /n_map if control, /n_set if audio
    void set(const std::string& param, const Bus& v);
};
