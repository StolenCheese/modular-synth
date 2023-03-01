// using System;
// Console.WriteLine(stringInputTest("t"));
using modular_synth_frontend.API;
API.connectToSCServer();

using var game = new modular_synth_frontend.Core.ModularSynth();
game.Run();
