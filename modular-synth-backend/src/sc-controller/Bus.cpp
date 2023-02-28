#include "sc-controller/Bus.hpp"
#include "sc-controller/SuperColliderController.hpp"
//#include <format>

Bus::Bus(int index) : index(index)
{
}
Bus::Bus() : index(-1)
{
}

std::string Bus::asMap()
{
	if (rate == BusRate::CONTROL)
		return "c" + std::to_string(index);
	return "a" + std::to_string(index);
}

std::variant<int, float> Bus::get()
{
	auto m = SuperColliderController::get().c_get({ index });
	auto i = m.ArgumentsBegin();
	i++; // index
	if ((*i).IsFloat()) {
		return (*i).AsFloat();
	}
	else {
		return (*i).AsInt32();
	}
}

void Bus::set(const float v)
{
	SuperColliderController::get().c_set({ {index,v} });
}

void Bus::set(const int v)
{
	SuperColliderController::get().c_set({ {index,v} });
}