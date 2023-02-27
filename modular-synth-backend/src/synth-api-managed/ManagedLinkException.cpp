#include "SCPort.cpp"

#include <msclr/marshal_cppstd.h>
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
}