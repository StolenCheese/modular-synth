#include "osc/OscOutboundPacketStream.h"
#include "osc/OscReceivedElements.h"
#include "osc/OscPacketListener.h"
#include "ip/UdpSocket.h"
#include <iostream>

#include "SuperColliderController.hpp"
#include <WinSock2.h>
#include <vector>
#include <utility>
#include <variant>

#define ADDRESS "127.0.0.1"
#define PORT 58000

#define OUTPUT_BUFFER_SIZE 1024

int main(int argc, char* argv[])
{

    // https://doc.sccode.org/Reference/Server-Command-Reference.html
    // cd "C:\Program Files\SuperCollider-3.13.0-rc1"; ./scsynth.exe -u 58000

    std::variant<int, float, std::string> intFloatString;

    (void)argc; // suppress unused parameter warnings
    (void)argv; // suppress unused parameter warnings

    auto endpoint = IpEndpointName(ADDRESS, PORT);
      
    SuperColliderController::Connect(endpoint);
    auto& server = SuperColliderController::get();
    // transmitSocket.Bind(IpEndpointName(IpEndpointName::ANY_ADDRESS, PORT + 1));

    std::cout << "Transmitting test packets" << std::endl;
     
    server.dumpOSC(1);
    // reset everything
    server.g_deepFree({ 0 });

    server.root.syncTree();

    std::cout << "Version:" << std::endl;
    std::cout << server.version().AddressPattern() << std::endl;

    auto s1 = server.InstantiateSynth("sin-kr");

    std::cout << *s1 << std::endl;

    auto s2 = server.InstantiateSynth("sin-ar");


    std::cout << *s2 << std::endl;

    auto b = server.InstantiateBus();
 

    std::cout << "Linking synths:" << std::endl;
    s1->set("out", b.index); 
    s1->set("add", 440);
    s1->set("mul", 100);
    s1->set("freq", 1);

    s2->set("freq", b);


    std::cout << *s1 << std::endl;
    std::cout << *s2 << std::endl;

    std::string s;
    std::cin >> s;

    

    delete s1;
    delete s2;


    {
       

       // std::map<int, std::pair<std::string, std::vector<std::pair<std::string, osc::ReceivedMessageArgument*>>>> tree{};

      //  osc::ReceivedMessage::const_iterator arg = m.ArgumentsBegin();


        //bool flag = (arg++)->AsBool();
        //int nodeID = (arg++)->AsInt32();
        //int childCount = (arg++)->AsInt32();
        //
        //int i = 3;

        //while (i < m.ArgumentCount()) {
        //    int n_id = (arg++)->AsInt32();
        //    int children = (arg++)->AsInt32();
        //    if (children == -1) 
        //    {
        //        const char* s_type = (arg++)->AsString();
        //        if (flag)
        //        {
        //           int M = (arg++)->AsInt32();
        //          // std::vector<std::pair<std::string, osc::ReceivedMessageArgument*>> args{M};
  
        //           //TODO:
        //            for (size_t j = 0; j < M; j++)
        //           {
        //                                (arg++);
        //                                (arg++);
        //               //args.push_back(std::make_pair((arg++)->AsString(), new osc::ReceivedMessageArgument(&(arg++))))
        //           }
        //
        //               i += 4 + M * 2;
        //           tree.emplace(n_id, std::make_pair(s_type, NULL));
        //        }
        //        else
        //        {
        //            i += 3;
        //             tree.emplace(n_id, std::make_pair(s_type, NULL));
        //        }
        //    }
        //    else
        //    {
        //        i += 2;
        //    }
        //}

        //if (arg != m.ArgumentsEnd())
        //    throw osc::ExcessArgumentException();
    }
     
}


