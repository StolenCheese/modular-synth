#include "SuperColliderCommander.hpp"  
    std::future<osc::ReceivedMessage> SuperColliderCommander::quit(){
        
        
        p << osc::BeginMessage("/quit");
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::notify(int notifications, int client){
        
        
        p << osc::BeginMessage("/notify");
        p << notifications;
        p << client;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::status(){
        
        
        p << osc::BeginMessage("/status");
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::cmd(std::string command, ... arguments){
        
        
        p << osc::BeginMessage("/cmd");
        p << command;
        p << arguments;
        
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::dumpOSC(int code){
        
        
        p << osc::BeginMessage("/dumpOSC");
        p << code;
        
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::sync(int identifying){
        
        
        p << osc::BeginMessage("/sync");
        p << identifying;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::clearSched(){
        
        
        p << osc::BeginMessage("/clearSched");
        
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::error(int mode){
        
        
        p << osc::BeginMessage("/error");
        p << mode;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::version(){
        
        
        p << osc::BeginMessage("/version");
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::d_recv(const char * buffer, const char * completion){
        
        
        p << osc::BeginMessage("/d_recv");
        p << buffer;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::d_load(std::string pathname, const char * completion){
        
        
        p << osc::BeginMessage("/d_load");
        p << pathname;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::d_loadDir(std::string directory, const char * completion){
        
        
        p << osc::BeginMessage("/d_loadDir");
        p << directory;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::d_free(std::vector<std::string> synth){
        
        
        p << osc::BeginMessage("/d_free");
        foreach(auto v : synth){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_free(std::vector<int> node){
        
        
        p << osc::BeginMessage("/n_free");
        foreach(auto v : node){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_run(std::vector<std::tuple<int, int>> node){
        
        
        p << osc::BeginMessage("/n_run");
        foreach(auto v : node){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::n_set(int node, std::vector<std::tuple<std::variant<int ,  std::string>, std::variant<float ,  int>>> control){
        
        
        p << osc::BeginMessage("/n_set");
        p << node;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::n_setn(int node, std::vector<std::tuple<std::variant<int ,  std::string>, int, std::vector<std::variant<float ,  int>>>> control){
        
        
        p << osc::BeginMessage("/n_setn");
        p << node;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_fill(int node, std::vector<std::tuple<std::variant<int ,  std::string>, int, std::variant<float ,  int>>> control){
        
        
        p << osc::BeginMessage("/n_fill");
        p << node;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_map(int node, std::vector<std::tuple<std::variant<int ,  std::string>, int>> control){
        
        
        p << osc::BeginMessage("/n_map");
        p << node;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_mapn(int node, std::vector<std::tuple<std::variant<int ,  std::string>, int, int>> control){
        
        
        p << osc::BeginMessage("/n_mapn");
        p << node;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_mapa(int node, std::vector<std::tuple<std::variant<int ,  std::string>, int>> control){
        
        
        p << osc::BeginMessage("/n_mapa");
        p << node;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_mapan(int node, std::vector<std::tuple<std::variant<int ,  std::string>, int, int>> control){
        
        
        p << osc::BeginMessage("/n_mapan");
        p << node;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_before(std::vector<std::tuple<int, int>> place){
        
        
        p << osc::BeginMessage("/n_before");
        foreach(auto v : place){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_after(std::vector<std::tuple<int, int>> place){
        
        
        p << osc::BeginMessage("/n_after");
        foreach(auto v : place){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::n_query(std::vector<int> node){
        
        
        p << osc::BeginMessage("/n_query");
        foreach(auto v : node){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::n_trace(std::vector<int> node){
        
        
        p << osc::BeginMessage("/n_trace");
        foreach(auto v : node){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_order(int action, int target, std::vector<int> node){
        
        
        p << osc::BeginMessage("/n_order");
        p << action;
        p << target;
        foreach(auto v : node){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::s_new(std::string definition, int synth, int action, int target, std::vector<std::tuple<std::variant<int ,  std::string>, std::variant<float ,  int ,  std::string>>> control){
        
        
        p << osc::BeginMessage("/s_new");
        p << definition;
        p << synth;
        p << action;
        p << target;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::s_get(int synth, std::vector<std::variant<int ,  std::string>> control){
        
        
        p << osc::BeginMessage("/s_get");
        p << synth;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::s_getn(int synth, std::vector<std::tuple<std::variant<int ,  std::string>, int>> control){
        
        
        p << osc::BeginMessage("/s_getn");
        p << synth;
        foreach(auto v : control){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::s_noid(std::vector<int> synth){
        
        
        p << osc::BeginMessage("/s_noid");
        foreach(auto v : synth){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::g_new(std::vector<std::tuple<int, int, int>> group){
        
        
        p << osc::BeginMessage("/g_new");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::p_new(std::vector<std::tuple<int, int, int>> group){
        
        
        p << osc::BeginMessage("/p_new");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::g_head(std::vector<std::tuple<int, int>> group){
        
        
        p << osc::BeginMessage("/g_head");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::g_tail(std::vector<std::tuple<int, int>> group){
        
        
        p << osc::BeginMessage("/g_tail");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::g_freeAll(std::vector<int> group){
        
        
        p << osc::BeginMessage("/g_freeAll");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::g_deepFree(std::vector<int> group){
        
        
        p << osc::BeginMessage("/g_deepFree");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::g_dumpTree(std::vector<std::tuple<int, int>> group){
        
        
        p << osc::BeginMessage("/g_dumpTree");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::g_queryTree(std::vector<std::tuple<int, int>> group){
        
        
        p << osc::BeginMessage("/g_queryTree");
        foreach(auto v : group){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::u_cmd(int node, int generator, std::string command, ... arguments){
        
        
        p << osc::BeginMessage("/u_cmd");
        p << node;
        p << generator;
        p << command;
        p << arguments;
        
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_alloc(int buffer, int number, int channels, const char * completion){
        
        
        p << osc::BeginMessage("/b_alloc");
        p << buffer;
        p << number;
        p << channels;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_allocRead(int buffer, std::string sound, int starting, int number, const char * completion){
        
        
        p << osc::BeginMessage("/b_allocRead");
        p << buffer;
        p << sound;
        p << starting;
        p << number;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_allocReadChannel(int buffer, std::string sound, int starting, int number, std::vector<int> channel, const char * completion){
        
        
        p << osc::BeginMessage("/b_allocReadChannel");
        p << buffer;
        p << sound;
        p << starting;
        p << number;
        foreach(auto v : channel){p << v;}p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_read(int buffer, std::string sound, int starting, int number, int frame, int leave, const char * completion){
        
        
        p << osc::BeginMessage("/b_read");
        p << buffer;
        p << sound;
        p << starting;
        p << number;
        p << frame;
        p << leave;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_readChannel(int buffer, std::string sound, int starting, int number, int frame, int leave, std::vector<int> channel, const char * completion){
        
        
        p << osc::BeginMessage("/b_readChannel");
        p << buffer;
        p << sound;
        p << starting;
        p << number;
        p << frame;
        p << leave;
        foreach(auto v : channel){p << v;}p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_write(int buffer, std::string sound, std::string header, std::string sample, int number, int starting, int leave, const char * completion){
        
        
        p << osc::BeginMessage("/b_write");
        p << buffer;
        p << sound;
        p << header;
        p << sample;
        p << number;
        p << starting;
        p << leave;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_free(int buffer, const char * completion){
        
        
        p << osc::BeginMessage("/b_free");
        p << buffer;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_zero(int buffer, const char * completion){
        
        
        p << osc::BeginMessage("/b_zero");
        p << buffer;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::b_set(int buffer, std::vector<std::tuple<int, float>> sample){
        
        
        p << osc::BeginMessage("/b_set");
        p << buffer;
        foreach(auto v : sample){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::b_setn(int buffer, std::vector<std::tuple<int, int, std::vector<float>>> starting){
        
        
        p << osc::BeginMessage("/b_setn");
        p << buffer;
        foreach(auto v : starting){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::b_fill(int buffer, std::vector<std::tuple<int, int, float>> starting){
        
        
        p << osc::BeginMessage("/b_fill");
        p << buffer;
        foreach(auto v : starting){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_gen(int buffer, std::string command, ... arguments){
        
        
        p << osc::BeginMessage("/b_gen");
        p << buffer;
        p << command;
        p << arguments;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_close(int buffer, const char * completion){
        
        
        p << osc::BeginMessage("/b_close");
        p << buffer;
        p << completion;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_query(std::vector<int> buffer){
        
        
        p << osc::BeginMessage("/b_query");
        foreach(auto v : buffer){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_get(int buffer, std::vector<int> sample){
        
        
        p << osc::BeginMessage("/b_get");
        p << buffer;
        foreach(auto v : sample){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::b_getn(int buffer, std::vector<std::tuple<int, int>> starting){
        
        
        p << osc::BeginMessage("/b_getn");
        p << buffer;
        foreach(auto v : starting){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::c_set(std::vector<std::tuple<int, std::variant<float ,  int>>> index){
        
        
        p << osc::BeginMessage("/c_set");
        foreach(auto v : index){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::c_setn(std::vector<std::tuple<int, int, std::vector<std::variant<float ,  int>>>> starting){
        
        
        p << osc::BeginMessage("/c_setn");
        foreach(auto v : starting){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::c_fill(std::vector<std::tuple<int, int, std::variant<float ,  int>>> starting){
        
        
        p << osc::BeginMessage("/c_fill");
        foreach(auto v : starting){p << v;}
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::c_get(std::vector<int> index){
        
        
        p << osc::BeginMessage("/c_get");
        foreach(auto v : index){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::c_getn(std::vector<std::tuple<int, int>> starting){
        
        
        p << osc::BeginMessage("/c_getn");
        foreach(auto v : starting){p << v;}
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::nrt_end(){
        
        
        p << osc::BeginMessage("/nrt_end");
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    void SuperColliderCommander::n_go(){
        
        
        p << osc::BeginMessage("/n_go");
        
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_end(){
        
        
        p << osc::BeginMessage("/n_end");
        
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_off(){
        
        
        p << osc::BeginMessage("/n_off");
        
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_on(){
        
        
        p << osc::BeginMessage("/n_on");
        
        p << osc::EndMessage;
        Send();
    }
 
    void SuperColliderCommander::n_move(){
        
        
        p << osc::BeginMessage("/n_move");
        
        p << osc::EndMessage;
        Send();
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::n_info(){
        
        
        p << osc::BeginMessage("/n_info");
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
 
    std::future<osc::ReceivedMessage> SuperColliderCommander::tr(int node, int trigger, float value){
        
        
        p << osc::BeginMessage("/tr");
        p << node;
        p << trigger;
        p << value;
        
        p << osc::EndMessage;
        
        return SendRecieve();
    
    }
