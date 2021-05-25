using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FFAMod
{
    [HarmonyPatch(typeof(NetworkConnectionHandler))]
    internal class NetworkConnectionHandlerPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch("HostPrivateAndInviteFriend")]
        private static IEnumerable<CodeInstruction> HostPrivateAndInviteFriendTranspiler(IEnumerable<CodeInstruction> instructions)
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

        [HarmonyTranspiler]
        [HarmonyPatch("OnPlayerEnteredRoom")]
        private static IEnumerable<CodeInstruction> OnPlayerEnteredRoomTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            int stage = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                if (stage == 0 && codes[i].opcode == OpCodes.Call && codes[i].operand is MethodInfo && (codes[i].operand as MethodInfo) == AccessTools.PropertyGetter(typeof(PhotonNetwork), "PlayerList"))
                {
                    stage++;
                }
                if (stage == 1 && codes[i].opcode == OpCodes.Ldc_I4_2)
                {
                    codes[i].opcode = OpCodes.Ldc_I4_4;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }
}
