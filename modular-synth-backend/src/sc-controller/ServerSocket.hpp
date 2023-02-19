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
#include "SuperColliderPacketBuilder.hpp"

#define ADDRESS "127.0.0.1"
#define PORT  58000
#define OUTPUT_BUFFER_SIZE  1024

class ServerSocket : UdpSocket {
	IpEndpointName _endpoint; 

	std::thread _recvThread;

	std::map<std::string, void (*) (osc::ReceivedMessage)> waiting{};
	 
	osc::ReceivedMessage Recv();
protected:
	std::array<char, OUTPUT_BUFFER_SIZE> _inBuf{};
	std::array<char, OUTPUT_BUFFER_SIZE> _outBuf{};
public:
	SuperColliderPacketBuilder p{ _outBuf.data(), _outBuf.size() };

	ServerSocket(IpEndpointName endpoint);
	void Send();
	std::future<osc::ReceivedMessage> SendReceive();
	void Callback(std::string address, void (callback) (osc::ReceivedMessage) );
	 

};