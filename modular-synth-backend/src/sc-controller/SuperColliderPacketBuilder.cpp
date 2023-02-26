#include "../../include/sc-controller/SuperColliderPacketBuilder.hpp"

void SuperColliderPacketBuilder::s_new( std::string definition, int synth, int action, int target, std::vector<std::pair<std::variant<int, std::string>, std::variant<float, int, std::string>>> control)
{
    *this << osc::BeginMessage("/s_new");
    *this << definition.c_str();
    *this << synth;
    *this << action;
    *this << target;
    push(control); 
    *this << osc::EndMessage;
}

SuperColliderPacketBuilder::SuperColliderPacketBuilder(char* data, size_t len) : osc::OutboundPacketStream( data,  len)
{
}
