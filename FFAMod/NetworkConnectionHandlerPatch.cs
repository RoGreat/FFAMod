using HarmonyLib;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Steamworks;
using Landfall.Network;

namespace FFAMod
{
    [HarmonyPatch(typeof(NetworkConnectionHandler))]
    internal class NetworkConnectionHandlerPatch
    {
        [HarmonyPatch("HostPrivateAndInviteFriend")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for(int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stfld && codes[i].operand is FieldInfo && (codes[i].operand as FieldInfo) == AccessTools.Field(typeof(RoomOptions), "MaxPlayers"))
                {
                    codes[i - 1].opcode = OpCodes.Ldc_I4_4;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(ClientSteamLobby))]
    internal class ClientSteamLobbyPatch
    {

        [HarmonyPatch("HideLobby")]
        private static void Postfix(ClientSteamLobby __instance)
        {
            SteamMatchmaking.SetLobbyJoinable(__instance.CurrentLobby, true);
        }
    }
}
