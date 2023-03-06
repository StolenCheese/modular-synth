// Console.WriteLine(stringInputTest("t"));
using modular_synth_frontend.API;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;

var t = SectionDefManager.LoadSectionDef("sin-ui");




if (API.enableAPI)
{
	AppDomain.CurrentDomain.ProcessExit += new EventHandler(API.OnProcessExit);
	API.StartSCServer();
}
using var game = new modular_synth_frontend.Core.ModularSynth();
game.Run();
