
using SynthAPI;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

SCController.Connect("127.0.0.1", 58000);

SCController.DumpOSC(1);

var synths = new Dictionary<int, SCSection>();

while (true)
{
	Console.Write(">>>");
	var command = Console.ReadLine()!.Split(' ');

	switch (command)
	{
		case ["new", String synth]:
			var s = new SCSection(synth);
			Console.WriteLine($"Created new synth i={s.index} with controls [{System.String.Join(',', s.controls)}]");
			synths[s.index] = s;

			break;

		case ["run", String sid, String run]:
			synths[int.Parse(sid)].Run(Boolean.Parse(run));

			break;

		case ["get", String sid, String param]:
			Console.WriteLine(synths[int.Parse(sid)].Get(param));

			break;

		case ["set", String sid, String param, String v]:
			synths[int.Parse(sid)].Set(param, float.Parse(v));

			break;
	};
}
