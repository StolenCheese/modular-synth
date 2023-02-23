
#include "ServerSocket.hpp"

#include <chrono>
using namespace std::chrono_literals;

osc::ReceivedMessage ServerSocket::Recv()
{
	size_t size = 0; 
		
	while ((size = ReceiveFrom(_endpoint, _inBuf.data(), OUTPUT_BUFFER_SIZE)) != SOCKET_ERROR) {

		osc::ReceivedPacket p(_inBuf.data(), size);
		try {
			if (p.IsMessage()) {
				return osc::ReceivedMessage(p);
			}
			else {
				throw std::exception("Received bundle when shouldn't");
			}
		}
		catch (osc::MalformedPacketException& p) {
			 //Try to receive again
			std::cout << "Failed to receive packet" << std::endl;
		}
	}

}



ServerSocket::ServerSocket(IpEndpointName endpoint) : 
	_endpoint(endpoint), UdpSocket() {


	Connect(_endpoint);
	//_recvThread = std::thread([=] { RecvLoop(); });
}

void ServerSocket::Send( )
{
	UdpSocket::Send(p.Data(), p.Size());
	//std::fill_n(_outBuf, OUTPUT_BUFFER_SIZE-1, 0);
	p.Clear();
}

std::future<osc::ReceivedMessage> ServerSocket::SendReceive()
{
	Send();

	// Create a thread to async wait on the socket's response
	// Only one thread should wait at any time, so this can maybe be turned into a constant background thread
	std::shared_ptr< std::promise<osc::ReceivedMessage>> p= std::make_shared< std::promise<osc::ReceivedMessage>>( );
	std::future<osc::ReceivedMessage> f3 = (*p).get_future();
	std::thread([p, this] { (*p).set_value_at_thread_exit(Recv()); }).detach();

	return f3;
}

void ServerSocket::Callback(std::string address, void(callback)(osc::ReceivedMessage))
{
	waiting.emplace(address, callback);
}


