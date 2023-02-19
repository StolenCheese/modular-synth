#include "Synth.hpp"

Synth::Synth(SuperColliderController* _s,int index, std::set<std::string> _params) : Node(_s, index), params(_params){}


Synth::Synth( ) : Node(NULL, -1)  {}

