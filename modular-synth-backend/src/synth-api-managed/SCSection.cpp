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
    ///<summary> 
    ///A SuperCollider Section representation, linked to a synth node on the server
    ///</summary> 
    public ref class SCSection {
    private:
        array< String^ >^ params;
        Synth* m_Impl;
    public:


        ///<summary> 
        ///Allocate the native object on the C++ Heap via a constructor 
        ///</summary>
        SCSection(String^ synthdef)   {
            try {
                //Generate the synth on the server.
                //Currently blocking, in future will use a bool valid
                m_Impl = SuperColliderController::get().InstantiateSynth(msclr::interop::marshal_as<std::string>(synthdef));
                auto size = m_Impl->controls.size();
                 params = gcnew array< String^ >(size);
                int i = 0;

                // Cache the params of the function locally, to save lots of re-generating of strings
       
                for (auto it = m_Impl->controls.begin(); it != m_Impl->controls.end(); ++it) {
                    params[i] = gcnew String(it->first.c_str());

                    i++;
                }
                 

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

        //Currently a null check, in future will also show if the node exists on the server yet
        bool Valid() {
            return m_Impl != nullptr;
        }

        //This all works with the old model, but without any wire attachments its pretty much useless
        void Set(String^ param, float value) {
            auto s = msclr::interop::marshal_as<std::string>(param);
            m_Impl->set(s, value);
        }       

        float Get(String^ param) {
            auto s = msclr::interop::marshal_as<std::string>(param);
            return std::get<float>(m_Impl->get(s));
        }

        property array<String^>^  controls {
            
            array<String^>^ get(){ return params; }
         
        }

        property int index {

            int get() { return m_Impl->index; }

        }

        void Run(bool run) {
            m_Impl->Run(run);
        }

    };
}