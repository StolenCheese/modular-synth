#include "Synth.hpp"

Synth::Synth(SuperColliderController* _s, int index, std::set<std::string> _params)
    : Node(_s, index)
    , params(_params)
{
}

Synth::Synth()
    : Node(NULL, -1)
{
}

std::future<std::variant<int, float, Bus>> Synth::get(const std::string& param)
{
    auto msg = s->s_get(index, { param }).get();
    // TODO: Decode the message properly
    print(msg);

    osc::ReceivedMessage::const_iterator arg = msg.ArgumentsBegin();

    int id = (arg++)->AsInt32();
    arg++;

    if (arg != msg.ArgumentsEnd())
        throw osc::ExcessArgumentException();

    std::promise<std::variant<int, float, Bus>>
        prom {};
    auto f = prom.get_future();
    return f;
}

void Synth::set(const std::string& param, const float v)
{
    s->n_set(index, { { param, v } });
}

void Synth::set(const std::string& param, const int v)
{
    s->n_set(index, { { param, v } });
}

void Synth::set(const std::string& param, const Bus& v)
{
    if (v.t == BusType::CONTROL)
        s->n_map(index, { { param, v.index } });
    else
        set(param, v.index)
}
