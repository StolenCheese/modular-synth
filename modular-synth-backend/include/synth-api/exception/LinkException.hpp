//
// Created by bmsyi on 08/02/2023.
//

#ifndef MODULAR_SYNTH_LINKEXCEPTION_HPP
#define MODULAR_SYNTH_LINKEXCEPTION_HPP

#include "synth-api/ports/Port.h"

#include <exception>

namespace synth_api {
    enum exceptionType {
        LinkException_enum,
        AddedLink,
        OutputToOutput,
        AlreadyBoundInput,
        CyclicLinks,
        RemovedLink,
        NoSuchConnection,
        FatalOutputController
    };

    class LinkException : public std::exception {
    protected:
        explicit LinkException(char * message, Port &p1, Port &p2, exceptionType type = LinkException_enum) : p1(p1), p2(p2), message(message), type(type) {}
    public:
        Port& p1;
        Port& p2;
        char* message;
        exceptionType type;
    };

    class AddedLinkException : public LinkException {
    public:
        AddedLinkException(char * message, Port &p1, Port &p2, exceptionType type = AddedLink) : LinkException(message, p1, p2, type) {

        }
    };

    class OutputToOutputException : public AddedLinkException {
    public:
        OutputToOutputException(char * message, Port &p1, Port &p2) : AddedLinkException(message, p1, p2, OutputToOutput) { };
    };

    class AlreadyBoundInputException : public AddedLinkException {
    public:
        AlreadyBoundInputException(char * message, Port &p1, Port &p2) : AddedLinkException(message, p1, p2, AlreadyBoundInput) { };
    };

    class CyclicLinksException : public AddedLinkException {
    public:
        CyclicLinksException(char * message, Port &p1, Port &p2) : AddedLinkException(message, p1, p2, CyclicLinks) { };
    };


    class RemovedLinkException : public LinkException {
    public:
        RemovedLinkException(char * message, Port &p1, Port &p2, exceptionType type = RemovedLink) : LinkException(message, p1, p2, type) { };
    };

    class NoSuchConnectionException : public RemovedLinkException {
    public:
        NoSuchConnectionException(char * message, Port &p1, Port &p2) : RemovedLinkException(message, p1, p2, NoSuchConnection) { };
    };


    class FatalOutputControllerException : public std::exception {
    protected:
        char * message;
    public:
        explicit FatalOutputControllerException(char * message) : message(message) {};
    };

} // synth-api

#endif //MODULAR_SYNTH_LINKEXCEPTION_HPP
