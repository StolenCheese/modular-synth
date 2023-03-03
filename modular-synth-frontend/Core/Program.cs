using System;
// Console.WriteLine(stringInputTest("t"));
using modular_synth_frontend.API;
if(API.enableAPI){
    API.connectToSCServer();
    AppDomain.CurrentDomain.ProcessExit += new EventHandler (API.OnProcessExit);
}
using var game = new modular_synth_frontend.Core.ModularSynth();
game.Run();
