using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FFAMod
{
    [HarmonyPatch(typeof(PlayerAssigner))]
    internal class PlayerAssignerPatch
    {
        [HarmonyPatch("CreatePlayer")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            int startIndex = -1;
            int endIndex = -1;
            int startIndex2 = -1;
            int endIndex2 = -1;
            int stage = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                if (stage == 0 && codes[i].opcode == OpCodes.Ldfld && codes[i].operand is FieldInfo && (codes[i].operand as FieldInfo) == AccessTools.Field(typeof(PlayerAssigner), "playerIDToSet"))
                {
                    startIndex = i + 1;
                    stage++;
                }
                else if (stage == 1 && codes[i].opcode == OpCodes.Stfld && codes[i].operand is FieldInfo && (codes[i].operand as FieldInfo) == AccessTools.Field(typeof(PlayerAssigner), "teamIDToSet"))
                {
                    endIndex = i;
                    stage++;
                }
                else if (stage == 2 && codes[i].opcode == OpCodes.Ldfld && codes[i].operand is FieldInfo && (codes[i].operand as FieldInfo) == AccessTools.Field(typeof(PlayerAssigner), "playerIDToSet"))
                {
                    startIndex2 = i + 1;
                    stage++;
                }
                else if (stage == 3 && codes[i].opcode == OpCodes.Stfld && codes[i].operand is FieldInfo && (codes[i].operand as FieldInfo) == AccessTools.Field(typeof(PlayerAssigner), "teamIDToSet"))
                {
                    endIndex2 = i;
                    break;
                }
            }
            if (startIndex > -1 && endIndex > -1)
            {
                codes[startIndex].opcode = OpCodes.Nop;
                codes.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
            }
            if (startIndex2 > -1 && endIndex2 > -1)
            {
                codes[startIndex2].opcode = OpCodes.Nop;
                codes.RemoveRange(startIndex2 + 1, endIndex2 - startIndex2 - 1);
            }
            return codes.AsEnumerable();
        }

        [HarmonyPatch("RegisterPlayer")]
        private static void Postfix(int teamID)
        {
            Main.mod.Logger.Log("Teams: " + teamID);
        }

        /*
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
            int playerIDToSet = (int)AccessTools.Field(typeof(PlayerAssigner), "playerIDToSet").GetValue(instance);
            int teamIDToSet = (int)AccessTools.Field(typeof(PlayerAssigner), "teamIDToSet").GetValue(instance);
            if (!(bool)waitingForRegisterResponse.GetValue(instance) && (PhotonNetwork.OfflineMode || !(bool)hasCreatedLocalPlayer.GetValue(instance)) && playerAssigner.players.Count < playerAssigner.maxPlayers)
            {
                if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
                {
                    playerAssigner.GetComponent<PhotonView>().RPC("RPCM_RequestTeamAndPlayerID", RpcTarget.MasterClient, (object)PhotonNetwork.LocalPlayer.ActorNumber);
                    waitingForRegisterResponse.SetValue(instance, true);
                }
                while ((bool)waitingForRegisterResponse.GetValue(instance))
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
                hasCreatedLocalPlayer.SetValue(instance, true);
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
        */
    }
}
