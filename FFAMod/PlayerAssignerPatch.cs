using HarmonyLib;
using InControl;
using Photon.Pun;
using SoundImplementation;
using System.Collections;
using UnityEngine;

namespace FFAMod 
{
    [HarmonyPatch(typeof(PlayerAssigner))]
    internal class PlayerAssignerPatch : PlayerAssigner
    {
        [HarmonyPatch("RPCM_RequestTeamAndPlayerID")]
        private static bool Prefix(int askingPlayer, ref bool ___waitingForRegisterResponse)
        {
            int count = PlayerManager.instance.players.Count;
            int num = count;
            instance.GetComponent<PhotonView>().RPC("RPC_ReturnPlayerAndTeamID", PhotonNetwork.CurrentRoom.GetPlayer(askingPlayer), new object[]
            {
                count,
                num
            });
            ___waitingForRegisterResponse = true;
            return false;
        }

        // AI bugs out when battle is going
        [HarmonyPatch("LateUpdate")]
        private static bool Prefix()
        {
            if (GameManager.instance.battleOngoing)
                return false;
            return true;
        }

        [HarmonyPatch("CreatePlayer")]
        private static bool Prefix(ref IEnumerator __result, InputDevice inputDevice, bool isAI = false)
        {
            __result = CreatePlayerPatch(inputDevice, isAI);
            return false;
        }
        private static IEnumerator CreatePlayerPatch(InputDevice inputDevice, bool isAI = false)
        {
            PlayerAssigner playerAssigner = instance;
            var waitingForRegisterResponse = AccessTools.Field(typeof(PlayerAssigner), "waitingForRegisterResponse");
            var hasCreatedLocalPlayer = AccessTools.Field(typeof(PlayerAssigner), "hasCreatedLocalPlayer");
            var playerIDToSet = AccessTools.Field(typeof(PlayerAssigner), "playerIDToSet");
            var teamIDToSet = AccessTools.Field(typeof(PlayerAssigner), "teamIDToSet");
            if ((bool)waitingForRegisterResponse.GetValue(instance))
                yield break;
            if (!PhotonNetwork.OfflineMode && (bool)hasCreatedLocalPlayer.GetValue(playerAssigner))
                yield break;
            if (playerAssigner.players.Count < playerAssigner.maxPlayers)
            {
                if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
                {
                    playerAssigner.GetComponent<PhotonView>().RPC("RPCM_RequestTeamAndPlayerID", RpcTarget.MasterClient, new object[] { PhotonNetwork.LocalPlayer.ActorNumber });
                    waitingForRegisterResponse.SetValue(instance, true);
                }
                while ((bool)waitingForRegisterResponse.GetValue(instance))
                    yield return null;
                if (!PhotonNetwork.OfflineMode)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        playerIDToSet.SetValue(playerAssigner, PlayerManager.instance.players.Count);
                        teamIDToSet.SetValue(playerAssigner, (int)playerIDToSet.GetValue(playerAssigner));
                    }
                }
                else
                {
                    playerIDToSet.SetValue(playerAssigner, PlayerManager.instance.players.Count);
                    teamIDToSet.SetValue(playerAssigner, (int)playerIDToSet.GetValue(playerAssigner));
                }
                hasCreatedLocalPlayer.SetValue(playerAssigner, true);
                SoundPlayerStatic.Instance.PlayPlayerAdded();
                Vector3 position = Vector3.up * 100f;
                CharacterData component = PhotonNetwork.Instantiate(playerAssigner.playerPrefab.name, position, Quaternion.identity).GetComponent<CharacterData>();
                if (isAI)
                {
                    GameObject original = playerAssigner.player1AI;
                    if (playerAssigner.players.Count > 0)
                        original = playerAssigner.player2AI;
                    component.GetComponent<CharacterData>().SetAI();
                    Instantiate(original, component.transform.position, component.transform.rotation, component.transform);
                }
                else
                {
                    if (inputDevice != null)
                    {
                        component.input.inputType = GeneralInput.InputType.Controller;
                        component.playerActions = PlayerActions.CreateWithControllerBindings();
                    }
                    else
                    {
                        component.input.inputType = GeneralInput.InputType.Keyboard;
                        component.playerActions = PlayerActions.CreateWithKeyboardBindings();
                    }
                    component.playerActions.Device = inputDevice;
                }
                playerAssigner.players.Add(component);
                // playerAssigner.RegisterPlayer(component, playerAssigner.teamIDToSet, playerAssigner.playerIDToSet);
                AccessTools.Method(typeof(PlayerAssigner), "RegisterPlayer").Invoke(playerAssigner, new object[] { component, teamIDToSet.GetValue(playerAssigner), playerIDToSet.GetValue(playerAssigner) });
                yield break;
            }
            yield break;
        }
    }
}
