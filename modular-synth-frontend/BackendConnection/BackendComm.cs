using System.Runtime.InteropServices;


namespace modular_synth_frontend.BackendComms;
public class BackendComm {
    public const string CppFunctionsDLL = @"..\..\..\..\DLL-Linking-Test\x64\Debug\DLL-Linking-Test";  //string containing location of the DLL. Note that this is relative to the location of the modular-synth-frontend.exe file

    [DllImport(CppFunctionsDLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] //must be 1 to 1 match with cpp func name
    public static extern int addNumbers(int a, int b);

    [DllImport(CppFunctionsDLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] 
    public static extern int stringInputTest([MarshalAs(UnmanagedType.LPStr)]string data);

}