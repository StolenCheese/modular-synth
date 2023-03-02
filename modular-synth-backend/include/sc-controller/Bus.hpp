class Bus;

#pragma once

#include<string>

#include<variant> 

enum class BusRate {
	CONTROL,
	AUDIO
};

class SuperColliderController;

class Bus {
	friend class SuperColliderController;

private:

	Bus(int index);

public:
	Bus(); 

	BusRate rate{ BusRate::CONTROL };
	int index;
	// a symbol consisting of the letter 'c' or 'a' (for control or audio) followed by the bus's index. This may be used when setting a synth node's control inputs to map the input to the control bus.
	std::string asMap() const;

	std::variant<int, float> get() const;

	//These are not constant - they are simply not cached in the bus.
	// They are actually very dynamic
	// Associates to /c_set
	void set(const float v) const;
	// Associates to /c_set
	void set(const int v) const;


};
