//
// Created by bmsyi on 26/02/2023.
//


#include <exception>
#include <string>

class UnitializedConnectionException : public SCControllerException {
public:
    explicit UnitializedConnectionException(std::string message) : SCControllerException(message) {}
};


