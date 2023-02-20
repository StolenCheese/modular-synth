#include "Bus.hpp"

Bus::Bus(SuperColliderController* s, int index)
    : s(s)
    , index(index)
{
}

std::string Bus::asMap()
{
    if (rate == BusRate::CONTROL)
        return std::format("c{}", index);
    return std::format("a{}", index);
}

std::future<std::variant<int, float>> Bus::get()
{
    return std::future<std::variant<int, float>>();
}
// TODO
void Bus::set(const float v)
{
}
void Bus::set(const int v)
{
}