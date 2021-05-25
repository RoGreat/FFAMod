using HarmonyLib;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SoundImplementation;
using Photon.Pun;
using UnityEngine;

namespace FFAMod
{
    [HarmonyPatch(typeof(NetworkConnectionHandler))]
    internal class NetworkConnectionHandlerPatch
    {
        public static int PlayersNeededToStart = 4;

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

        [HarmonyPatch("OnPlayerEnteredRoom")]
        private static bool Prefix(NetworkConnectionHandler __instance, Photon.Realtime.Player newPlayer)
        {
            if (PhotonNetwork.PlayerList.Length >= PlayersNeededToStart)
            {
                return true;
            }
            else
            {
                SoundPlayerStatic.Instance.PlayPlayerAdded();
                Debug.Log("PlayerJoined");
                __instance.OnPlayerEnteredRoom(newPlayer);
                return false;
            }
        }

        [HarmonyPatch("Update")]
        private static void Postfix()
        {
            if (Input.GetKey(KeyCode.Alpha4))
            {
                PlayersNeededToStart = 4;
                SoundPlayerStatic.Instance.PlayButtonClick();
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                PlayersNeededToStart = 3;
                SoundPlayerStatic.Instance.PlayButtonClick();
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                PlayersNeededToStart = 2;
                SoundPlayerStatic.Instance.PlayButtonClick();
            }
        }
    }
}
