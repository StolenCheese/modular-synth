//
// Created by kofi on 28/02/23.
//

#ifndef MODULAR_SYNTH_SETTERFUNCTOR_H
#define MODULAR_SYNTH_SETTERFUNCTOR_H

#include "sc-controller/Synth.hpp"

namespace synth_api {
    class SetterFunctor {
    private:
        std::string paramName;
        Synth *synth;
    public:
        SetterFunctor(std::string paramName, Synth *synth) : paramName(paramName), synth(synth) {}

        void operator()(int v);
        void operator()(float v);
        void operator()(Bus &v);
    };
}

#endif //MODULAR_SYNTH_SETTERFUNCTOR_H
