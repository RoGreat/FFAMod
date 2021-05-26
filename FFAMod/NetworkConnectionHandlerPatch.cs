using HarmonyLib;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SoundImplementation;
using Photon.Pun;
using UnityEngine;
using Landfall.Network;

namespace FFAMod
{
    [HarmonyPatch(typeof(NetworkConnectionHandler))]
    internal class NetworkConnectionHandlerPatch
    {
        public static int PlayersNeededToStart = 4;

        /*
        [HarmonyTranspiler]
        [HarmonyPatch("HostPrivateAndInviteFriend")]
        private static IEnumerable<CodeInstruction> HostPrivateAndInviteFriendTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stfld && codes[i].operand is FieldInfo && (codes[i].operand as FieldInfo) == AccessTools.Field(typeof(RoomOptions), "MaxPlayers"))
                {
                    codes[i - 1].opcode = OpCodes.Ldc_I4_4;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
        */

        private static RoomOptions options;

        [HarmonyPatch("CreateRoom")]
        private static void Prefix(ref RoomOptions roomOptions)
        {
            options = roomOptions;
            options.MaxPlayers = (byte)PlayersNeededToStart;
            Debug.Log("CreateRoom set max players: " + roomOptions.MaxPlayers);
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
                if (stage == 1 && codes[i].opcode == OpCodes.Bne_Un_S)
                {
                    codes[i].opcode = OpCodes.Blt_S;
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
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
                return;
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
            if (PhotonNetwork.IsMasterClient)
            {
                if (options != null)
                {
                    if (options.MaxPlayers != (byte)PlayersNeededToStart)
                    {
                        options.MaxPlayers = (byte)PlayersNeededToStart;
                        Debug.Log("Update set max players: " + options.MaxPlayers);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(LoadBalancingClient))]
    internal class LoadBalancingClientPatch
    {
        [HarmonyPatch("OpJoinRoom")]
        private static void Prefix(EnterRoomParams enterRoomParams)
        {
            NetworkConnectionHandlerPatch.PlayersNeededToStart = enterRoomParams.RoomOptions.MaxPlayers;
        }
    }
}

 