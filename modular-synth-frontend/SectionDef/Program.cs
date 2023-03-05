using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
namespace SectionDefTest
{
    internal class Program
    {
        //input UIDef file path, SecDefFile path and CombinedFile as an path
        static public Dictionary<string, Dictionary<string,string>> combineSecUIDef(string UIDefFile, string SecDefFile, string CombinedFile) 
        {
            //read in UIDef and SecDef Files
            string jsonUI = File.ReadAllText(UIDefFile);
            Dictionary<string, Dictionary<string, string>> UIDefValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonUI);
            string jsonSec = File.ReadAllText(SecDefFile);
            Dictionary<string, Dictionary<string, string>> SecDefValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonSec);

            //check for each component in SecDef there is a matching component in UIDef
            foreach (var component in SecDefValues)
            {
                if (!UIDefValues.ContainsKey(component.Key))
                {
                    throw new InvalidOperationException("SecDef file contains parameters not found in UIDef File");
                }
            }

            Dictionary<string, Dictionary<string, string>> combinedFile = new Dictionary<string, Dictionary<string, string>>();
            foreach (var component in SecDefValues) //loop over SecDef as we do not mind if there are additional things in UIDef that we don't use
            {
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                //add parameters in both UI and Se Def for that component
                newDict = UIDefValues[component.Key];
                foreach (var a in component.Value)
                {
                    newDict.Add(a.Key, a.Value);
                }

                combinedFile.Add(component.Key, newDict);
            }

            return combinedFile;
        }
    }
    
}
