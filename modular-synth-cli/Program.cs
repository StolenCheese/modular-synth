
using SynthAPI;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

if (!SCController.Connect("127.0.0.1", 58000))
	throw new Exception("Failed to connect");

SCController.DumpOSC(1);

var synths = new Dictionary<int, SCSection>();

while (true)
{
	// cd "C:\Program Files\SuperCollider-3.13.0-rc1"; ./scsynth.exe -u 58000
	Console.Write(">>>");
	var command = Console.ReadLine()!.Split(' ');
	SCSection s;
	switch (command)
	{

		case ["new", "midi", String midi, "with", String inst]:
			var path = "A:\\Documents\\synth\\midi\\" + midi + ".mid";
			s = SCSection.FromMidi(path);
			Console.WriteLine($"Loading from {path}");
			Console.WriteLine($"Created new synth i={s.index} with controls [{System.String.Join(',', s.controls)}]");
			synths[s.index] = s;

			var instPath = "A:\\Documents\\synth\\modular-synth\\modular-synth-backend\\synthdefs\\" + inst + ".scsyndef";
			foreach (var p in s.controls)
			{
				Console.WriteLine($"Loading for {p}");
				var i = SCSection.FromSynthdef(instPath, null);
				synths[i.index] = i;

				s.getPortFor(p).linkTo(i.getPortFor("freq"));
			}

			break;

		case ["new", "synth", String synth]:

			s = SCSection.FromSynthdef("D:\\REPOS\\modular-synth\\modular-synth-backend\\synthdefs\\" + synth + ".scsyndef", null);
			Console.WriteLine($"Created new synth i={s.index} with controls [{System.String.Join(',', s.controls)}]");
			synths[s.index] = s;


			break;

		case ["run", String sid, String run]:
			synths[int.Parse(sid)].Run(Boolean.Parse(run));

			break;

		case ["get", String sid, String param]:
			//Console.WriteLine(synths[int.Parse(sid)].Get(param));

			break;

		case ["set", String sid, String param, String v]:
			synths[int.Parse(sid)].Set(param, float.Parse(v));

			break;

		case ["connect", String sidSrc, String sidDst, String srcParam, String dstParam]:
			synths[int.Parse(sidSrc)].getPortFor(srcParam).linkTo(synths[int.Parse(sidDst)].getPortFor(dstParam));

			break;

		case ["disconnect", String sidSrc, String sidDst, String srcParam, String dstParam]:
			synths[int.Parse(sidSrc)].getPortFor(srcParam).removeLink(synths[int.Parse(sidDst)].getPortFor(dstParam));
			break;
	};
}
