#include "../../include/sc-controller/Synth.hpp"

Synth::Synth( int32_t index, std::map<std::string, std::variant< int, float, Bus>> controls)
    : Node( index)
    , controls(controls)
{
 
}


Synth::Synth()
    : Node( -1)
{
 
}

std::variant<int, float, Bus> Synth::get(const std::string& param)
{
    return controls[param];
}

void Synth::set(const std::string& param, const float v)
{
    controls[param] = v;
    SuperColliderController::get().n_set(index, { { param, v } });
}

void Synth::set(const std::string& param, const int v)
{
    if (controls.count(param)) {
        controls[param] = v;
        SuperColliderController::get().n_set(index, {{param, v}});
    }
    else {
        throw std::invalid_argument(param);
    }
}

void Synth::set(const std::string& param, const Bus& v)
{
    if (v.rate == BusRate::CONTROL)
    {
        controls.emplace(param, v);
        SuperColliderController::get().n_map(index, { { param, v.index } });
    }
    else
        set(param, v.index);
}


std::ostream& operator<<(std::ostream& os, const Synth& g)
{
    os << "Synth " << g.index << ":{";
   
    for (auto it = g.controls.cbegin(); it != g.controls.cend(); ++it)
    {
        if (const int* pval = std::get_if<int>(&it->second))
            os << it->first << " = " << *pval ;
        else  if (const float* pval = std::get_if<float>(&it->second))
           os << it->first << " = " << *pval   ;
        else 
           os << it->first << " = bus " << std::get<Bus>(it->second).index  ;
    }

    os << "}";
    return os;
}
