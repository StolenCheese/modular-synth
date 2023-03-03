using System.Runtime.InteropServices;
using SynthAPI;
using modular_synth_frontend.UI;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
namespace modular_synth_frontend.API;

public static class API {

    public static bool enableAPI = false;

    public static string absPathToSynthDefs;

    //relative to modular_synth_frontend
    private const string relPathToSynthDefs =  @"..\modular-synth-backend\synthdefs";
    
    public static Dictionary<int, SCSection> synths = new Dictionary<int, SCSection>(); 

    static Process scProcess = new Process();

    public static void connectToSCServer(){

        Console.WriteLine("Connecting to SC Server...");

        if(!SCController.Connect("127.0.0.1", 58000)){
            cleanup();
            throw new Exception("failed to connect to server");
        }

        //Server prints all osc commands it recieves (debug)
        SCController.DumpOSC(1);
        
    }

    public static void startSCServer(){
        if(enableAPI){
                absPathToSynthDefs = Path.GetFullPath(relPathToSynthDefs)+"\\";

                string pathToSC = absPathToSynthDefs.Substring(0,2) + @"\Program Files\SuperCollider-3.13.0\scsynth.exe";
                //string command = "cd " + pathToSC + " && scsynth.exe -u 58000cd";

            Console.WriteLine("Starting SuperCollider Process");
            try{
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
                        Console.WriteLine("SuperCollider Output: " + e.Data);

                        if(e.Data=="SuperCollider 3 server ready."){

                            //connect to server we started
                            connectToSCServer();
                        }
                    }
                });

                scProcess.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine("SuperCollider Error: " + e.Data);
                    }
                    cleanup();
                });

                scProcess.Start();

                scProcess.BeginOutputReadLine();
                scProcess.BeginErrorReadLine();

                Console.WriteLine("SuperCollider process started");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " Do you have SuperCollider-3.13.0 in Program Files Folder?");
                cleanup();
            }
        }
    }
        


    public static void OnProcessExit (object sender, EventArgs e){
        cleanup();
    } 

    static void cleanup(){
        try{
            Console.WriteLine("killing sc server");
            scProcess.Kill();
            scProcess.Dispose(); //release any resources related to process

        } catch(InvalidOperationException e){
            Console.WriteLine(e.Message+". Cleanup not run");
        }
    }

    public static void createSection(Module m){
        if(enableAPI){
            if(m.scSection==null){
            Console.WriteLine("attempting section creation");
            m.scSection = SCSection.FromSynthdef(absPathToSynthDefs+m.function+".scsyndef",null);

            synths[m.ModuleId] = m.scSection;

            Console.WriteLine($"Created new synth {m.scSection} (ID:{m.ModuleId}) with controls [{System.String.Join(',', m.scSection.controls)}]");
            } else{
                Console.WriteLine("attempted section recreation of module with section!");
            }
        }
    }

    public static bool linkPorts(Port portFrom, Port portTo){
        if(enableAPI){
            Console.WriteLine($"portFrom: {portFrom.parentModuleId}.{portFrom.parameterID},portTo: {portTo.parentModuleId}.{portTo.parameterID}");
            try{
                synths[portFrom.parentModuleId].getPortFor(portFrom.parameterID).linkTo(synths[portTo.parentModuleId].getPortFor(portTo.parameterID));
                Console.WriteLine("connection made");
                return true;
            } catch(SynthAPI.CyclicLinksException_t e ){
                Console.WriteLine(e.Message);
                return false;
            }
        } else {
            return false;
        }
    }
    public static void unlinkPorts(Port portFrom, Port portTo){
        if(enableAPI){
            Console.WriteLine($"portFrom: {portFrom.parentModuleId}.{portFrom.parameterID},portTo: {portTo.parentModuleId}.{portTo.parameterID}");

            synths[portFrom.parentModuleId].getPortFor(portFrom.parameterID).removeLink(synths[portTo.parentModuleId].getPortFor(portTo.parameterID));

            Console.WriteLine("connection removed");
        }
    }


    public static void setValue(int modueleID,string property,float value){
        if(enableAPI) {
            //Console.WriteLine($"params: ID:{modueleID},property:{property},value:{value}");
            if(property!=null){
                synths[modueleID].Set(property, value);
            } else {
                Console.WriteLine("Tried setValue API call with null property!");
            }
        }
    }

    

    public const string CppFunctionsDLL = @"..\..\..\..\DLL-Linking-Test\x64\Debug\DLL-Linking-Test";  //string containing location of the DLL. Note that this is relative to the location of the modular-synth-frontend.exe file

    [DllImport(CppFunctionsDLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] //must be 1 to 1 match with cpp func name
    public static extern int addNumbers(int a, int b);

    [DllImport(CppFunctionsDLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)] 
    public static extern int stringInputTest([MarshalAs(UnmanagedType.LPStr)]string data);

}