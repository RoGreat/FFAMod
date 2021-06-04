using HarmonyLib;

namespace FFAMod
{
    [HarmonyPatch(typeof(MapManager))]
    class MapManagerPatch
    {
        [HarmonyPatch("GetRandomMap")]
        private static void Postfix(string __result)
        {
            UnityEngine.Debug.Log("Current map: " + __result);
        }
    }
}
