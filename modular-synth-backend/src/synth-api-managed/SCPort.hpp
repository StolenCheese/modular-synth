#pragma once
#include "ManagedLinkException.hpp"

#using <System.dll>

using namespace System;

namespace SynthAPI {

	public ref class SCPort {
	private:
		synth_api::Port* m_port;

	public:
		SCPort(synth_api::Port* port);

		/* (Back-end) Important: We do NOT deallocate m_port! That is deallocated by the SECTION, as SCPort is just a temporary wrapper and not something we re-use.
		Multiple wrappers may be used for the same port! */
		~SCPort();

		void linkTo(SCPort^ other);

		void removeLink(SCPort^ other);

		void setDefault(float value);

		float getValue();
	};
}
