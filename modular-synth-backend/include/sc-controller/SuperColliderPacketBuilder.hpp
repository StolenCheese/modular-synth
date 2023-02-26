
#pragma once

#include "osc/OscOutboundPacketStream.h"
#include "sc-controller/exceptions/PushDataException.h"
#include <string>
#include <vector>
#include <utility>
#include <variant>
#include <array>

class SuperColliderPacketBuilder :public osc::OutboundPacketStream
{

public:

	SuperColliderPacketBuilder(char* data, size_t len);

/*
	Create a new synth.
Create a new synth from a synth definition, give it an ID, and add it to the tree of nodes. There are four ways to add the node to the tree as determined by the add action argument which is defined as follows:

add actions:
0add the new node to the head of the group specified by the add target ID.1add the new node to the tail of the group specified by the add target ID.2add the new node just before the node specified by the add target ID.3add the new node just after the node specified by the add target ID.4the new node replaces the node specified by the add target ID. The target node is freed.

Controls may be set when creating the synth. The control arguments are the same as for the n_set command.

If you send /s_new with a synth ID of -1, then the server will generate an ID for you. The server reserves all negative IDs. Since you don't know what the ID is, you cannot talk to this node directly later. So this is useful for nodes that are of finite duration and that get the control information they need from arguments and buses or messages directed to their group. In addition no notifications are sent when there are changes of state for this node, such as /go, /end, /on, /off.

If you use a node ID of -1 for any other command, such as /n_map, then it refers to the most recently created node by /s_new (auto generated ID or not). This is how you can map the controls of a node with an auto generated ID. In a multi-client situation, the only way you can be sure what node -1 refers to is to put the messages in a bundle.

This message now supports array type tags ($[ and $]) in the control/value component of the OSC message. Arrayed control values are applied in the manner of n_setn (i.e., sequentially starting at the indexed or named control). See the Node Messaging helpfile.
:param:definition: - synth definition name
:param:synth: - synth ID
:param:action: - add action (0,1,2, 3 or 4 see below)
:param:target: - add target ID
:param:control: - a control index or name
:param:interpreted: - floating point and integer arguments are interpreted as control value.  a symbol argument consisting of the letter 'c' or 'a' (for control or audio) followed by the bus's index.
 */
	void s_new(std::string definition, int synth, int action, int target, std::vector<std::pair<std::variant<int, std::string>, std::variant<float, int, std::string>>> control);


	template <typename  T>
	void push(T& v) {
		throw PushDataException("Unable to push data");
	}

	template <typename  T>
	void push(std::vector<T>& v) {
		for (auto& i : v)
			push(i);
	}
	template <>
	void push(std::string& v) {
		*this << v.c_str();
	}
	template <>
	void push(const std::string& v) {
		*this << v.c_str();
	}
	template <>
	void push(float& v) {
		*this << v;
	}
	template <>
	void push(const float& v) {
		*this << v;
	}

	template <>
	void push(int& v) {
		*this << v;
	}
	template <>
	void push(const int& v) {
		*this << v;
	}
	template <typename  T0, typename  T1>
	void push(std::tuple<T0, T1>& v) {
		push(std::get<0>(v));
		push(std::get<1>(v));
	}

	template <typename  T0, typename  T1, typename  T2>
	void push(std::tuple<T0, T1, T2>& v) {
		push(std::get<0>(v));
		push(std::get<1>(v));
		push(std::get<2>(v));
	}

	template <typename  T0, typename  T1>
	void push(std::pair<T0, T1>& v) {
		push(v.first);
		push(v.second);
	}

	template <typename  T0, typename  T1>
	void push(std::variant<T0, T1>& v) {
		if (const T0 * ptr(std::get_if<T0>(&v)); ptr)
			push(*ptr);
		if (const T1 * ptr(std::get_if<T1>(&v)); ptr)
			push(*ptr);
	}

	template <typename  T0, typename  T1, typename  T2>
	void push(std::variant<T0, T1,T2>& v) {
		if (const T0 * ptr(std::get_if<T0>(&v)); ptr)
			push(*ptr);
		if (const T1 * ptr(std::get_if<T1>(&v)); ptr)
			push(*ptr);
		if (const T2 * ptr(std::get_if<T2>(&v)); ptr)
			push(*ptr);
	}

};
