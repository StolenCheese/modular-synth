using modular_synth_frontend.UI;
using SynthAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
namespace modular_synth_frontend.API;

public static class API
{

	public const bool enableAPI = true;
	public static bool connected = false;

	public static string absPathToSynthDefs;

	//relative to modular_synth_frontend
	private const string relPathToSynthDefs = "..\\..\\..\\..\\modular-synth-backend\\synthdefs";

	public static Dictionary<int, SCSection> synths = new();

	static readonly Process scProcess = new();

	public static void ConnectToSCServer()
	{

		Console.WriteLine("Connecting to SC Server...");

		if (!SCController.Connect("127.0.0.1", 58000))
		{
			Cleanup();
			throw new Exception("Failed to connect to server");
		}
		else
		{
			connected = true;

			//Server prints all osc commands it receives (debug)
			SCController.DumpOSC(1);
		}



	}

	public static void StartSCServer()
	{

		absPathToSynthDefs = Path.GetFullPath(relPathToSynthDefs) + "\\";

		string pathToSC = @"C:\Program Files\SuperCollider-3.13.0\scsynth.exe";
		if (!File.Exists(pathToSC))
		{
			pathToSC = absPathToSynthDefs[2..] + @"\Program Files\SuperCollider-3.13.0\scsynth.exe";
		}


		//string command = "cd " + pathToSC + " && scsynth.exe -u 58000cd";

		Debug.WriteLine("Starting SuperCollider Process");
		try
		{
			//kill old process if still running
			foreach (var process in Process.GetProcessesByName("scsynth"))
			{
				process.Kill();
			}

			// Start the SuperCollider process
			scProcess.StartInfo.FileName = pathToSC;
			scProcess.StartInfo.Arguments = "-u 58000cd";
			scProcess.StartInfo.UseShellExecute = false;
			scProcess.StartInfo.RedirectStandardOutput = true;
			scProcess.StartInfo.RedirectStandardError = true;

			scProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					Debug.WriteLine("SuperCollider Output: " + e.Data);

					if (e.Data == "SuperCollider 3 server ready.")
					{

						//connect to server we started
						ConnectToSCServer();
					}
				}
			});

			scProcess.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					Debug.WriteLine("SuperCollider Error: " + e.Data);
				}
				Cleanup();
			});

			scProcess.Start();

			scProcess.BeginOutputReadLine();
			scProcess.BeginErrorReadLine();

			Debug.WriteLine("SuperCollider process started");

		}
		catch (Exception e)
		{
			Debug.WriteLine(e.Message + " Do you have SuperCollider-3.13.0 in Program Files Folder?");
			Cleanup();
		}
	}



	public static void OnProcessExit(object sender, EventArgs e)
	{
		Cleanup();
	}

	static void Cleanup()
	{
		try
		{
			Debug.WriteLine("Killing SC server");
			scProcess.Kill();
			scProcess.Dispose(); //release any resources related to process

			//kill old process if still running. Shouldnt be necessary here!
			foreach (var process in Process.GetProcessesByName("scsynth"))
			{
				process.Kill();
			}

		}
		catch (InvalidOperationException e)
		{
			Debug.WriteLine(e.Message + ". Cleanup not run");
		}
	}

	public static void CreateSection(Module m)
	{
		if (!connected)
			throw new Exception("Not connected to server");

		if (enableAPI)
		{
			if (m.scSection == null)
			{
				Debug.WriteLine("attempting section creation");
				m.scSection = SCSection.FromSynthdef(absPathToSynthDefs + m.function + ".scsyndef", null);

				synths[m.ModuleId] = m.scSection;

				Debug.WriteLine($"Created new synth {m.scSection} (ID:{m.ModuleId}) with controls [{String.Join(',', m.scSection.controls)}]");
			}
			else
			{
				Debug.WriteLine("attempted section recreation of module with section!");
			}
		}
	}

	public static bool LinkPorts(Port portFrom, Port portTo)
	{
		if (!connected)
			throw new Exception("Not connected to server");

		Debug.WriteLine($"portFrom: {portFrom.parentModuleId}.{portFrom.parameterID},portTo: {portTo.parentModuleId}.{portTo.parameterID}");
		try
		{
			synths[portFrom.parentModuleId].getPortFor(portFrom.parameterID).linkTo(synths[portTo.parentModuleId].getPortFor(portTo.parameterID));
			Debug.WriteLine("connection made");
			return true;
		}
		catch (CyclicLinksException_t e)
		{
			Debug.WriteLine(e.Message);
			return false;
		}
		catch (LinkException_t e)
		{
			Debug.WriteLine(e.Message);
			return false;
		}

	}

	public static bool UnlinkPorts(Port portFrom, Port portTo)
	{
		if (!connected)
			throw new Exception("Not connected to server");

		if (enableAPI)
		{
			Console.WriteLine($"portFrom: {portFrom.parentModuleId}.{portFrom.parameterID},portTo: {portTo.parentModuleId}.{portTo.parameterID}");
			try
			{
				synths[portTo.parentModuleId].getPortFor(portTo.parameterID).removeLink(synths[portFrom.parentModuleId].getPortFor(portFrom.parameterID));
				Console.WriteLine("connection removed");
				return true;
			}
			catch (SynthAPI.NoSuchConnectionException_t e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}
		return false;
	}


	public static void SetValue(int modueleID, string property, float value)
	{
		if (enableAPI)
		{
			//Console.WriteLine($"params: ID:{modueleID},property:{property},value:{value}");
			if (property != null)
			{
				synths[modueleID].Set(property, value);
			}
			else
			{
				Console.WriteLine("Tried setValue API call with null property!");
			}
		}
	}


	// public static float getValue(int modueleID,string property){
	//     if(enableAPI) {
	//         //Console.WriteLine($"params: ID:{modueleID},property:{property},value:{value}");
	//         if(property!=null){
	//             return synths[modueleID].Get(property);
	//         } else {
	//             Console.WriteLine("Tried getValue API call with null property!");
	//             return 0;
	//         }
	//     }
	//     return 0;
	// }


}