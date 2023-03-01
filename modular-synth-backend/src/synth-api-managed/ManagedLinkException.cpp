#include "synth-api/exception/LinkException.hpp"
#include "ManagedLinkException.hpp" 
#include <vcclr.h> 

#using <System.dll>

namespace SynthAPI {
	System::Exception^ categorizeException(synth_api::LinkException& link_e) {
		const char* msg = link_e.message;
		switch (link_e.type) {
		case (synth_api::exceptionType::AddedLink):
			return gcnew AddedLinkException_t(msg);
		case (synth_api::exceptionType::OutputToOutput):
			return gcnew AlreadyBoundInputException_t(msg);
		case (synth_api::exceptionType::CyclicLinks):
			return gcnew CyclicLinksException_t(msg);
		case (synth_api::exceptionType::RemovedLink):
			return gcnew RemovedLinkException_t(msg);
		case (synth_api::exceptionType::NoSuchConnection):
			return gcnew NoSuchConnectionException_t(msg);
		default:
			return gcnew LinkException_t(msg);
		};
	}

	System::Exception^ defaultException(std::exception& e) {
		return gcnew System::Exception(gcnew System::String(e.what()));
	}



	LinkException_t::LinkException_t(const char* message) : System::Exception(gcnew System::String(message)) {}



	AddedLinkException_t::AddedLinkException_t(const char* message) : LinkException_t(message) { }


	OutputToOutputException_t::OutputToOutputException_t(const char* message) : AddedLinkException_t(message) { };


	AlreadyBoundInputException_t::AlreadyBoundInputException_t(const char* message) : AddedLinkException_t(message) { };



	CyclicLinksException_t::CyclicLinksException_t(const char* message) : AddedLinkException_t(message) { };



	RemovedLinkException_t::RemovedLinkException_t(const char* message) : LinkException_t(message) { };



	NoSuchConnectionException_t::NoSuchConnectionException_t(const char* message) : RemovedLinkException_t(message) { };


	FatalOutputControllerException_t::FatalOutputControllerException_t(const char* message) : System::Exception(gcnew System::String(message)) {};


}