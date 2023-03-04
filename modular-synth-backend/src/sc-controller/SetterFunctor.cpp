//
// Created by kofi on 28/02/23.
//

#include "sc-controller/SetterFunctor.h"
#include <stdio.h>

namespace synth_api {
    void SetterFunctor::operator()(int v) { synth->set(paramName, v); }

    void SetterFunctor::operator()(float v) { synth->set(paramName, v); }

    void SetterFunctor::operator()(Bus& v) { synth->set(paramName, v); }
}
