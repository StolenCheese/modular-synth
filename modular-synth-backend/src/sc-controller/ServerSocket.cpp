#include "sc-controller/ServerSocket.hpp"
#include "sc-controller/exceptions/UnexpectedBundleException.h"

#include <exception>

osc::ReceivedMessage ServerSocket::Recv()
{
	size_t size = 0;

	while ((size = ReceiveFrom(_endpoint, _inBuf.data(), OUTPUT_BUFFER_SIZE)) != -1) {

		osc::ReceivedPacket p(_inBuf.data(), size);
		try {
			if (p.IsMessage()) {
				return osc::ReceivedMessage(p);
			}
			else {
				throw UnexpectedBundleException("Received bundle when shouldn't");
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
	////std::fill_n(_outBuf, OUTPUT_BUFFER_SIZE-1, 0);
	p.Clear();
}

osc::ReceivedMessage ServerSocket::SendReceive()
{
	Send();

	return Recv();
}


