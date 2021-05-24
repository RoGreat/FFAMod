using HarmonyLib;

namespace FFAMod
{
    [HarmonyPatch(typeof(CharacterSelectionInstance))]
    internal class CharacterSelectionInstancePatch
    {
        [HarmonyPatch("Update")]
        private static void Prefix(CharacterSelectionInstance __instance)
        {
            if (!__instance.currentPlayer)
            {
                return;
            }
            if (__instance.currentPlayer.GetComponent<PlayerAPI>().enabled)
            {
                AccessTools.Method(typeof(CharacterSelectionInstance), "ReadyUp").Invoke(__instance, null);
            }
        }
    }
}
