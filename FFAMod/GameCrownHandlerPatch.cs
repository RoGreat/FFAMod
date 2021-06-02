using HarmonyLib;
using System.Collections;
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
			{
				return;
			}
			var getCrownPos = AccessTools.Method(typeof(CharacterData), "GetCrownPos");
			// Vector3 position = Vector3.LerpUnclamped(PlayerManager.instance.players[0].data.GetCrownPos(), PlayerManager.instance.players[1].data.GetCrownPos(), this.crownPos);
			Vector3 position = Vector3.LerpUnclamped((Vector3)getCrownPos.Invoke(PlayerManager.instance.players[previousCrownHolder].data, null), (Vector3)getCrownPos.Invoke(PlayerManager.instance.players[___currentCrownHolder].data, null), 1f);
			__instance.transform.position = position;
		}

		[HarmonyPatch("PointOver")]
        private static bool Prefix(GM_ArmsRace ___gm, ref int ___currentCrownHolder, GameCrownHandler __instance)
        {
			previousCrownHolder = 0;
			int p1Rounds = ___gm.p1Rounds;
			int p2Rounds = ___gm.p2Rounds;
			int p3Rounds = GM_ArmsRacePatch.p3Rounds;
			int p4Rounds = GM_ArmsRacePatch.p4Rounds;
			int p1Points = ___gm.p1Points;
			int p2Points = ___gm.p2Points;
			int p3Points = GM_ArmsRacePatch.p3Points;
			int p4Points = GM_ArmsRacePatch.p4Points;
			int num = -1;
			int num2 = -1;
			if (p1Rounds > p2Rounds && p1Rounds > p3Rounds && p1Rounds > p4Rounds)
			{
				num2 = 0;
			}
			if (p2Rounds > p1Rounds && p2Rounds > p3Rounds && p2Rounds > p4Rounds)
			{
				num2 = 1;
			}
			if (p3Rounds > p1Rounds && p3Rounds > p2Rounds && p3Rounds > p4Rounds)
			{
				num2 = 2;
			}
			if (p4Rounds > p1Rounds && p4Rounds > p2Rounds && p4Rounds > p3Rounds)
			{
				num2 = 3;
			}
			if (num2 == -1)
			{
				int num3 = -1;
				if (p1Points > p2Points && p1Points > p3Points && p1Points > p4Points)
				{
					num3 = 0;
				}
				if (p2Points > p1Points && p2Points > p3Points && p2Points > p4Points)
				{
					num3 = 1;
				}
				if (p3Points > p1Points && p3Points > p2Points && p3Points > p4Points)
				{
					num3 = 2;
				}
				if (p4Points > p1Points && p4Points > p2Points && p4Points > p3Points)
				{
					num3 = 3;
				}
				if (num3 != -1)
				{
					num = num3;
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
