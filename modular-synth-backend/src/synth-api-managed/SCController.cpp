// wrap_native_class_for_mgd_consumption.cpp
// compile with: /clr /LD  
#include "sc-controller/SuperColliderController.hpp"
#include <msclr\marshal_cppstd.h>
#include <vcclr.h>

#using <System.dll>
 

using namespace System;

namespace SynthAPI {
    ///<summary>
    /// Static SuperCollider server representation
    ///</summary>
    public ref class SCController abstract sealed {
    public:
        // Allocate the native object on the C++ Heap via a constructor
        //ManagedClass() : m_Impl(new UnmanagedClass) {}

        // Deallocate the native object on a destructor
       // ~ManagedClass() {
       //     delete m_Impl;
       // }

    protected:
        // Deallocate the native object on the finalizer just in case no destructor is called
        //!ManagedClass() {
       //     delete m_Impl;
       // }

    public:
        ///<summary>
        /// Connect to a supercollider server
        /// Currently has to be running locally
        ///</summary>
        static void Connect(String^ addr, int port) {
            auto s = msclr::interop::marshal_as<std::string>(addr);

            auto endpoint = IpEndpointName(s.c_str(), port);

            SuperColliderController::Connect(endpoint);

            //TODO: Design choice - do we allow for reconnection, or rely on save/load?

            SuperColliderController::get().g_deepFree({ 0 });
        }

        ///<summary>
        /// Display incoming OSC messages.
        ///Turns on and off printing of the contents of incoming Open Sound Control messages. This is useful when debugging your command stream.
        ///</summary>
        /// <param name="code">
        /// The values for the code are as follows:  <para />
        ///- 0 - turn dumping OFF.  <para />
        ///- 1 - print the parsed contents of the message.  <para />
        ///- 2 - print the contents in hexadecimal. <para />
        ///- 3 - print both the parsed and hexadecimal representations of the contents.
        ///</param>  
        static void DumpOSC(int code) {
            SuperColliderController::get().dumpOSC(code);
        }

    private:
        //UnmanagedClass* m_Impl;
    };
}