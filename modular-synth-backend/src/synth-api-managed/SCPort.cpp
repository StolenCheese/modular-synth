#include "synth-api/ports/Port.h"
#include "synth-api/exception/LinkException.hpp"
#include "ManagedLinkException.hpp"

#include <msclr/marshal_cppstd.h>
#include <vcclr.h>

#using <System.dll>

namespace SynthAPI {

    public ref class SCPort {
    private:
        synth_api::Port* m_port;

    public:
        SCPort(synth_api::Port *port) : m_port(port) {};

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
    };
}
