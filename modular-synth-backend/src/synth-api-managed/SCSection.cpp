// wrap_native_class_for_mgd_consumption.cpp
// compile with: /clr /LD0

#include "synth-api/section/Section.h"
#include "synth-api/ports/Port.h"

#include <msclr/marshal_cppstd.h>
#include <vcclr.h>

#include "synth-api-managed/ManagedLinkException.hpp"
#include "synth-api-managed/SCPort.hpp" 



#using <System.dll>

using namespace System;

// ATM this "Section" uses the lower level synth abstraction
//  Needs to be changed to the port model, this is a proof of concept

namespace SynthAPI {
	///< summary>
	/// A SuperCollider Section representation, linked to a synth node on the server
	///</summary>
	public
	ref class SCSection {
	private:
		synth_api::Section* m_section;
		array<String^>^ params;

		/* TODO @mp2015: Is this safe in the context of buses? */
		float GetValueOf(String^ param)
		{
			auto s = msclr::interop::marshal_as<std::string>(param);
			return std::get<float>(m_section->synth->get(s));
		}

		///< summary>
/// Allocate the native object on the C++ Heap via a constructor
///</summary>
		SCSection(synth_api::Section* section)
		{
			m_section = section;

			try {
				// Generate the synth on the server.
				// TODO @mp2015: Currently blocking, in future will use a bool valid
				auto size = m_section->synth->controls.size();
				params = gcnew array<String^>(size);
				int i = 0;

				// Cache the params of the function locally, to save lots of re-generating of strings

				for (auto it = m_section->synth->controls.begin(); it != m_section->synth->controls.end(); ++it) {
					params[i] = gcnew String(it->first.c_str());
					++i;
				}

			}
			catch (std::exception& ex) { 
				throw defaultException(ex);
			}
		}


	public:

		static SCSection^ FromSynthdef(String^ audioSynthDef, String^ controlSynthDef) {

			try {
				std::string audio_synth_def = audioSynthDef == nullptr ? "" : msclr::interop::marshal_as<std::string>(audioSynthDef);
				std::string control_synth_def = controlSynthDef == nullptr ? "" : msclr::interop::marshal_as<std::string>(controlSynthDef);

				auto m_section = new synth_api::Section(audio_synth_def, control_synth_def);

				return gcnew SCSection(m_section);
			}
			catch (std::exception& ex) {
				throw defaultException(ex);
			}
		}

		static SCSection^ FromMidi(String^ midi) {
			try {
				std::string _midi = msclr::interop::marshal_as<std::string>(midi);

				auto m_section = new synth_api::Section(_midi);

				return gcnew SCSection(m_section);
			}
			catch (std::exception& ex) {
				throw defaultException(ex);
			}
		}

		/*
		FRONT-END: _EXTREMELY_ important that, when you delete an SCSection object, you've already removed all links connecting to its ports! Otherwise you'll get use-after-free.
		If this is too inconvenient then you'll have to speak to Kofi. (Kofi: OutputPort has heap-allocated LogicalBus, right? You'd need to propagate down a removed LB *before*
		delete of LB if we want front-end to think less about it)
		*/
		// Deallocate the native object on a destructor
		~SCSection()
		{
			delete m_section->synth;
			// TODO: Delete section, implement section destructor
			// implement port destructor too
		}

	protected:
		// Deallocate the native object on the finalizer just in case no destructor is called
		!SCSection()
		{
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

		// Currently a null check, in future will also show if the node exists on the server yet
		bool Valid()
		{
			return m_section->synth != nullptr;
		}

		void Set(String^ param, float value)
		{
			auto s = msclr::interop::marshal_as<std::string>(param);
			m_section->synth->set(s, value);
		}

		property array<String^>^ controls {
			array<String^>^ get() { return params; }
		}

		property int index
		{
			int get() { return m_section->synth->index; }
		}

		void Run(bool run)
		{
			m_section->synth->Run(run);
		}
	};
}