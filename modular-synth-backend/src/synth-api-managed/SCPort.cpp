#include "synth-api/ports/Port.h"
#include "synth-api/exception/LinkException.hpp"
#include "ManagedLinkException.hpp"

#include <msclr/marshal_cppstd.h>
#include <vcclr.h>

#using <System.dll>

using namespace System;

namespace SynthAPI {

    public ref class SCPort {
    private:
        synth_api::Port* m_port;

    public:
        SCPort(synth_api::Port *port) : m_port(port) {};

        /* (Back-end) Important: We do NOT deallocate m_port! That is deallocated by the SECTION, as SCPort is just a temporary wrapper and not something we re-use. 
        Multiple wrappers may be used for the same port! */
        ~SCPort() {};

        void linkTo(SCPort^ other) {
            try {
                this->m_port->linkTo(other->m_port);
            }
            catch (synth_api::LinkException &e) {
                throw categorizeException(e);
            } 
            catch (std::exception &e) {
                throw defaultException(e);
            }
        }


        void removeLink(SCPort^ other) {
            try {
                this->m_port->linkTo(other->m_port);
            }
            catch (synth_api::LinkException& e) {
                throw categorizeException(e);
            }
            catch (std::exception& e) {
                throw defaultException(e);
            }
        }

        void setDefault(float value) {
            try {
                this->m_port->setDefault(float(value));
            }
            catch (std::exception& e) {
                throw defaultException(e);
            }
        }

        float getValue() {
            float cppvalue;
            try {
                float cppvalue = m_port->getValue();
            }
            catch (std::exception& e) {
                throw defaultException(e);
            }
            return cppvalue;
        }
    };
}
