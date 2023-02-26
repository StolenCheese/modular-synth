//
// Created by bmsyi on 26/02/2023.
//

#ifndef MODULAR_SYNTH_BACKEND_SCCONTROLLEREXCEPTION_H
#define MODULAR_SYNTH_BACKEND_SCCONTROLLEREXCEPTION_H

#include <string>
#include <utility>

class SCControllerException : std::exception {
protected:
    std::string message;
    explicit SCControllerException(std::string message) : message(std::move(message)) {}
public:
    std::string what() { return message; }

};


#endif //MODULAR_SYNTH_BACKEND_SCCONTROLLEREXCEPTION_H
