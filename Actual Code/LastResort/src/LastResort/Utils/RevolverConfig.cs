using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LastResort.Utils
{
    public  class RevolverConfig
    {
        public static ConfigEntry<int> maxRevolverShots;
        public static ConfigEntry<float> revolverDamage;
        public static ConfigEntry<float> revolverBlowback;

        public static void AllRevolverConfigs(ConfigFile revolverConfigFile) { 
            maxRevolverShots = revolverConfigFile.Bind<int>("Revolver", "Max Shots", 4, "The maximum number of shots a revolver can have.");
            revolverDamage = revolverConfigFile.Bind<float>("Revolver", "Revolver Damage", 0.5f, "The amount of damage you/a friend takes upon an unsuccessful shot of the revolver. Make this a value somewhere between 0.0 for no damage, to 1.0 for maximum damage!");
            revolverBlowback = revolverConfigFile.Bind<float>("Revolver", "Revolver Blowback", 0.25f, "How far back you/a friend goes flying upon an unsuccessful shot of the revolver. Make this a value somewhere between 0.0 for no blowback, to 1.0 for a lot of blowback! You can of course go higher, but beware it has not been tested :) "); 
        }
    }
}
