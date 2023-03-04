using System;
// Console.WriteLine(stringInputTest("t"));
using modular_synth_frontend.API;
if(API.enableAPI){
    AppDomain.CurrentDomain.ProcessExit += new EventHandler (API.OnProcessExit);
    API.startSCServer();
}
using var game = new modular_synth_frontend.Core.ModularSynth();
game.Run();
