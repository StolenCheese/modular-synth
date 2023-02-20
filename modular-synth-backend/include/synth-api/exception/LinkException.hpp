//
// Created by bmsyi on 08/02/2023.
//

#ifndef MODULAR_SYNTH_LINKEXCEPTION_HPP
#define MODULAR_SYNTH_LINKEXCEPTION_HPP

#include "../ports/Port.h"

#include <exception>

namespace synth_api {

    class LinkException : public std::exception {
    protected:
        Port &p1;
        Port &p2;
        char * message;
        explicit LinkException(char * message, Port &p1, Port &p2) : p1(p1), p2(p2), message(message) {}
    };

    class AddedLinkException : public LinkException {
    public:
        AddedLinkException(char * message, Port &p1, Port &p2) : LinkException(message, p1, p2) {

        }
    };

    class OutputToOutputException : public AddedLinkException {
    public:
        OutputToOutputException(char * message, Port &p1, Port &p2) : AddedLinkException(message, p1, p2) { };
    };

    class AlreadyBoundInputException : public AddedLinkException {
    public:
        AlreadyBoundInputException(char * message, Port &p1, Port &p2) : AddedLinkException(message, p1, p2) { };
    };

    class CyclicLinksException : public AddedLinkException {
    public:
        CyclicLinksException(char * message, Port &p1, Port &p2) : AddedLinkException(message, p1, p2) { };
    };


    class RemovedLinkException : public LinkException {
    public:
        RemovedLinkException(char * message, Port &p1, Port &p2) : LinkException(message, p1, p2) { };
    };

    class NoSuchConnectionException : public RemovedLinkException {
    public:
        NoSuchConnectionException(char * message, Port &p1, Port &p2) : RemovedLinkException(message, p1, p2) { };
    };


    class FatalOutputControllerException : public std::exception {
    protected:
        char * message;
    public:
        explicit FatalOutputControllerException(char * message) : message(message) {};
    };

} // synth-api

#endif //MODULAR_SYNTH_LINKEXCEPTION_HPP
