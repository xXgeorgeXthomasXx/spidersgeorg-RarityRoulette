using HarmonyLib;


namespace LastResort.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal static class CharacterAwake
    {
        [HarmonyPatch(nameof(Character.Awake))]
        [HarmonyPostfix]
        private static void AddSFXManagerForRevolver(Character __instance)
        { 
            __instance.gameObject.AddComponent<SFXManagerForRevolver>();
        }
    }
}
