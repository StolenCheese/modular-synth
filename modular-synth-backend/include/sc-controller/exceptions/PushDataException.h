//
// Created by bmsyi on 26/02/2023.
//

#ifndef MODULAR_SYNTH_BACKEND_EXCEPTIONS_H
#define MODULAR_SYNTH_BACKEND_EXCEPTIONS_H

#include <exception>
#include <string>
#include "sc-controller/exceptions/SCControllerException.h"

class PushDataException : public SCControllerException {
public:
    explicit PushDataException(std::string message) : SCControllerException(message) {};
};


#endif //MODULAR_SYNTH_BACKEND_EXCEPTIONS_H
