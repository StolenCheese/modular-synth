#include "osc/OscOutboundPacketStream.h"
#include "osc/OscReceivedElements.h"
#include "osc/OscPacketListener.h"
#include "ip/UdpSocket.h"
#include <chrono>
#include <iostream> 
#include <vector>
#include <algorithm>
#include <numeric>
#include <future>
#include <string>
#include <mutex>

#include <WinSock2.h>
#include <array>
#include <map>
#include <thread>
#include <variant>

#define ADDRESS "127.0.0.1"
#define PORT  58000
#define OUTPUT_BUFFER_SIZE  1024

class ServerSocket : UdpSocket {
	IpEndpointName _endpoint; 

	std::thread _recvThread;
	std::array<char, OUTPUT_BUFFER_SIZE> _inBuf{};
	std::array<char, OUTPUT_BUFFER_SIZE> _outBuf{};

	std::map<std::string, void (*) (osc::ReceivedMessage)> waiting{};
	 
	osc::ReceivedMessage Recv();

public:
	osc::OutboundPacketStream p;

	ServerSocket(IpEndpointName endpoint);
	void Send();
	std::future<osc::ReceivedMessage> SendRecieve();
	void Callback(std::string address, void (callback) (osc::ReceivedMessage) );
	 

	template <typename  T>
	void push(T v) {
		p << v;
	}

	template <>
	void push(std::string v) {
		p << v.c_str();
	}

	template <>
	void push(float v) {
		p << v;
	}

	template <typename ... Ts>
	void push(std::variant<Ts...> var) {
		if (const auto intPtr(std::get_if<int>(&var)); intPtr)
			p << *intPtr;
		else if (const auto floatPtr(std::get_if<float>(&var)); floatPtr)
			p << *floatPtr;
		else if (const auto strPtr(std::get_if<float>(&var)); strPtr)
			p << *strPtr.c_str();
	}



 
};