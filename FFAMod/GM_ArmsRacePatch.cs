using HarmonyLib;
using Photon.Pun;
using SoundImplementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FFAMod
{
    [HarmonyPatch(typeof(GM_ArmsRace))]
    internal class GM_ArmsRacePatch : GM_ArmsRace
    {
        public static int p3Points;
        public static int p4Points;
        public static int p3Rounds;
        public static int p4Rounds;
        public static int winningTeamID;
        private static int pointsToWinRound;

        [HarmonyPatch("Start")]
        private static void Postfix()
        {
            p3Points = 0;
            p4Points = 0;
            p3Rounds = 0;
            p4Rounds = 0;
            PlayerAssigner.instance.maxPlayers = NetworkConnectionHandlerPatch.PlayersNeededToStart;
        }

        [HarmonyPatch("PlayerJoined")]
        private static bool Prefix(Player player)
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
                    if (NetworkConnectionHandlerPatch.PlayersNeededToStart - count == 3)
                    {
                        UIHandler.instance.ShowJoinGameText("ADD THREE MORE PLAYERS TO START", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                    }
                    if (NetworkConnectionHandlerPatch.PlayersNeededToStart - count == 2)
                    {
                        UIHandler.instance.ShowJoinGameText("ADD TWO MORE PLAYERS TO START", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                    }
                    if (NetworkConnectionHandlerPatch.PlayersNeededToStart - count == 1)
                    {
                        UIHandler.instance.ShowJoinGameText("ADD ONE MORE PLAYER TO START", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                    }
                }
                else
                {
                    UIHandler.instance.ShowJoinGameText("PRESS JUMP\n TO JOIN", PlayerSkinBank.GetPlayerSkinColors(count).winText);
                }
            }
            player.data.isPlaying = false;
            if (count >= NetworkConnectionHandlerPatch.PlayersNeededToStart)
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
                        AIPick(player);
                        yield return new WaitForSecondsRealtime(0.3f);
                        AISelect();
                        yield return new WaitForSecondsRealtime(1.5f);
                        AIDonePicking();
                        yield return new WaitForSecondsRealtime(0.3f);
                        Destroy(currentCard);
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
        private static bool Prefix(Player killedPlayer, PhotonView ___view)
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
        private static bool Prefix(int losingTeamID, int winningTeamID, int p1PointsSet, int p2PointsSet, int p1RoundsSet, int p2RoundsSet, ref bool ___isTransitioning, int ___pointsToWinRound)
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
            GM_ArmsRacePatch.winningTeamID = winningTeamID;
            pointsToWinRound = ___pointsToWinRound;
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
            points++;
            if (points >= pointsToWinRound)
            {
                rounds++;
                if (rounds >= instance.roundsToWinGame)
                {
                    Debug.Log("Game over, winning team: " + winningTeamID);
                    // GameOver(winningTeamID);
                    AccessTools.Method(typeof(GM_ArmsRace), "GameOver").Invoke(instance, new object[] { winningTeamID });
                    instance.pointOverAction();
                    return;
                }
                Debug.Log("Round over, winning team: " + winningTeamID);
                RoundOver(winningTeamID);
                instance.pointOverAction();
                return;
            }
            Debug.Log("Point over, winning team: " + winningTeamID);
            PointOver(winningTeamID);
            instance.pointOverAction();
        }

        private static void RoundOver(int winningTeamID)
        {
            // this.currentWinningTeamID = winningTeamID;
            AccessTools.Field(typeof(GM_ArmsRace), "currentWinningTeamID").SetValue(instance, winningTeamID);
            instance.StartCoroutine(RoundTransition(winningTeamID));
            instance.p1Points = 0;
            instance.p2Points = 0;
            p3Points = 0;
            p4Points = 0;
        }

        private static IEnumerator RoundTransition(int winningTeamID)
        {
            GM_ArmsRace gmArmsRace = instance;
            var setPlayersVisible = AccessTools.Method(typeof(PlayerManager), "SetPlayersVisible");
            var waitForSyncUp = AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp");
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
                setPlayersVisible.Invoke(PlayerManager.instance, new object[] { false });
                for (int i = 0; i < PlayerManager.instance.players.Count; i++)
                {
                    Player player = PlayerManager.instance.players[i];
                    if (player.teamID != winningTeamID)
                    {
                        // yield return gmArmsRace.StartCoroutine(gmArmsRace.WaitForSyncUp());
                        yield return gmArmsRace.StartCoroutine((IEnumerator)waitForSyncUp.Invoke(gmArmsRace, null));
                        CardChoiceVisuals.instance.Show(i, true);
                        if (player.GetComponent<PlayerAPI>().enabled == true)
                        {
                            AIPick(player);
                            yield return new WaitForSecondsRealtime(0.3f);
                            AISelect();
                            yield return new WaitForSecondsRealtime(1.5f);
                            AIDonePicking();
                            yield return new WaitForSecondsRealtime(0.3f);
                            Destroy(currentCard);
                        }
                        else if (player.teamID != winningTeamID)
                            yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
                        yield return new WaitForSecondsRealtime(0.3f);
                    }
                }
                // PlayerManager.instance.SetPlayersVisible(true);
                setPlayersVisible.Invoke(PlayerManager.instance, new object[] { true });
            }
            yield return gmArmsRace.StartCoroutine((IEnumerator)waitForSyncUp.Invoke(gmArmsRace, null));
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
            if (winningTeamID == 0)
                num--;
            else
                num2--;
            string winTextBefore = num.ToString() + " - " + num2.ToString();
            string winText = instance.p1Points.ToString() + " - " + instance.p2Points.ToString();
            // instance.StartCoroutine(PointTransition(winningTeamID, winTextBefore, winText));
            instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(GM_ArmsRace), "PointTransition").Invoke(instance, new object[] { winningTeamID, winTextBefore, winText }));
            UIHandler.instance.ShowRoundCounterSmall(instance.p1Rounds, instance.p2Rounds, instance.p1Points, instance.p2Points);
        }

        private static void AIPick(Player player)
        {
            // DoPick
            // CardChoice.instance.pickerType = player.playerID;
            UnityEngine.Debug.Log("AI Picks Card");
            AccessTools.Field(typeof(CardChoice), "pickerType").SetValue(CardChoice.instance, PickerType.Team);
            // StartPick
            CardChoice.instance.pickrID = player.playerID;
            CardChoice.instance.picks = 1;
            ArtHandler.instance.SetSpecificArt(CardChoice.instance.cardPickArt);
        }

        private static void AISelect()
        {
            // Pick
            // DoPlayerSelect
            SoundMusicManager.Instance.PlayIngame(true);
            var setCurrentSelected = AccessTools.Method(typeof(CardChoiceVisuals), "SetCurrentSelected");
            // CardChoiceVisuals.instance.SetCurrentSelected(this.currentlySelectedCard);
            setCurrentSelected.Invoke(CardChoiceVisuals.instance, new object[] { 0 });
            var getRanomCardMethod = AccessTools.Method(typeof(CardChoice), "GetRanomCard");
            GameObject getRandomCard = (GameObject)getRanomCardMethod.Invoke(CardChoice.instance, null);
            getRandomCard.GetComponent<CardInfo>().sourceCard = getRandomCard.GetComponent<CardInfo>();
            // CardBar
            // OnHover
            var children = (Transform[])AccessTools.Field(typeof(CardChoice), "children").GetValue(CardChoice.instance);
            currentCard = CardChoice.instance.AddCardVisual(getRandomCard.GetComponent<CardInfo>().sourceCard, children[0].transform.position);
            currentCard.transform.rotation = children[0].transform.rotation;
            currentCard.GetComponentInChildren<Canvas>().sortingLayerName = "MostFront";
            currentCard.GetComponentInChildren<GraphicRaycaster>().enabled = false;
            currentCard.GetComponentInChildren<SetScaleToZero>().enabled = false;
            currentCard.GetComponentInChildren<SetScaleToZero>().transform.localScale = Vector3.one * 1.15f;
            // ApplyCardStats
            getRandomCard.transform.root.GetComponentInChildren<ApplyCardStats>().Pick(CardChoice.instance.pickrID, true);
        }

        private static void AIDonePicking()
        {
            // CardChoice.instance.StartCoroutine(CardChoice.instance.IDoEndPick(currentCard, 0, CardChoice.instance.pickrID));
            CardChoice.instance.pickrID = -1;
            UIHandler.instance.StopShowPicker();
            CardChoiceVisuals.instance.Hide();

            // ReplaceCards
            // IEnumerator replaceCards = (IEnumerator)AccessTools.Method(typeof(CardChoice), "ReplaceCards").Invoke(CardChoice.instance, new object[] { currentCard, false });
            // CardChoice.instance.StartCoroutine(replaceCards);
        }

        private static GameObject currentCard;
    }
}
