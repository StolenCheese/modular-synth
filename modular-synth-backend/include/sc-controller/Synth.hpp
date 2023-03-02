#pragma once

#include "sc-controller/Bus.hpp"
#include "sc-controller/Node.hpp"

#include <map>
#include <variant>

class Synth : public Node {
    friend class SuperColliderController;
    friend std::ostream& operator<<(std::ostream& os, const Synth& g);

protected:

    std::string audio_rate_synth;
    std::string control_rate_synth;

    BusRate outputRate{BusRate::AUDIO};
    bool valid{false};

    // Create a new synth based on params from the server
    Synth(  int32_t index, std::string audio_rate_synth, std::string control_rate_synth);

    // default constructor exists to be used in future objects
    Synth();  

    void loadControls(std::map<std::string, std::variant< int, float, Bus>> controls);

public:

    // Get output rate
    BusRate const& getOutputRate();

    // Set output rate of synth, possibly changing node representation. Returns true if success
    bool setOutputRate(BusRate const& rate);

    // Has this synth been assigned controls?
    bool isValid();

    // Get returns cached values, but, if everything is working, that's fine
    std::variant<int, float, Bus> get(const std::string& param);

    // Associates to /n_set
    void set(const std::string& param, const float v);
    // Associates to /n_set
    void set(const std::string& param, const int v);

    // Associates to /n_map if control, /n_mapa if audio
    void set(const std::string& param,  Bus const& v);

    // DO NOT WRITE TO THIS, IT WON'T WORK, just to be looked at
    std::map<std::string, std::variant<int, float, Bus>> controls;
};
