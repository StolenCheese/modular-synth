
#include "synth-api-managed/ManagedLinkException.hpp"
#include "synth-api-managed/SCPort.hpp"
#using <System.dll>

using namespace System;

namespace SynthAPI {


	SCPort::SCPort(synth_api::Port* port) : m_port(port) {};

	/* (Back-end) Important: We do NOT deallocate m_port! That is deallocated by the SECTION, as SCPort is just a temporary wrapper and not something we re-use.
	Multiple wrappers may be used for the same port! */
	SCPort::~SCPort() {};

	void SCPort::linkTo(SCPort^ other) {
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


	void SCPort::removeLink(SCPort^ other) {
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

	void SCPort::setDefault(float value) {
		try {
			this->m_port->setDefault(value);
		}
		catch (std::exception& e) {
			throw defaultException(e);
		}
	}

	float SCPort::getValue() {
		try {
			return m_port->getValue();
		}
		catch (std::exception& e) {
			throw defaultException(e);
		}
	}
}
