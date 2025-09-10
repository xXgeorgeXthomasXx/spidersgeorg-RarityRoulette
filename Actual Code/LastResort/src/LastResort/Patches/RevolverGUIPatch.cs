using HarmonyLib;

namespace LastResort.Patches
{
    [HarmonyPatch(typeof(GUIManager))]
    internal class RevolverGUIPatch
    {
        [HarmonyPatch(nameof(GUIManager.LateUpdate))]
        [HarmonyPostfix]
        private static void AddAlwaysShowSecondaryUse(GUIManager __instance)
        {
            if ((bool)Character.localCharacter && (bool)Character.localCharacter.data.currentItem) {
                if (Character.localCharacter.data.currentItem.GetItemName() == "Revolver")
                {
                    __instance.itemPromptSecondary.gameObject.SetActive(true);
                    __instance.itemPromptSecondary.text = __instance.GetSecondaryInteractPrompt(Character.localCharacter.data.currentItem);
                }
            }
           
        }

    }
}
