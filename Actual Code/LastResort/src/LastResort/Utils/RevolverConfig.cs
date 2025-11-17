using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LastResort.Utils
{
    public  class RevolverConfig
    {
        public static ConfigEntry<int> maxRevolverShots;

        public static void AllRevolverConfigs(ConfigFile revolverConfigFile) { 
            maxRevolverShots = revolverConfigFile.Bind<int>("Revolver", "Max Shots", 4, "The maximum number of shots a revolver can have.");
        }
    }
}
