using HarmonyLib;
using Photon.Pun;
using SoundImplementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace FFAMod
{
    [HarmonyPatch(typeof(GM_ArmsRace))]
    internal class GM_ArmsRacePatch
    {
        public static int p3Points;
        public static int p4Points;
        public static int p3Rounds;
        public static int p4Rounds;
        public static int winningTeamID;
        private static int pointsToWinRound;
        private static GameObject currentCard;
        private static int waitingForOtherPlayers;

        [HarmonyPatch("IDoRematch")]
        private static bool Prefix(ref IEnumerator __result, GM_ArmsRace __instance)
        {
            __result = IDoRematch(__instance);
            return false;
        }

        private static IEnumerator IDoRematch(GM_ArmsRace __instance)
        {
            if (!PhotonNetwork.OfflineMode)
            {
                __instance.GetComponent<PhotonView>().RPC("RPCA_PlayAgain", RpcTarget.All, new object[] { });
                UIHandler.instance.DisplayScreenTextLoop("WAITING");
                float c = 0f;
                while (waitingForOtherPlayers < PlayerAssigner.instance.maxPlayers)
                {
                    c += Time.unscaledDeltaTime;
                    if (c > 10f)
                    {
                        DoRestart();
                        yield break;
                    }
                    yield return null;
                }
            }
            yield return null;
            UIHandler.instance.StopScreenTextLoop();
            AccessTools.Method(typeof(PlayerManager), "ResetCharacters").Invoke(PlayerManager.instance, null);
            ResetMatch(__instance);
            var doStartGame = AccessTools.Method(typeof(GM_ArmsRace), "DoStartGame").Invoke(__instance, null);
            __instance.StartCoroutine((IEnumerator)doStartGame);
            waitingForOtherPlayers = 0;
            yield break;
        }

        [HarmonyPatch("RPCA_PlayAgain")]
        private static bool Prefix()
        {
            waitingForOtherPlayers += 1;
            return false;
        }

        private static void ResetMatch(GM_ArmsRace __instance)
        {
            __instance.p1Points = 0;
            __instance.p1Rounds = 0;
            __instance.p2Points = 0;
            __instance.p2Rounds = 0;
            p3Points = 0;
            p4Points = 0;
            p3Rounds = 0;
            p4Rounds = 0;
            //this.isTransitioning = false;
            AccessTools.Field(typeof(GM_ArmsRace), "isTransitioning").SetValue(__instance, false);
            waitingForOtherPlayers = 0;
            UIHandler.instance.ShowRoundCounterSmall(__instance.p1Rounds, __instance.p2Rounds, __instance.p1Points, __instance.p2Points);
            CardBarHandler.instance.ResetCardBards();
            PointVisualizer.instance.ResetPoints();
        }

        private static void DoRestart()
        {
            GameManager.instance.battleOngoing = false;
            if (PhotonNetwork.OfflineMode)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Application.LoadLevel(Application.loadedLevel);
#pragma warning restore CS0618 // Type or member is obsolete
                return;
            }
            NetworkConnectionHandler.instance.NetworkRestart();
        }

        [HarmonyPatch("Start")]
        private static void Postfix()
        {
            if (PhotonNetwork.CurrentRoom != null && !PhotonNetwork.OfflineMode)
            {
                UnityEngine.Debug.Log("ONLINE MODE");
                PlayerAssigner.instance.maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
                UnityEngine.Debug.Log("Start maxPlayers " + PlayerAssigner.instance.maxPlayers);
            }
        }

        [HarmonyPatch("PlayerJoined")]
        private static bool Prefix(Player player)
        {
            if (PhotonNetwork.OfflineMode)
            {
                UnityEngine.Debug.Log("OFFLINE MODE");
                PlayerAssigner.instance.maxPlayers = NetworkConnectionHandlerPatch.PlayersNeededToStart;
                return false;
            }
            else if (PhotonNetwork.CurrentRoom != null)
            {
                UnityEngine.Debug.Log("ONLINE MODE");
                PlayerAssigner.instance.maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
            }
            int count = PlayerManager.instance.players.Count;
            UnityEngine.Debug.Log("PlayerJoined count " + count);
            UnityEngine.Debug.Log("PlayerJoined maxPlayers " + PlayerAssigner.instance.maxPlayers);
            if (!PhotonNetwork.OfflineMode)
            {
                if (player.data.view.IsMine)
                {
                    if (PlayerAssigner.instance.maxPlayers - count == 3)
                    {
                        UIHandler.instance.ShowJoinGameText("ADD THREE MORE PLAYERS TO START", PlayerSkinBank.GetPlayerSkinColors(1).winText);
                    }
                    if (PlayerAssigner.instance.maxPlayers - count == 2)
                    {
                        UIHandler.instance.ShowJoinGameText("ADD TWO MORE PLAYERS TO START", PlayerSkinBank.GetPlayerSkinColors(1).winText);
                    }
                    if (PlayerAssigner.instance.maxPlayers - count == 1)
                    {
                        UIHandler.instance.ShowJoinGameText("ADD ONE MORE PLAYER TO START", PlayerSkinBank.GetPlayerSkinColors(1).winText);
                    }
                }
                else
                {
                    UIHandler.instance.ShowJoinGameText("PRESS JUMP\n TO JOIN", PlayerSkinBank.GetPlayerSkinColors(1).winText);
                }
            }
            player.data.isPlaying = false;
            if (count >= PlayerAssigner.instance.maxPlayers)
            {
                GM_ArmsRace.instance.StartGame();
            }
            return false;
        }

        [HarmonyPatch("DoStartGame")]
        private static bool Prefix(ref IEnumerator __result)
        {
            // ResetMatch
            p3Points = 0;
            p4Points = 0;
            p3Rounds = 0;
            p4Rounds = 0;
            __result = DoStartGame();
            return false;
        }

        private static IEnumerator DoStartGame()
        {
            var instance = GM_ArmsRace.instance;
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
                    if (player.GetComponent<PlayerAPI>().enabled)
                        yield return AIPick(player);
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
            var instance = GM_ArmsRace.instance;
            if (!PhotonNetwork.OfflineMode)
                UnityEngine.Debug.Log("PlayerDied: " + killedPlayer.data.view.Owner.NickName);
            if (PlayerManagerPatch.TeamsAlive() >= 2)
                return false;
            TimeHandler.instance.DoSlowDown();
            instance.StartCoroutine(WaitForSyncUp());
            if (!PhotonNetwork.IsMasterClient)
                return false;
            ___view.RPC("RPCA_NextRound", RpcTarget.All, PlayerManagerPatch.GetOtherTeam(PlayerManager.instance.GetLastTeamAlive()), PlayerManager.instance.GetLastTeamAlive(), instance.p1Points, instance.p2Points, instance.p1Rounds, instance.p2Rounds);
            return false;
        }

        private static IEnumerator WaitForSyncUp()
        {
            var instance = GM_ArmsRace.instance;
            var waitForSyncUp = AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp").Invoke(instance, null);
            yield return instance.StartCoroutine((IEnumerator)waitForSyncUp);
        }


        [HarmonyPatch("RPCA_NextRound")]
        private static bool Prefix(int losingTeamID, int winningTeamID, int p1PointsSet, int p2PointsSet, int p1RoundsSet, int p2RoundsSet, ref bool ___isTransitioning, int ___pointsToWinRound)
        {
            var instance = GM_ArmsRace.instance;
            int losingTeamID2 = -1;
            int losingTeamID3 = -1;
            UnityEngine.Debug.Log("Losing team: " + losingTeamID);
            if (PlayerManager.instance.players.Count >= 3)
            {
                losingTeamID2 = PlayerManagerPatch.GetOtherTeam(PlayerManager.instance.GetLastTeamAlive(), 2);
                UnityEngine.Debug.Log("Losing team: " + losingTeamID2);
            }
            if (PlayerManager.instance.players.Count == 4)
            {
                losingTeamID3 = PlayerManagerPatch.GetOtherTeam(PlayerManager.instance.GetLastTeamAlive(), 3);
                UnityEngine.Debug.Log("Losing team: " + losingTeamID3);
            }
            GM_ArmsRacePatch.winningTeamID = winningTeamID;
            pointsToWinRound = ___pointsToWinRound;
            UnityEngine.Debug.Log("Winning team: " + winningTeamID);
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
            var instance = GM_ArmsRace.instance;
            points++;
            if (points >= pointsToWinRound)
            {
                rounds++;
                if (rounds >= instance.roundsToWinGame)
                {
                    UnityEngine.Debug.Log("Game over, winning team: " + winningTeamID);
                    // GameOver(winningTeamID);
                    AccessTools.Method(typeof(GM_ArmsRace), "GameOver").Invoke(instance, new object[] { winningTeamID });
                    instance.pointOverAction();
                    return;
                }
                UnityEngine.Debug.Log("Round over, winning team: " + winningTeamID);
                RoundOver(winningTeamID);
                instance.pointOverAction();
                return;
            }
            UnityEngine.Debug.Log("Point over, winning team: " + winningTeamID);
            PointOver(winningTeamID);
            instance.pointOverAction();
        }

        private static void RoundOver(int winningTeamID)
        {
            var instance = GM_ArmsRace.instance;
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
            var instance = GM_ArmsRace.instance;
            var setPlayersVisible = AccessTools.Method(typeof(PlayerManager), "SetPlayersVisible");
            var waitForSyncUp = AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp");
            GM_ArmsRacePatch.winningTeamID = winningTeamID;
            instance.StartCoroutine(PointVisualizer.instance.DoWinSequence(instance.p1Points, instance.p2Points, instance.p1Rounds, instance.p2Rounds, winningTeamID == 0));
            
            yield return new WaitForSecondsRealtime(1f);
            MapManager.instance.LoadNextLevel();
            yield return new WaitForSecondsRealtime(0.3f);
            yield return new WaitForSecondsRealtime(1f);
            TimeHandler.instance.DoSpeedUp();
            if (instance.pickPhase)
            {
                UnityEngine.Debug.Log("PICK PHASE");
                // PlayerManager.instance.SetPlayersVisible(false);
                setPlayersVisible.Invoke(PlayerManager.instance, new object[] { false });
                var players = PlayerManager.instance.players;
                for (int i = 0; i < players.Count; i++)
                {
                    Player player = players[i];
                    if (player.teamID != winningTeamID)
                    {
                        yield return instance.StartCoroutine((IEnumerator)waitForSyncUp.Invoke(instance, null));
                        CardChoiceVisuals.instance.Show(i, true);
                        if (player.data.currentCards.Count < GM_ArmsRace.instance.roundsToWinGame)
                        {
                            // yield return this.StartCoroutine(gmArmsRace.WaitForSyncUp());
                            if (player.GetComponent<PlayerAPI>().enabled)
                                yield return AIPick(player);
                            else
                                yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
                        }
                        yield return new WaitForSecondsRealtime(0.3f);
                    }
                }
                // PlayerManager.instance.SetPlayersVisible(true);
                setPlayersVisible.Invoke(PlayerManager.instance, new object[] { true });
            }
            yield return instance.StartCoroutine((IEnumerator)waitForSyncUp.Invoke(instance, null));
            TimeHandler.instance.DoSlowDown();
            MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
            PlayerManager.instance.RevivePlayers();
            yield return new WaitForSecondsRealtime(0.3f);
            TimeHandler.instance.DoSpeedUp();
            // this.isTransitioning = false;
            AccessTools.Field(typeof(GM_ArmsRace), "isTransitioning").SetValue(instance, false);
            GameManager.instance.battleOngoing = true;
            UIHandler.instance.ShowRoundCounterSmall(instance.p1Rounds, instance.p2Rounds, instance.p1Points, instance.p2Points);
            foreach (var child in GameObject.Find("P3").GetComponentsInChildren<ProceduralImage>())
                {
                    if (child.transform.localScale == new Vector3(0.3f,0.3f,0.3f))
                    {
                        child.GetComponent<ProceduralImage>().color = new Color(0.3387f, 0.3696f, 0.4057f);
                        break;
                    }
                }
            foreach (var child in GameObject.Find("P4").GetComponentsInChildren<ProceduralImage>())
                {
                    if (child.transform.localScale == new Vector3(0.3f,0.3f,0.3f))
                    {
                        child.GetComponent<ProceduralImage>().color = new Color(0.3387f, 0.3696f, 0.4057f);
                        break;
                    }
                }

        }

        private static void PointOver(int winningTeamID)
        {
            var instance = GM_ArmsRace.instance;
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

        private static IEnumerator AIPick(Player player)
        {
            // - CardChoice
            // -- DoPick
            // CardChoice.instance.pickerType = player.playerID;
            UnityEngine.Debug.Log("AI Picks Card");
            AccessTools.Field(typeof(CardChoice), "pickerType").SetValue(CardChoice.instance, PickerType.Team);
            // -- StartPick
            CardChoice.instance.pickrID = player.playerID;
            CardChoice.instance.picks = 1;
            ArtHandler.instance.SetSpecificArt(CardChoice.instance.cardPickArt);
            // -- Pick
            CardChoice.instance.Pick(null);
            yield return new WaitForSecondsRealtime(1f);

            // -- Update
            // -- DoPlayerSelect
            SoundMusicManager.Instance.PlayIngame(true);
            // - CardBar
            // -- OnHover
            var spawnedCards = (List<GameObject>)AccessTools.Field(typeof(CardChoice), "spawnedCards").GetValue(CardChoice.instance);
            currentCard = CardChoice.instance.AddCardVisual(spawnedCards[0].GetComponent<CardInfo>(), spawnedCards[0].transform.position);
            currentCard.transform.rotation = spawnedCards[0].transform.rotation;
            currentCard.GetComponentInChildren<Canvas>().sortingLayerName = "MostFront";
            currentCard.GetComponentInChildren<GraphicRaycaster>().enabled = false;
            currentCard.GetComponentInChildren<SetScaleToZero>().enabled = false;
            currentCard.GetComponentInChildren<SetScaleToZero>().transform.localScale = Vector3.one * 1.15f;
            // - ApplyCardStats
            currentCard.transform.root.GetComponentInChildren<ApplyCardStats>().Pick(CardChoice.instance.pickrID, true);
            yield return new WaitForSecondsRealtime(1f);
            // - CardChoice
            // -- IDoEndPick
            CardChoice.instance.StartCoroutine(CardChoice.instance.IDoEndPick(currentCard, 0, CardChoice.instance.pickrID));
            CardChoice.instance.pickrID = -1;
            UIHandler.instance.StopShowPicker();
            CardChoiceVisuals.instance.Hide();
            yield return new WaitForSecondsRealtime(0.3f);
            Object.Destroy(currentCard);
            yield return new WaitForSecondsRealtime(0.3f);
            for (int i = 0; i < spawnedCards.Count; i++)
                Object.Destroy(spawnedCards[i]);
            yield break;

            // ReplaceCards
            // IEnumerator replaceCards = (IEnumerator)AccessTools.Method(typeof(CardChoice), "ReplaceCards").Invoke(CardChoice.instance, new object[] { currentCard, false });
            // CardChoice.instance.StartCoroutine(replaceCards);
        }
    }
}
