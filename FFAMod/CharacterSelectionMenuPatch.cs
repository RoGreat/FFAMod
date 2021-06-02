using HarmonyLib;
using Photon.Pun;

namespace FFAMod
{
    [HarmonyPatch(typeof(CharacterSelectionMenu))]
    internal class CharacterSelectionMenuPatch
    {
        [HarmonyPatch("PlayerJoined")]
        private static bool Prefix()
        {
            if (PhotonNetwork.OfflineMode)
                return true;
            return false;
        }
    }
}
