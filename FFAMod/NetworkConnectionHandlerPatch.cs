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
using System;
using System.Collections;

namespace FFAMod
{
    [HarmonyPatch(typeof(NetworkConnectionHandler))]
    internal class NetworkConnectionHandlerPatch : NetworkConnectionHandler
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

        [HarmonyPatch("CreateRoom")]
        private static bool Prefix(RoomOptions roomOptions, ClientSteamLobby ___m_SteamLobby)
        {
            roomOptions.MaxPlayers = (byte)PlayersNeededToStart;
            UnityEngine.Debug.Log("roomOptions MaxPlayers " + roomOptions.MaxPlayers);
            ___m_SteamLobby.CreateLobby(roomOptions.MaxPlayers, delegate (string RoomName)
            {
                PhotonNetwork.CreateRoom(RoomName, roomOptions, null, null);
            });
            return false;
        }

        [HarmonyPatch("OnPlayerEnteredRoom")]
        private static bool Prefix(Photon.Realtime.Player newPlayer, ClientSteamLobby ___m_SteamLobby)
        {
            SoundPlayerStatic.Instance.PlayPlayerAdded();
            PlayersNeededToStart = PhotonNetwork.CurrentRoom.MaxPlayers;
            UnityEngine.Debug.Log("CurrentRoom MaxPlayers " + PlayersNeededToStart);
            if (PhotonNetwork.PlayerList.Length == PlayersNeededToStart)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    instance.GetComponent<PhotonView>().RPC("RPCA_FoundGame", RpcTarget.All, new object[] { });
                }
                if (___m_SteamLobby != null)
                {
                    ___m_SteamLobby.HideLobby();
                }
            }
            UnityEngine.Debug.Log("PlayerJoined");
            instance.OnPlayerEnteredRoom(newPlayer);
            return false;
        }

        /*
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
        */

        /*
        [HarmonyPatch("OnPlayerEnteredRoom")]
        private static bool Prefix(NetworkConnectionHandler __instance, Photon.Realtime.Player newPlayer)
        {
            if (PhotonNetwork.PlayerList.Length >= PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                return true;
            }
            else
            {
                SoundPlayerStatic.Instance.PlayPlayerAdded();
                UnityEngine.Debug.Log("PlayerJoined");
                __instance.OnPlayerEnteredRoom(newPlayer);
                return false;
            }
        }
        */

        [HarmonyPatch("Update")]
        private static void Postfix()
        {
            if (PhotonNetwork.IsMasterClient)
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
    /*
    [HarmonyPatch(typeof(LoadBalancingClient))]
    internal class LoadBalancingClientPatch
    {
        [HarmonyPatch("OpJoinRoom")]
        private static void Prefix(EnterRoomParams enterRoomParams)
        {
            if (enterRoomParams != null)
            {
                if (enterRoomParams.RoomOptions != null)
                {
                    NetworkConnectionHandlerPatch.PlayersNeededToStart = enterRoomParams.RoomOptions.MaxPlayers;
                    UnityEngine.Debug.Log("OpJoinRoom max players set to: " + enterRoomParams.RoomOptions.MaxPlayers);
                }
            }
        }
    }
    */
}

 