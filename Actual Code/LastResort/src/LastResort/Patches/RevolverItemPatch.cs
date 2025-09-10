using HarmonyLib;
using UnityEngine;

namespace LastResort.Patches
{
    [HarmonyPatch(typeof(Item))]
    internal class RevolverItemPatch
    {
        [HarmonyPatch(nameof(Item.ContinueUseSecondary))]
        [HarmonyPrefix]
        private static bool ContinueSecondaryCastRevolver(Item __instance)
        {                
            if (!__instance.isUsingPrimary && __instance.isUsingSecondary) {
                    
                if (__instance.usingTimePrimary > 0f) {
                        
                    if (__instance.castProgress >= 1f) {
                        if (!__instance.finishedCast) {
                            if (__instance.GetName() == "Revolver")
                            {
                                __instance.GetComponent<Action_Revolver_Self>().RunAction();
                            }
                            __instance.FinishCastSecondary();
                        }
                    }
                }
            }  
                
            
            if (__instance.isUsingPrimary || !__instance.isUsingSecondary)
            {
                return false;
            }
            if (__instance.usingTimePrimary > 0f)
            {
                __instance.castProgress += 1f / __instance.totalSecondaryUsingTime * Time.deltaTime;
                if (__instance.castProgress >= 1f)
                {
                    if (__instance.OnSecondaryHeld != null)
                    {
                        
                        __instance.OnSecondaryHeld();
                    }
                    
                }
            }
            else if (__instance.OnSecondaryHeld != null)
            {
                
                __instance.OnSecondaryHeld();
            }
            return false;
        }
    }
}