using HarmonyLib;
using InControl;
using Photon.Pun;
using SoundImplementation;
using System.Collections;
using UnityEngine;

namespace FFAMod 
{
    [HarmonyPatch(typeof(PlayerAssigner))]
    internal class PlayerAssignerPatch
    {
        [HarmonyPatch("RPCM_RequestTeamAndPlayerID")]
        private static bool Prefix(int askingPlayer, ref bool ___waitingForRegisterResponse)
        {
            int count = PlayerManager.instance.players.Count;
            int num = count;
            PlayerAssigner.instance.GetComponent<PhotonView>().RPC("RPC_ReturnPlayerAndTeamID", PhotonNetwork.CurrentRoom.GetPlayer(askingPlayer), new object[]
            {
                count,
                num
            });
            ___waitingForRegisterResponse = true;
            return false;
        }

        // AI bugs out when battle is going
        [HarmonyPatch("LateUpdate")]
        private static bool Prefix(PlayerAssigner __instance)
        {
            if (GameManager.instance.battleOngoing && GM_Test.instance == null)
                return false;
            if (!PhotonNetwork.OfflineMode)
            {
                bool flag = true;
                for (int i = 0; i < __instance.players.Count; i++)
                {
                    if (__instance.players[i].playerActions.Device == null)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    __instance.StartCoroutine(__instance.CreatePlayer(null, false));
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch("CreatePlayer")]
        private static bool Prefix(ref IEnumerator __result, InputDevice inputDevice, bool isAI = false)
        {
            __result = CreatePlayer(inputDevice, isAI);
            return false;
        }

        private static IEnumerator CreatePlayer(InputDevice inputDevice, bool isAI)
        {
            yield return new WaitForSecondsRealtime(PhotonNetwork.LocalPlayer.ActorNumber);
            UnityEngine.Debug.Log("Creating Player");
            var instance = PlayerAssigner.instance;
            var waitingForRegisterResponse = AccessTools.Field(typeof(PlayerAssigner), "waitingForRegisterResponse");
            var hasCreatedLocalPlayer = AccessTools.Field(typeof(PlayerAssigner), "hasCreatedLocalPlayer");
            var playerIDToSet = AccessTools.Field(typeof(PlayerAssigner), "playerIDToSet");
            var teamIDToSet = AccessTools.Field(typeof(PlayerAssigner), "teamIDToSet");
            if ((bool)waitingForRegisterResponse.GetValue(instance))
            {
                UnityEngine.Debug.Log("Waiting for register response and not creating player");
                yield break;
            }
            if (!PhotonNetwork.OfflineMode && (bool)hasCreatedLocalPlayer.GetValue(instance))
                yield break;
            UnityEngine.Debug.Log("CreatePlayer maxPlayers " + instance.maxPlayers);
            if (instance.players.Count < instance.maxPlayers)
            {
                if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
                {
                    PlayerAssigner.instance.GetComponent<PhotonView>().RPC("RPCM_RequestTeamAndPlayerID", RpcTarget.MasterClient, new object[] { PhotonNetwork.LocalPlayer.ActorNumber });
                    waitingForRegisterResponse.SetValue(instance, true);
                    UnityEngine.Debug.Log("Waiting for register response");
                }
                while ((bool)waitingForRegisterResponse.GetValue(instance))
                    yield return null;
                UnityEngine.Debug.Log("No longer waiting for register response");
                if (!PhotonNetwork.OfflineMode)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        playerIDToSet.SetValue(instance, PlayerManager.instance.players.Count);
                        teamIDToSet.SetValue(instance, (int)playerIDToSet.GetValue(instance));
                    }
                }
                else
                {
                    playerIDToSet.SetValue(instance, PlayerManager.instance.players.Count);
                    teamIDToSet.SetValue(instance, (int)playerIDToSet.GetValue(instance));
                }
                hasCreatedLocalPlayer.SetValue(instance, true);
                SoundPlayerStatic.Instance.PlayPlayerAdded();
                Vector3 position = Vector3.up * 100f;
                CharacterData component = PhotonNetwork.Instantiate(instance.playerPrefab.name, position, Quaternion.identity).GetComponent<CharacterData>();
                if (isAI)
                {
                    GameObject original = instance.player2AI;
                    component.GetComponent<CharacterData>().SetAI();
                    Object.Instantiate(original, component.transform.position, component.transform.rotation, component.transform);
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
                instance.players.Add(component);
                // playerAssigner.RegisterPlayer(component, playerAssigner.teamIDToSet, playerAssigner.playerIDToSet);
                AccessTools.Method(typeof(PlayerAssigner), "RegisterPlayer").Invoke(instance, new object[] { component, teamIDToSet.GetValue(instance), playerIDToSet.GetValue(instance) });
                UnityEngine.Debug.Log("Player Registered");
                yield break;
            }
            yield break;
        }
    }
}
