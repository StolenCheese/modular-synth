//
// Created by bmsyi on 26/02/2023.
//


#include "sc-controller/exceptions/SCControllerException.h"
#include <string>

class UnexpectedBundleException : public SCControllerException {
public:
    explicit UnexpectedBundleException(std::string message) : SCControllerException(message) {}
};

