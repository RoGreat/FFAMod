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
        public static int PlayersNeededToStart { get; private set; } = 4;

        [HarmonyPatch("CreateRoom")]
        private static bool Prefix(RoomOptions roomOptions, ClientSteamLobby ___m_SteamLobby)
        {
            roomOptions.MaxPlayers = (byte)PlayersNeededToStart;
            UnityEngine.Debug.Log("CreateRoom MaxPlayers " + roomOptions.MaxPlayers);
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

        [HarmonyPatch("Update")]
        private static void Postfix()
        {
            if (MainMenuHandler.instance.isOpen)
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
}

 