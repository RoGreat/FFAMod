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
        [HarmonyPatch("CreatePlayer")]
        private static bool Prefix(ref IEnumerator __result, InputDevice inputDevice, bool isAI = false)
        {
            __result = CreatePlayerPatch(inputDevice, isAI);
            return false;
        }

        private static IEnumerator CreatePlayerPatch(InputDevice inputDevice, bool isAI = false)
        {
            PlayerAssigner playerAssigner = instance;
            bool waitingForRegisterResponse = (bool)AccessTools.Field(typeof(PlayerAssigner), "waitingForRegisterResponse").GetValue(instance);
            bool hasCreatedLocalPlayer = (bool)AccessTools.Field(typeof(PlayerAssigner), "hasCreatedLocalPlayer").GetValue(instance);
            int playerIDToSet = (int)AccessTools.Field(typeof(PlayerAssigner), "playerIDToSet").GetValue(instance);
            int teamIDToSet = (int)AccessTools.Field(typeof(PlayerAssigner), "teamIDToSet").GetValue(instance);
            if (!waitingForRegisterResponse && (PhotonNetwork.OfflineMode || !hasCreatedLocalPlayer) && playerAssigner.players.Count < playerAssigner.maxPlayers)
            {
                if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
                {
                    playerAssigner.GetComponent<PhotonView>().RPC("RPCM_RequestTeamAndPlayerID", RpcTarget.MasterClient, (object)PhotonNetwork.LocalPlayer.ActorNumber);
                    waitingForRegisterResponse = true;
                }
                while (waitingForRegisterResponse)
                    yield return null;
                if (!PhotonNetwork.OfflineMode)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        playerIDToSet = PlayerManager.instance.players.Count;
                        teamIDToSet = playerIDToSet;
                    }
                }
                else
                {
                    playerIDToSet = PlayerManager.instance.players.Count;
                    teamIDToSet = playerIDToSet;
                }
                hasCreatedLocalPlayer = true;
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
                AccessTools.Method(typeof(PlayerAssigner), "RegisterPlayer").Invoke(playerAssigner, new object[] { component, teamIDToSet, playerIDToSet });
            }
        }
    }
}
