using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FFAMod
{
    [HarmonyPatch(typeof(GameCrownHandler))]
    internal class GameCrownHandlerPatch
    {
        private static int previousCrownHolder = 0;

        [HarmonyPatch("LateUpdate")]
        private static void Postfix(GameCrownHandler __instance, int ___currentCrownHolder)
        {
            if (___currentCrownHolder == -1)
                return;
            var getCrownPos = AccessTools.Method(typeof(CharacterData), "GetCrownPos");
            // Vector3 position = Vector3.LerpUnclamped(PlayerManager.instance.players[0].data.GetCrownPos(), PlayerManager.instance.players[1].data.GetCrownPos(), this.crownPos);
            Vector3 position = Vector3.LerpUnclamped((Vector3)getCrownPos.Invoke(PlayerManager.instance.players[previousCrownHolder].data, null), (Vector3)getCrownPos.Invoke(PlayerManager.instance.players[___currentCrownHolder].data, null), 1f);
            __instance.transform.position = position;
        }

        [HarmonyPatch("PointOver")]
        private static bool Prefix(GM_ArmsRace ___gm, ref int ___currentCrownHolder, GameCrownHandler __instance)
        {
            previousCrownHolder = 0;
            int[] rounds = new int[4]
            {
                ___gm.p1Rounds,
                ___gm.p2Rounds,
                GM_ArmsRacePatch.p3Rounds,
                GM_ArmsRacePatch.p4Rounds
            };
            int[] points = new int[4]
            {
                ___gm.p1Points,
                ___gm.p2Points,
                GM_ArmsRacePatch.p3Points,
                GM_ArmsRacePatch.p4Points
            };
            int maxRounds = rounds.Max();
            int atMaxRounds = rounds.Count(x => x == maxRounds);
            int num = -1;
            int num2 = -1;
            if (atMaxRounds == 1)
            {
                if (rounds[0] == maxRounds)
                {
                    num2 = 0;
                }
                if (rounds[1] == maxRounds)
                {
                    num2 = 1;
                }
                if (rounds[2] == maxRounds)
                {
                    num2 = 2;
                }
                if (rounds[3] == maxRounds)
                {
                    num2 = 3;
                }
            }
            if (num2 == -1)
            {
                int num3 = -1;
                if (atMaxRounds > 1)
                {
                    int winner = -1;
                    int withMostPoints = 0;
                    for (int i = 0; i < PlayerManager.instance.players.Count; i++)
                    {
                        if (rounds[i] == maxRounds)
                        {
                            if (points[i] == 1)
                            {
                                winner = i;
                                withMostPoints += 1;
                            }
                            if (withMostPoints == 2)
                            {
                                winner = -1;
                                break;
                            }
                        }
                    }
                    if (winner == 0)
                    {
                        num3 = 0;
                    }
                    if (winner == 1)
                    {
                        num3 = 1;
                    }
                    if (winner == 2)
                    {
                        num3 = 2;
                    }
                    if (winner == 3)
                    {
                        num3 = 3;
                    }
                    if (num3 != -1)
                    {
                        num = num3;
                    }
                }
            }
            else
            {
                num = num2;
            }
            if (num != -1 && num != ___currentCrownHolder)
            {
                if (___currentCrownHolder == -1)
                {
                    ___currentCrownHolder = num;
                    __instance.GetComponent<CurveAnimation>().PlayIn();
                    return false;
                }
                previousCrownHolder = ___currentCrownHolder;
                // this.GiveCrownToPlayer(num);
                AccessTools.Method(typeof(GameCrownHandler), "GiveCrownToPlayer").Invoke(__instance, new object[] { num });
            }
            return false;
        }
    }
}
