using System.Runtime.InteropServices;
using SynthAPI;
using modular_synth_frontend.UI;
using System.Collections.Generic;
using System;
using System.IO;
namespace modular_synth_frontend.API;

public static class API {

    public static string absPathToSynthDefs;

    //relative to modular_synth_frontend
    private const string relPathToSynthDefs =  @"..\modular-synth-backend\synthdefs";
    
    public static Dictionary<int, SCSection> synths = new Dictionary<int, SCSection>(); 

    public static void connectToSCServer(){

        Console.WriteLine("Connecting to SC Server...");

        SCController.Connect("127.0.0.1", 58000);

        //Server prints all osc commands it recieves (debug)
        SCController.DumpOSC(1);
         
        absPathToSynthDefs = Path.GetFullPath(relPathToSynthDefs)+"\\";
    }

    public static void createSection(Module m){
        if(m.scSection==null){
            Console.WriteLine("attempting section creation");
            m.scSection = new SCSection(absPathToSynthDefs+m.function+".scsyndef");

            synths[m.ModuleId] = m.scSection;

            Console.WriteLine($"Created new synth {m.scSection} (ID:{m.ModuleId}) with controls [{System.String.Join(',', m.scSection.controls)}]");
        } else{
            Console.WriteLine("attempted section recreation of module with section!");
        }
        
    }

    public static void linkPorts(Port portFrom, Port portTo){
        //synths[int.Parse(sidSrc)].getPortFor(srcParam).linkTo(synths[int.Parse(sidDst)].getPortFor(dstParam));
    }

    public static void setValue(int modueleID,string property,float value){
        //Console.WriteLine($"params: ID:{modueleID},property:{property},value:{value}");
        if(value!=null&&property!=null&&modueleID!=null){
            synths[modueleID].Set(property, value);
        } else {
            
        }
    }

    

    public const string CppFunctionsDLL = @"..\..\..\..\DLL-Linking-Test\x64\Debug\DLL-Linking-Test";  //string containing location of the DLL. Note that this is relative to the location of the modular-synth-frontend.exe file

    [DllImport(CppFunctionsDLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] //must be 1 to 1 match with cpp func name
    public static extern int addNumbers(int a, int b);

    [DllImport(CppFunctionsDLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] 
    public static extern int stringInputTest([MarshalAs(UnmanagedType.LPStr)]string data);

}