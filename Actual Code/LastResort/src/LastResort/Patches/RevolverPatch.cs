using HarmonyLib;

namespace LastResort.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal class RevolverPatch
    {
        [HarmonyPatch(nameof(Character.Awake))]
        [HarmonyPostfix]
        private static void AddRevolverBlowbackWatcher(Character __instance)
        {
            __instance.gameObject.AddComponent<RevolverBlowbackWatcher>();
        }

    }
}
