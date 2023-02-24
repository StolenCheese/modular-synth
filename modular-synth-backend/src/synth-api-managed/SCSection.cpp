// wrap_native_class_for_mgd_consumption.cpp
// compile with: /clr /LD
#include <vcclr.h>
#include <Synth.hpp>
#include <msclr\marshal_cppstd.h>
#using <System.dll>

using namespace System;

//ATM this "Section" uses the lower level synth abstraction
// Needs to be changed to the port model, this is a proof of concept

namespace SynthAPI {

    public ref class SCSection {
    public:
        // Allocate the native object on the C++ Heap via a constructor
        SCSection(String^ synthdef)   {
            try {
                m_Impl = SuperColliderController::get().InstantiateSynth(msclr::interop::marshal_as<std::string>(synthdef));
               
            }
            catch (std::exception& ex) {
                //standard conversion from native to managed exception
                throw gcnew System::Exception(gcnew System::String(ex.what()));
            }
        }

        // Deallocate the native object on a destructor
        ~SCSection() {
            delete m_Impl;
        }

    protected:
        // Deallocate the native object on the finalizer just in case no destructor is called
        !SCSection() {
            delete m_Impl;
        }

    public:

        void Set(String^ param, float value) {
            auto s = msclr::interop::marshal_as<std::string>(param);
            m_Impl->set(s, value);
        }       
        float Get(String^ param) {
            auto s = msclr::interop::marshal_as<std::string>(param);
            return std::get<float>(m_Impl->get(s));
        }

        void Run(bool run) {
            m_Impl->Run(run);
        }

    private:
        Synth* m_Impl;
    };
}