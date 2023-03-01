#pragma once

#include "synth-api/exception/LinkException.hpp"
 

#using <System.dll>

/* See synth-api/exception/LinkException.hpp */
namespace SynthAPI {

    System::Exception^ categorizeException(synth_api::LinkException& link_e);
    System::Exception^ defaultException(std::exception& e);

    public ref class LinkException_t : public System::Exception {
    public:
        explicit LinkException_t(const char* message);

    };

    public ref class AddedLinkException_t : public LinkException_t {
    public:
        AddedLinkException_t(const char* message);
    };

    public ref class OutputToOutputException_t : public AddedLinkException_t {
    public:
        OutputToOutputException_t(const char* message);
    };

    public ref class AlreadyBoundInputException_t : public AddedLinkException_t {
    public:
        AlreadyBoundInputException_t(const char* message);
    };

    public ref class CyclicLinksException_t : public AddedLinkException_t {
    public:
        CyclicLinksException_t(const char* message);
    };


    public ref class RemovedLinkException_t : public LinkException_t {
    public:
        RemovedLinkException_t(const char* message);
    };

    public ref class NoSuchConnectionException_t : public RemovedLinkException_t {
    public:
        NoSuchConnectionException_t(const char* message);
    };


    public ref class FatalOutputControllerException_t : public System::Exception {
    public:
        explicit FatalOutputControllerException_t(const char* message);
    };
}
