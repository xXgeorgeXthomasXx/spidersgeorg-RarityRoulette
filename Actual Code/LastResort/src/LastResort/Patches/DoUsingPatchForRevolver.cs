using HarmonyLib;
using UnityEngine.TextCore.Text;

namespace LastResort.Patches
{
    [HarmonyPatch(typeof(CharacterItems))]
    internal class DoUsingPatchForRevolver
    {
        [HarmonyPatch(nameof(CharacterItems.DoUsing))]
        [HarmonyPrefix]
        private static bool RevolverDoUsing(CharacterItems __instance)
        {

           if ((bool)__instance.character.data.currentItem && !__instance.character.data.passedOut && !__instance.character.data.fullyPassedOut)
            {
                if (__instance.character.input.usePrimaryWasPressed && __instance.character.data.currentItem.CanUsePrimary())
                {
                    __instance.character.data.currentItem.StartUsePrimary();
                }
                if (__instance.character.input.usePrimaryIsPressed && __instance.character.data.currentItem.CanUsePrimary())
                {
                    __instance.character.data.currentItem.ContinueUsePrimary();
                }
                if (__instance.character.input.usePrimaryWasReleased || (__instance.character.data.currentItem.isUsingPrimary && !__instance.character.data.currentItem.CanUsePrimary()))
                {
                    __instance.character.data.currentItem.CancelUsePrimary();
                }
                if (!__instance.character.CanDoInput())
                {
                    __instance.character.data.currentItem.CancelUsePrimary();
                }
                if (__instance.character.data.currentItem.GetName() == "Revolver")
                {
                    if (__instance.character.input.useSecondaryIsPressed)
                    {
                        __instance.character.data.currentItem.StartUseSecondary();
                    }
                    
                    if (__instance.character.input.useSecondaryIsPressed)
                    {
                        __instance.character.data.currentItem.ContinueUseSecondary();
                    }
                    
                    if (__instance.character.input.useSecondaryWasReleased )
                    {
                        __instance.character.data.currentItem.CancelUseSecondary();
                    }
                }
                else if(__instance.character.data.currentItem.GetName() != "Revolver") {
                    if (__instance.character.input.useSecondaryIsPressed && __instance.character.data.currentItem.CanUseSecondary())
                    {
                        __instance.character.data.currentItem.StartUseSecondary();
                    }
                    if (__instance.character.input.useSecondaryIsPressed && __instance.character.data.currentItem.CanUseSecondary())
                    {
                        __instance.character.data.currentItem.ContinueUseSecondary();
                    }
                    if (__instance.character.input.useSecondaryWasReleased || (__instance.character.data.currentItem.isUsingSecondary && !__instance.character.data.currentItem.CanUseSecondary()))
                    {
                        __instance.character.data.currentItem.CancelUseSecondary();
                    }
                }
                if (__instance.character.input.scrollBackwardWasPressed)
                {
                    __instance.character.data.currentItem.ScrollButtonBackwardPressed();
                }
                if (__instance.character.input.scrollForwardWasPressed)
                {
                    __instance.character.data.currentItem.ScrollButtonForwardPressed();
                }
                if (__instance.character.input.scrollBackwardIsPressed)
                {
                    __instance.character.data.currentItem.ScrollButtonBackwardHeld();
                }
                if (__instance.character.input.scrollForwardIsPressed)
                {
                    __instance.character.data.currentItem.ScrollButtonForwardHeld();
                }
                if (__instance.character.input.scrollInput != 0f)
                {
                    __instance.character.data.currentItem.Scroll(__instance.character.input.scrollInput);
                }
            }
            return false;
        }
    }
}