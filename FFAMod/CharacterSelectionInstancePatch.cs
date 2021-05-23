using HarmonyLib;

namespace FFAMod
{
    [HarmonyPatch(typeof(CharacterSelectionInstance))]
    internal class CharacterSelectionInstancePatch
    {
        [HarmonyPatch("Update")]
        private static void Prefix(CharacterSelectionInstance __instance, Player ___currentPlayer)
        {
            if (!___currentPlayer)
            {
                return;
            }
            if (___currentPlayer.GetComponent<PlayerAPI>().enabled == true)
            {
                AccessTools.Method(typeof(CharacterSelectionInstance), "ReadyUp").Invoke(__instance, null);
            }
        }
    }
}
