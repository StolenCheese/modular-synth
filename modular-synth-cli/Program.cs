
using SynthAPI;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

SCController.Connect("127.0.0.1", 58000);


while (true)
{
	var command = Console.ReadLine()!.Split(' ');
	switch (command)
	{
		case ["new", String synth]:
			var s = new SCSection(synth);
			Console.WriteLine($"Created new synth");

			break;
	};
}
