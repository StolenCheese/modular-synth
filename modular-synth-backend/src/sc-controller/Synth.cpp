#include "sc-controller/Synth.hpp"
#include "sc-controller/SuperColliderController.hpp"

Synth::Synth( int32_t index, std::string audio_rate_synth, std::string control_rate_synth)
    : Node( index) ,
    audio_rate_synth(audio_rate_synth),
    control_rate_synth(control_rate_synth),
    valid(false),
    outputRate(BusRate::AUDIO)
{
    if (audio_rate_synth == "" && control_rate_synth != "") {
        outputRate = BusRate::CONTROL;
    } 
}


Synth::Synth()
    : Node( -1), valid(false), outputRate(BusRate::CONTROL)
{
 
}

// Get output rate
BusRate const& Synth::getOutputRate() {
    return outputRate;
}

// Set output rate of synth, possibly changing node representation
bool Synth::setOutputRate(BusRate const& rate){
    if (rate == outputRate)
        // Already set
        return true;
    else if (audio_rate_synth == "" || control_rate_synth == "") 
        // Unable to change
        return false;
    
    auto& server = SuperColliderController::get();

    int new_id = server.allocateSynthID();

    auto& new_synth = rate == BusRate::CONTROL ? control_rate_synth : audio_rate_synth;

    std::vector<  std::pair<
        std::variant<int, std::string>,
        std::variant<int, float, std::string>
        >> controls_copy{};

    for (auto it = controls.begin(); it != controls.end(); it++)
    {
        std::variant<int, float, std::string> val;

        if (const int* ptr(std::get_if<int>(&it->second)); ptr)
            val = *ptr;
        if (const float* ptr(std::get_if<float>(&it->second)); ptr) {
            val = *ptr;
        }
        if (const Bus * ptr(std::get_if<Bus>(&it->second)); ptr)
            if (it->first.substr(0, 3) == "out" || it->first.substr(0, 2) == "in") {
                val = ptr->index;
            }
            else {
                val = ptr->asMap();
            }

        controls_copy.push_back(std::make_pair(
            it->first, 
            val

        ));
    }

    server.s_new(new_synth, new_id,4, index, controls_copy);
     
    outputRate = rate;
    index = new_id;
}

// Has this synth been assigned controls?
bool Synth::isValid() {
    return valid;
}

void Synth::loadControls(std::map<std::string, std::variant< int, float, Bus>> controls) {
    this->controls = controls;
    valid = true;
}


std::variant<int, float, Bus> Synth::get(const std::string& param)
{
    return controls[param];
}


void Synth::set(const std::string& param, const float v)
{   
    // std::cout << "Setting " << param << " to " << v << std::endl;
    controls[param] = v;
    SuperColliderController::get().n_set(index, { { param, v } });
}

void Synth::set(const std::string& param, const int v)
{    
    //std::cout << "Setting " << param << " to " << v << std::endl;
    if (controls.count(param)) {
        controls[param] = v;
        SuperColliderController::get().n_set(index, {{param, v}});
    }
    else {
        throw std::invalid_argument(param);
    }
}

void Synth::set(const std::string& param,  Bus const& v)
{
    //std::cout << "Setting " << param << " to bus " << v.index << std::endl;
    controls[param] = v;

    // OUT params write to a bus ID, not bus MAP
    if (param.substr(0, 3) == "out" || param.substr(0, 2) == "in") {
		//std::cout << param << " is in/out, so using ID and not MAP" << std::endl;
        set(param, v.index);
        return;
    }

    if (v.rate == BusRate::CONTROL)
    {
        SuperColliderController::get().n_map(index, { { param, v.index } });
    }
    else
    { 
        SuperColliderController::get().n_mapa(index, { { param, v.index } });
    }
}


std::ostream& operator<<(std::ostream& os, const Synth& g)
{
    os << "Synth " << g.index << ":{";
   
    for (auto it = g.controls.cbegin(); it != g.controls.cend(); ++it)
    {
        if (const int* pval = std::get_if<int>(&it->second))
            os << it->first << " = " << *pval ;
        else  if (const float* pval = std::get_if<float>(&it->second))
           os << it->first << " = " << *pval   ;
        else 
           os << it->first << " = bus " << std::get<Bus>(it->second).index  ;
    }

    os << "}";
    return os;
}
