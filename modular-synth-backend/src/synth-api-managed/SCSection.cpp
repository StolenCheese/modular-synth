// wrap_native_class_for_mgd_consumption.cpp
// compile with: /clr /LD0
#include "synth-api/ports/Port.h"
#include "synth-api/section/Section.h"
#include "sc-controller/Synth.hpp"
#include "SCPort.cpp"

#include <msclr/marshal_cppstd.h>
#include <vcclr.h>

#using <System.dll>

using namespace System;

//ATM this "Section" uses the lower level synth abstraction
// Needs to be changed to the port model, this is a proof of concept

namespace SynthAPI {

    public ref class SCSection {
    private:
        synth_api::Section* m_section;
        array<String^>^ params;

        /* TODO @mp2015: Is this safe in the context of buses? */
        float GetValueOf(String^ param) {
            auto s = msclr::interop::marshal_as<std::string>(param);
            return std::get<float>(m_section->synth->get(s));
        }
    public:


        // Allocate the native object on the C++ Heap via a constructor
        SCSection(String^ synthdef)  {
            try {
                std::string cppsynthdef = msclr::interop::marshal_as<std::string>(synthdef);
                m_section = new synth_api::Section(cppsynthdef.c_str());
                // Generate the synth on the server.
                // TODO @mp2015: Currently blocking, in future will use a bool valid
                auto size = m_section->synth->controls.size();
                 params = gcnew array< String^ >(size);
                int i = 0;

                // Cache the params of the function locally, to save lots of re-generating of strings
       
                for (auto it = m_section->synth->controls.begin(); it != m_section->synth->controls.end(); ++it) {
                    params[i] = gcnew String(it->first.c_str());
                    ++i;
                }
                 

            }
            catch (std::exception& ex) {
                //standard conversion from native to managed exception
                throw gcnew System::Exception(gcnew System::String(ex.what()));
            }
        }
        
        // Deallocate the native object on a destructor
        ~SCSection() {
            delete m_section->synth;
            // TODO: Delete section, implement section destructor
            // implement port destructor too
        }

    protected:
        // Deallocate the native object on the finalizer just in case no destructor is called
        !SCSection() {
            delete m_section;
        }

    public:

        SCPort^ getPortFor(String^ param) {
            auto s = msclr::interop::marshal_as<std::string>(param);
            synth_api::Port* port = m_section->getPortFor(s);
            if (port) {
                return gcnew SCPort(port);
            }
            return nullptr;
        }

        //Currently a null check, in future will also show if the node exists on the server yet
        bool Valid() {
            return m_section->synth != nullptr;
        }

        void Set(String^ param, float value) {
            auto s = msclr::interop::marshal_as<std::string>(param);
            m_section->synth->set(s, value);
        }

        property array<String^>^ controls {
            
            array<String^>^ get(){ return params; }
         
        }

        property int index {
            int get() { return m_section->synth->index; }
        }

        void Run(bool run) {
            m_section->synth->Run(run);
        }

    };
}