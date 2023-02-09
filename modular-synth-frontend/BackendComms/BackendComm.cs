using System.Runtime.InteropServices;


namespace modular_synth_frontend.BackendComms;
public class BackendComm {
    public const string CppFunctionsDLL = @"commTestFuncs.dll";  //string containing location of the DLL

    [DllImport(CppFunctionsDLL, CallingConvention = CallingConvention.Cdecl)] //add attributes to C# functions
    public static extern int addNumbers(int a, int b);

}