using HarmonyLib;
using Photon.Realtime;
using SoundImplementation;
using Photon.Pun;
using UnityEngine;
using Landfall.Network;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;

namespace FFAMod
{
    [HarmonyPatch(typeof(NetworkConnectionHandler))]
    internal class NetworkConnectionHandlerPatch
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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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

        [HarmonyPatch("OnPlayerEnteredRoom")]
        private static bool Prefix(ClientSteamLobby ___m_SteamLobby)
        {
            PlayersNeededToStart = PhotonNetwork.CurrentRoom.MaxPlayers;
            if (PlayersNeededToStart == 4)
                return true;
            if (PhotonNetwork.PlayerList.Length == PlayersNeededToStart)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    NetworkConnectionHandler.instance.GetComponent<PhotonView>().RPC("RPCA_FoundGame", RpcTarget.All, new object[] { });
                }
                if (___m_SteamLobby != null)
                {
                    ___m_SteamLobby.HideLobby();
                }
            }
            return true;
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

 