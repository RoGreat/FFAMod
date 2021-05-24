using HarmonyLib;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace FFAMod
{
    [HarmonyPatch(typeof(GM_ArmsRace))]
    internal class GM_ArmsRacePatch : GM_ArmsRace
    {
        public static int p3Points;
        public static int p4Points;
        public static int p3Rounds;
        public static int p4Rounds;
        public static int winningTeamID = -1;
        private static int losingTeamID = -1;
        private static bool isReady;

        [HarmonyPatch("Start")]
        private static void Prefix()
        {
            isReady = false;
            p3Points = 0;
            p4Points = 0;
            p3Rounds = 0;
            p4Rounds = 0;
            winningTeamID = -1;
            losingTeamID = -1;
        }

        [HarmonyPatch("Start")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stfld && codes[i].operand is FieldInfo && (codes[i].operand as FieldInfo) == AccessTools.Field(typeof(GM_ArmsRace), "playersNeededToStart"))
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4_2)
                    {
                        codes[i - 1].opcode = OpCodes.Ldc_I4_4;
                        break;
                    }
                }
            }
            return codes.AsEnumerable();
        }

        [HarmonyPatch("Update")]
        private static void Postfix()
        {
            if (!PhotonNetwork.OfflineMode && !GameManager.instance.isPlaying)
            {
                int count = PlayerManager.instance.players.Count;
                if (count >= 2)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Player player = PlayerManager.instance.players[i];
                        instance.StartCoroutine(WaitToStart(player));
                    }
                }
            }
        }

        private static IEnumerator WaitToStart(Player player)
        {
            if (player.data.view.IsMine && player.data.isPlaying == false)
            {
                yield return new WaitForSeconds(0.1f);
                if (player.data.input.inputType == GeneralInput.InputType.Keyboard)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        isReady = true;
                    }
                }
                else if (player.data.playerActions.Device.CommandWasPressed)
                {
                    isReady = true;
                }
            }
            if (isReady)
            {
                var waitForSyncUp = AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp");
                yield return instance.StartCoroutine((IEnumerator)waitForSyncUp.Invoke(instance, null));
                instance.StartGame();
                isReady = false;
            }
            yield break;
        }

        [HarmonyPatch("PlayerJoined")]
        private static bool Prefix(Player player, int ___playersNeededToStart)
        {
            if (PhotonNetwork.OfflineMode)
            {
                return false;
            }
            int count = PlayerManager.instance.players.Count;
            if (!PhotonNetwork.OfflineMode)
            {
                if (player.data.view.IsMine)
                {
                    if (___playersNeededToStart - count == 2)
                    {
                        if (player.data.input.inputType == GeneralInput.InputType.Keyboard)
                            UIHandler.instance.ShowJoinGameText("TWO MORE PLAYERS CAN JOIN\n PRESS [SPACE] TO READY UP", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                        else
                            UIHandler.instance.ShowJoinGameText("TWO MORE PLAYERS CAN JOIN\n PRESS [START] TO READY UP", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                    }
                    else if (___playersNeededToStart - count == 1)
                    {
                        if (player.data.input.inputType == GeneralInput.InputType.Keyboard)
                            UIHandler.instance.ShowJoinGameText("ONE MORE PLAYER CAN JOIN\n PRESS [SPACE] TO READY UP", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                        else
                            UIHandler.instance.ShowJoinGameText("ONE MORE PLAYER CAN JOIN\n PRESS [START] TO READY UP", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                    }
                }
                else
                {
                    UIHandler.instance.ShowJoinGameText("PRESS JUMP\n TO JOIN", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                }
            }
            player.data.isPlaying = false;
            if (count >= ___playersNeededToStart)
            {
                instance.StartGame();
            }
            return false;
        }

        [HarmonyPatch("DoStartGame")]
        private static bool Prefix(ref IEnumerator __result)
        {
            __result = DoStartGamePatch();
            return false;
        }
        private static IEnumerator DoStartGamePatch()
        {
            var waitForSyncUp = AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp");
            var setPlayersVisible = AccessTools.Method(typeof(PlayerManager), "SetPlayersVisible");
            GameManager.instance.battleOngoing = false;
            UIHandler.instance.ShowJoinGameText("LETS GOO!", PlayerSkinBank.GetPlayerSkinColors(1).winText);
            yield return new WaitForSeconds(0.25f);
            UIHandler.instance.HideJoinGameText();
            PlayerManager.instance.SetPlayersSimulated(false);
            // PlayerManager.instance.SetPlayersVisible(false);
            setPlayersVisible.Invoke(PlayerManager.instance, new object[] { false });
            MapManager.instance.LoadNextLevel();
            TimeHandler.instance.DoSpeedUp();
            yield return new WaitForSecondsRealtime(1f);
            if (instance.pickPhase)
            {
                for (int i = 0; i < PlayerManager.instance.players.Count; i++)
                {
                    Player player = PlayerManager.instance.players[i];
                    yield return instance.StartCoroutine((IEnumerator)waitForSyncUp.Invoke(instance, null));
                    CardChoiceVisuals.instance.Show(i, true);
                    if (player.GetComponent<PlayerAPI>().enabled == true)
                    {
                        // AIPick(player);
                        yield return new WaitForSecondsRealtime(0.3f);
                    }
                    else
                        yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Team);
                    yield return new WaitForSecondsRealtime(0.3f);
                }
                yield return instance.StartCoroutine((IEnumerator)waitForSyncUp.Invoke(instance, null));
                CardChoiceVisuals.instance.Hide();
            }
            MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
            TimeHandler.instance.DoSpeedUp();
            TimeHandler.instance.StartGame();
            GameManager.instance.battleOngoing = true;
            UIHandler.instance.ShowRoundCounterSmall(instance.p1Rounds, instance.p2Rounds, instance.p1Points, instance.p2Points);
            // PlayerManager.instance.SetPlayersVisible(true);
            setPlayersVisible.Invoke(PlayerManager.instance, new object[] { true });
            yield break;
        }

        [HarmonyPatch("PlayerDied")]
        private static bool Prefix(Player killedPlayer, int playersAlive, ref PhotonView ___view)
        {
            if (!PhotonNetwork.OfflineMode)
                Debug.Log("PlayerDied: " + killedPlayer.data.view.Owner.NickName);
            if (PlayerManagerPatch.TeamsAlivePatch() >= 2)
                return false;
            TimeHandler.instance.DoSlowDown();
            if (!PhotonNetwork.IsMasterClient)
                return false;
            ___view.RPC("RPCA_NextRound", RpcTarget.All, PlayerManagerPatch.GetOtherTeamPatch(PlayerManager.instance.GetLastTeamAlive()), PlayerManager.instance.GetLastTeamAlive(), instance.p1Points, instance.p2Points, instance.p1Rounds, instance.p2Rounds);
            return false;
        }

        [HarmonyPatch("RPCA_NextRound")]
        private static bool Prefix(int losingTeamID, int winningTeamID, int p1PointsSet, int p2PointsSet, int p1RoundsSet, int p2RoundsSet, ref bool ___isTransitioning)
        {
            int losingTeamID2 = -1;
            int losingTeamID3 = -1;
            if (PlayerManager.instance.players.Count >= 3)
            {
                losingTeamID2 = PlayerManagerPatch.GetOtherTeamPatch(PlayerManager.instance.GetLastTeamAlive(), 2);
                Debug.Log("Losing team: " + losingTeamID2);
            }
            if (PlayerManager.instance.players.Count == 4)
            {
                losingTeamID3 = PlayerManagerPatch.GetOtherTeamPatch(PlayerManager.instance.GetLastTeamAlive(), 3);
                Debug.Log("Losing team: " + losingTeamID3);
            }
            GM_ArmsRacePatch.losingTeamID = losingTeamID;
            GM_ArmsRacePatch.winningTeamID = winningTeamID;
            Debug.Log("Losing team: " + losingTeamID);
            Debug.Log("Winning team: " + winningTeamID);
            if (___isTransitioning)
                return false;
            GameManager.instance.battleOngoing = false;
            instance.p1Points = p1PointsSet;
            instance.p2Points = p2PointsSet;
            instance.p1Rounds = p1RoundsSet;
            instance.p2Rounds = p2RoundsSet;
            ___isTransitioning = true;
            GameManager.instance.GameOver(winningTeamID, losingTeamID);
            if (losingTeamID2 >= 0)
                GameManager.instance.GameOver(winningTeamID, losingTeamID2);
            if (losingTeamID3 >= 0)
                GameManager.instance.GameOver(winningTeamID, losingTeamID3);
            PlayerManager.instance.SetPlayersSimulated(false);
            switch (winningTeamID)
            {
                case 0:
                    Points(ref instance.p1Points, ref instance.p1Rounds);
                    break;
                case 1:
                    Points(ref instance.p2Points, ref instance.p2Rounds);
                    break;
                case 2:
                    Points(ref p3Points, ref p3Rounds);
                    break;
                default:
                    Points(ref p4Points, ref p4Rounds);
                    break;
            }
            return false;
        }

        private static void Points(ref int points, ref int rounds)
        {
            int pointsToWinRound = (int)AccessTools.Field(typeof(GM_ArmsRace), "pointsToWinRound").GetValue(instance);
            ++points;
            if (points >= pointsToWinRound)
            {
                ++rounds;
                if (rounds >= instance.roundsToWinGame)
                {
                    Debug.Log("Game over, winning team: " + winningTeamID);
                    // GameOver(winningTeamID);
                    AccessTools.Method(typeof(GM_ArmsRace), "GameOver").Invoke(instance, new object[] { winningTeamID });
                    instance.pointOverAction();
                    return;
                }
                Debug.Log("Round over, winning team: " + winningTeamID);
                RoundOver(winningTeamID, losingTeamID);
                instance.pointOverAction();
                return;
            }
            Debug.Log("Point over, winning team: " + winningTeamID);
            PointOver(winningTeamID);
            instance.pointOverAction();
        }

        private static void RoundOver(int winningTeamID, int losingTeamID)
        {
            // this.currentWinningTeamID = winningTeamID;
            AccessTools.Field(typeof(GM_ArmsRace), "currentWinningTeamID").SetValue(instance, winningTeamID);
            instance.StartCoroutine(RoundTransition(winningTeamID, losingTeamID));
            instance.p1Points = 0;
            instance.p2Points = 0;
            p3Points = 0;
            p4Points = 0;
        }

        private static IEnumerator RoundTransition(int winningTeamID, int losingTeamID)
        {
            GM_ArmsRace gmArmsRace = instance;
            var SetPlayersVisible = AccessTools.Method(typeof(PlayerManager), "SetPlayersVisible");
            var WaitForSyncUp = AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp");
            gmArmsRace.StartCoroutine(PointVisualizer.instance.DoWinSequence(gmArmsRace.p1Points, gmArmsRace.p2Points, gmArmsRace.p1Rounds, gmArmsRace.p2Rounds, winningTeamID == 0));
            yield return new WaitForSecondsRealtime(1f);
            MapManager.instance.LoadNextLevel();
            yield return new WaitForSecondsRealtime(0.3f);
            yield return new WaitForSecondsRealtime(1f);
            TimeHandler.instance.DoSpeedUp();
            if (gmArmsRace.pickPhase)
            {
                Debug.Log("PICK PHASE");
                // PlayerManager.instance.SetPlayersVisible(false);
                SetPlayersVisible.Invoke(PlayerManager.instance, new object[] { false });
                for (int i = 0; i < PlayerManager.instance.players.Count; i++)
                {
                    Player player = PlayerManager.instance.players[i];
                    if (player.teamID != winningTeamID)
                    {
                        // yield return gmArmsRace.StartCoroutine(gmArmsRace.WaitForSyncUp());
                        yield return gmArmsRace.StartCoroutine((IEnumerator)WaitForSyncUp.Invoke(gmArmsRace, null));
                        CardChoiceVisuals.instance.Show(i, true);
                        if (player.GetComponent<PlayerAPI>().enabled == true)
                        {
                            // AIPick(player);
                            yield return new WaitForSecondsRealtime(0.3f);
                        }
                        else if (player.teamID != winningTeamID)
                            yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
                        yield return new WaitForSecondsRealtime(0.3f);
                    }
                }
                // PlayerManager.instance.SetPlayersVisible(true);
                SetPlayersVisible.Invoke(PlayerManager.instance, new object[] { true });
            }
            yield return gmArmsRace.StartCoroutine((IEnumerator)WaitForSyncUp.Invoke(gmArmsRace, null));
            CardChoiceVisuals.instance.Hide();
            TimeHandler.instance.DoSlowDown();
            MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
            PlayerManager.instance.RevivePlayers();
            yield return new WaitForSecondsRealtime(0.3f);
            TimeHandler.instance.DoSpeedUp();
            // gmArmsRace.isTransitioning = false;
            AccessTools.Field(typeof(GM_ArmsRace), "isTransitioning").SetValue(gmArmsRace, false);
            GameManager.instance.battleOngoing = true;
            UIHandler.instance.ShowRoundCounterSmall(gmArmsRace.p1Rounds, gmArmsRace.p2Rounds, gmArmsRace.p1Points, gmArmsRace.p2Points);
        }

        private static void PointOver(int winningTeamID)
        {
            int num = instance.p1Points;
            int num2 = instance.p2Points;
            int num3 = p3Points;
            int num4 = p4Points;
            if (winningTeamID == 0)
                num--;
            else if (winningTeamID == 1)
                num2--;
            else if (winningTeamID == 2)
                num3--;
            else
                num4--;
            string winTextBefore = num.ToString() + " - " + num2.ToString();
            string winText = instance.p1Points.ToString() + " - " + instance.p2Points.ToString();
            // instance.StartCoroutine(PointTransition(winningTeamID, winTextBefore, winText));
            instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(GM_ArmsRace), "PointTransition").Invoke(instance, new object[] { winningTeamID, winTextBefore, winText }));
            UIHandler.instance.ShowRoundCounterSmall(instance.p1Rounds, instance.p2Rounds, instance.p1Points, instance.p2Points);
        }

        /*
        private static void AIPick(Player player)
        {
            // StartPick
            // CardChoice.instance.pickerType = player.playerID;
            AccessTools.Field(typeof(CardChoice), "pickerType").SetValue(CardChoice.instance, PickerType.Team);
            CardChoice.instance.pickrID = player.playerID;
            CardChoice.instance.picks = 1;
            ArtHandler.instance.SetSpecificArt(CardChoice.instance.cardPickArt);
            // DoPlayerSelect
            SoundMusicManager.Instance.PlayIngame(true);
            // var replaceCardsMethod = AccessTools.Method(typeof(CardChoice), "ReplaceCards");
            var getRanomCardMethod = AccessTools.Method(typeof(CardChoice), "GetRanomCard");
            var getRanomCardInvoke = (GameObject)getRanomCardMethod.Invoke(CardChoice.instance, null);
            GameObject getRandomCard = getRanomCardInvoke.gameObject;
            getRandomCard.GetComponent<CardInfo>().sourceCard = getRandomCard.GetComponent<CardInfo>();
            CardChoice.instance.Pick(getRandomCard, false);
            CardChoice.instance.pickrID = -1;
            var setCurrentSelected = AccessTools.Method(typeof(CardChoiceVisuals), "SetCurrentSelected");
            // CardChoiceVisuals.instance.SetCurrentSelected(this.currentlySelectedCard);
            setCurrentSelected.Invoke(CardChoiceVisuals.instance, new object[] { 0 });
            // End of Pick(GameObject pickedCard, bool clear)
            //if (PlayerManager.instance.players[CardChoice.instance.pickrID].data.view.IsMine)
            //{
            //    var replaceCards = (IEnumerator)replaceCardsMethod.Invoke(CardChoice.instance, new object[] { getRandomCard, true });
            //    CardChoice.instance.StartCoroutine(replaceCards);
            //}
            UIHandler.instance.StopShowPicker();
            CardChoiceVisuals.instance.Hide();


            CardChoice.instance.pickerType = PickerType.Team;
            CardChoice.instance.pickrID = player.playerID;
            // ArtHandler.instance.SetSpecificArt(CardChoice.instance.cardPickArt);
            // Update => DoPlayerSelect
            SoundMusicManager.Instance.PlayIngame(true);
            // SpawnUniqueCard => GetRanomCard
            var getRanomCardMethod = AccessTools.Method(typeof(CardChoice), "GetRanomCard");
            var getRanomCardInvoke = (GameObject)getRanomCardMethod.Invoke(CardChoice.instance, null);
            GameObject getRandomCard = getRanomCardInvoke.gameObject;
            // Update => DoPlayerSelect
            CardChoice.instance.Pick(getRandomCard, false);
            CardChoice.instance.pickrID = -1;
            // CardChoiceVisuals.instance.Hide();
        }
        */
    }
}
