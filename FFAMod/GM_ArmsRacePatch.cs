using HarmonyLib;
using Photon.Pun;
using SoundImplementation;
using System;
using System.Collections;
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

        private static int winningTeamID = -1;
        private static int losingTeamID = -1;
        private static int losingTeamID2 = -1;
        private static int losingTeamID3 = -1;

        [HarmonyPatch("Start")]
        private static bool Prefix(ref int ___playersNeededToStart, ref PhotonView ___view)
        {
            var playerManager = PlayerManager.instance;
            var playerAssigner = PlayerAssigner.instance;
            var uiHandler = UIHandler.instance;
            ___view = instance.GetComponent<PhotonView>();
            playerManager.SetPlayersSimulated(false);
            playerAssigner.maxPlayers = ___playersNeededToStart;
            // PlayerAssigner.instance.SetPlayersCanJoin(true);
            AccessTools.Method(typeof(PlayerAssigner), "SetPlayersCanJoin").Invoke(playerAssigner, new object[] { true });
            // PlayerManager.instance.AddPlayerDiedAction(new Action<Player, int>(this.PlayerDied));
            AccessTools.Method(typeof(PlayerManager), "AddPlayerDiedAction").Invoke(playerManager, new object[] { new Action<Player, int>(instance.PlayerDied) });

            // playerManager.PlayerJoinedAction = (Action<Player>)Delegate.Combine(playerManager.PlayerJoinedAction, new Action<Player>(instance.PlayerJoined));
            Traverse.Create(playerManager).Property("PlayerJoinedAction").SetValue((Action<Player>)Delegate.Combine(playerManager.PlayerJoinedAction, new Action<Player>(instance.PlayerJoined)));
            ArtHandler.instance.NextArt();
            ___playersNeededToStart = 4;
            Main.mod.Logger.Log("Players needed to start: " + ___playersNeededToStart);
            // UIHandler.instance.SetNumberOfRounds(instance.roundsToWinGame);
            AccessTools.Method(typeof(UIHandler), "SetNumberOfRounds").Invoke(uiHandler, new object[] { instance.roundsToWinGame });
            playerAssigner.maxPlayers = ___playersNeededToStart;
            if (!PhotonNetwork.OfflineMode)
            {
                uiHandler.ShowJoinGameText("PRESS JUMP\n TO JOIN", PlayerSkinBank.GetPlayerSkinColors(0).winText);
            }
            return false;
        }

        [HarmonyPatch("Update")]
        private static void Postfix(ref int ___playersNeededToStart)
        {
            if (Input.GetKey(KeyCode.Alpha2))
            {
                SoundPlayerStatic.Instance.PlayButtonClick();
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                ___playersNeededToStart = 3;
                PlayerAssigner.instance.maxPlayers = ___playersNeededToStart;
                SoundPlayerStatic.Instance.PlayButtonClick();
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                SoundPlayerStatic.Instance.PlayButtonClick();
            }
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
            ___view.RPC("RPCA_NextRound", RpcTarget.All, PlayerManager.instance.GetOtherTeam(PlayerManager.instance.GetLastTeamAlive()), PlayerManager.instance.GetLastTeamAlive(), instance.p1Points, instance.p2Points, instance.p1Rounds, instance.p2Rounds);
            return false;
        }

        [HarmonyPatch("RPCA_NextRound")]
        private static bool Prefix(int losingTeamID, int winningTeamID, int p1PointsSet, int p2PointsSet, int p1RoundsSet, int p2RoundsSet)
        {
            GM_ArmsRacePatch.winningTeamID = winningTeamID;
            Debug.Log("Winning team: " + winningTeamID);
            if (PlayerManager.instance.players.Count >= 3)
            {
                losingTeamID = PlayerManagerPatch.GetOtherTeamPatch(PlayerManager.instance.GetLastTeamAlive());
                Debug.Log("Losing team: " + losingTeamID);
                losingTeamID2 = PlayerManagerPatch.GetOtherTeamPatch(PlayerManager.instance.GetLastTeamAlive(), 2);
                Debug.Log("Losing team: " + losingTeamID2);
            }
            if (PlayerManager.instance.players.Count == 4)
            {
                losingTeamID3 = PlayerManagerPatch.GetOtherTeamPatch(PlayerManager.instance.GetLastTeamAlive(), 3);
                Debug.Log("Losing team: " + losingTeamID3);
            }
            GM_ArmsRacePatch.losingTeamID = losingTeamID;
            Debug.Log("Losing team: " + losingTeamID);
            var isTransitioning = AccessTools.Field(typeof(GM_ArmsRace), "isTransitioning");
            if ((bool)isTransitioning.GetValue(instance))
                return false;
            GameManager.instance.battleOngoing = false;
            instance.p1Points = p1PointsSet;
            instance.p2Points = p2PointsSet;
            instance.p1Rounds = p1RoundsSet;
            instance.p2Rounds = p2RoundsSet;
            isTransitioning.SetValue(instance, true);
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
            if (winningTeamID == 0 || winningTeamID == 1)
            {
                gmArmsRace.StartCoroutine(PointVisualizer.instance.DoWinSequence(gmArmsRace.p1Points, gmArmsRace.p2Points, gmArmsRace.p1Rounds, gmArmsRace.p2Rounds, winningTeamID == 0));
            }
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
                yield return new WaitForSecondsRealtime(0.1f);
                for (int i = 0; i < PlayerManager.instance.players.Count; i++)
                {
                    Player player = PlayerManager.instance.players[i];
                    if (player.teamID == losingTeamID)
                    {
                        // yield return gmArmsRace.StartCoroutine(gmArmsRace.WaitForSyncUp());
                        yield return gmArmsRace.StartCoroutine((IEnumerator)WaitForSyncUp.Invoke(gmArmsRace, null));
                        yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    if (player.teamID == losingTeamID2 && losingTeamID2 >= 0)
                    {
                        yield return gmArmsRace.StartCoroutine((IEnumerator)WaitForSyncUp.Invoke(gmArmsRace, null));
                        yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    if (player.teamID == losingTeamID3 && losingTeamID3 >= 0)
                    {
                        yield return gmArmsRace.StartCoroutine((IEnumerator)WaitForSyncUp.Invoke(gmArmsRace, null));
                        yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                }
                // PlayerManager.instance.SetPlayersVisible(true);
                SetPlayersVisible.Invoke(PlayerManager.instance, new object[] { true });
            }
            yield return gmArmsRace.StartCoroutine((IEnumerator)AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp").Invoke(gmArmsRace, null));
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
            string winTextBefore = num.ToString() + " - " + num2.ToString() + ", " + num3.ToString() + ", " + num4.ToString();
            string winText = instance.p1Points.ToString() + " - " + instance.p2Points.ToString() + ", " + p3Points.ToString() + ", " + p4Points.ToString();
            instance.StartCoroutine(PointTransition(winningTeamID, winTextBefore, winText));
            UIHandler.instance.ShowRoundCounterSmall(instance.p1Rounds, instance.p2Rounds, instance.p1Points, instance.p2Points);
        }

        private static IEnumerator PointTransition(int winningTeamID, string winTextBefore, string winText)
        {
            GM_ArmsRace gmArmsRace = instance;
            if (winningTeamID == 0 || winningTeamID == 1)
            {
                gmArmsRace.StartCoroutine(PointVisualizer.instance.DoSequence(gmArmsRace.p1Points, gmArmsRace.p2Points, winningTeamID == 0));
            }
            yield return new WaitForSecondsRealtime(1f);
            MapManager.instance.LoadNextLevel();
            yield return new WaitForSecondsRealtime(0.5f);
            yield return gmArmsRace.StartCoroutine((IEnumerator)AccessTools.Method(typeof(GM_ArmsRace), "WaitForSyncUp").Invoke(gmArmsRace, null));
            
            MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
            PlayerManager.instance.RevivePlayers();
            yield return new WaitForSecondsRealtime(0.3f);
            TimeHandler.instance.DoSpeedUp();
            GameManager.instance.battleOngoing = true;
            // gmArmsRace.isTransitioning = false;
            AccessTools.Field(typeof(GM_ArmsRace), "isTransitioning").SetValue(gmArmsRace, false);
        }
    }
}
