
using SynthAPI;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

SCController.Connect("127.0.0.1", 58000);

var s = new SCSection("sin-ar");

System.Threading.Thread.Sleep(1000);

s.Run(false);