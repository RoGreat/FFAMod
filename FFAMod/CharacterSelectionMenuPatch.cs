using HarmonyLib;
using Photon.Pun;

namespace FFAMod
{
    [HarmonyPatch(typeof(CharacterSelectionMenu))]
    internal class CharacterSelectionMenuPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("PlayerJoined")]
        private static bool Prefix2()
        {
            if (PhotonNetwork.OfflineMode)
                return true;
            return false;
        }
    }
}
