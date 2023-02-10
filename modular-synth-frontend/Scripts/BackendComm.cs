using System.Runtime.InteropServices;


namespace modular_synth_frontend.BackendComms;
public class BackendComm {
    public const string CppFunctionsDLL = @"..\..\..\..\DLL-Linking-Test\x64\Debug\DLL-Linking-Test";  //string containing location of the DLL. Note that this runs from the location of the modular-synth-frontend.exe file

    [DllImport(CppFunctionsDLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] //add attributes to C# functions
    public static extern int addNumbers(int a, int b);
}