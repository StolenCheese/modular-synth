#include "Bus.hpp"

#include<format>

Bus::Bus(SuperColliderController* s, int index)
    : s(s)
    , index(index)
{
}

std::string Bus::asMap()
{
    if (rate == BusRate::CONTROL)
        return "c" + std::to_string(index);
    return "a"+ std::to_string(index);
}

std::variant<int, float> Bus::get()
{
    return std::variant<int, float>();
}
// TODO
void Bus::set(const float v)
{
}
void Bus::set(const int v)
{
}