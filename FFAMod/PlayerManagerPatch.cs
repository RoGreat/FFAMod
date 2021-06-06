using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FFAMod
{
    [HarmonyPatch(typeof(PlayerManager))]
    internal class PlayerManagerPatch
    {
        // Original GetOtherTeam method returns own team instead of other team
        // Utilized in the GetClosestPlayerInTeam patched method only
        [HarmonyPatch("GetOtherTeam")]
        private static bool Prefix(ref int __result, int team)
        {
            __result = team;
            return false;
        }

        public static int GetOtherTeam(int team, int offset = 1)
        {
            var instance = PlayerManager.instance;
            int[] array;
            if (instance.players.Count == 4)
                array = new int[4] { 0, 1, 2, 3 };
            else if (instance.players.Count == 3)
                array = new int[3] { 0, 1, 2 };
            else
                array = new int[2] { 0, 1 };
            return array[(team + offset) % array.Length];
        }

        [HarmonyPatch("GetClosestPlayerInTeam")]
        private static bool Prefix(ref Player __result, Vector3 position, int team, bool needVision = false)
        {
            __result = GetClosestPlayerInTeam(position, team, needVision);
            return false;
        }

        private static Player GetClosestPlayerInTeam(Vector3 position, int team, bool needVision)
        {
            var instance = PlayerManager.instance;
            Player result = null;
            Player[] players = instance.players.ToArray();
            float num = float.MaxValue;
            float num2 = 0;
            var dictionary = new Dictionary<int, float>();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].teamID == team)
                    continue;
                num2 = Vector2.Distance(position, players[i].transform.position);
                if (!players[i].data.dead)
                    dictionary.Add(i, num2);
            }
            if (dictionary.Count == 0)
                return result;
            int j = dictionary.OrderBy(x => x.Value).First().Key;
            num2 = dictionary.OrderBy(x => x.Value).First().Value;
            if ((!needVision || instance.CanSeePlayer(position, players[j]).canSee) && num2 < num)
                result = players[j];
            return result;
        }

        public static int TeamsAlive()
        {
            var instance = PlayerManager.instance;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            for (int i = 0; i < instance.players.Count; i++)
            {
                if (instance.players[i].teamID == 0 && !instance.players[i].data.dead)
                    flag = true;
                if (instance.players[i].teamID == 1 && !instance.players[i].data.dead)
                    flag2 = true;
                if (instance.players[i].teamID == 2 && !instance.players[i].data.dead)
                    flag3 = true;
                if (instance.players[i].teamID == 3 && !instance.players[i].data.dead)
                    flag4 = true;
            }
            int num = 0;
            if (flag)
                num++;
            if (flag2)
                num++;
            if (flag3)
                num++;
            if (flag4)
                num++;
            return num;
        }

        [HarmonyPatch("MovePlayers")]
        private static void Prefix(SpawnPoint[] spawnPoints)
        {
            if (PlayerManager.instance.players.Count < 3)
                return;
            if (MapManager.instance.currentLevelID == 0)
            {
                spawnPoints[0].localStartPos = new Vector3(-30f, -5f);
                spawnPoints[1].localStartPos = new Vector3(-9f, -13f);
                spawnPoints[2].localStartPos = new Vector3(9f, -13f);
                spawnPoints[3].localStartPos = new Vector3(30f, -5f);
            }
            else if (MapManager.instance.currentLevelID == 1)
            {
                spawnPoints[0].localStartPos = new Vector3(-20f, 12f);
                spawnPoints[1].localStartPos = new Vector3(-30f, -5f);
                spawnPoints[2].localStartPos = new Vector3(20f, 12f);
                spawnPoints[3].localStartPos = new Vector3(30f, -5f);
            }
            else if (MapManager.instance.currentLevelID == 2)
            {
                spawnPoints[0].localStartPos = new Vector3(-26f, 12f);
                spawnPoints[1].localStartPos = new Vector3(-24f, -11f);
                spawnPoints[2].localStartPos = new Vector3(26f, 12f);
                spawnPoints[3].localStartPos = new Vector3(24f, -11f);
            }
            else if (MapManager.instance.currentLevelID == 3)
            {
                spawnPoints[0].localStartPos = new Vector3(-27f, -14f);
                spawnPoints[1].localStartPos = new Vector3(-11f, 4f);
                spawnPoints[2].localStartPos = new Vector3(11f, 4f);
                spawnPoints[3].localStartPos = new Vector3(27f, -14f);
            }
            else if (MapManager.instance.currentLevelID == 4)
            {
                spawnPoints[0].localStartPos = new Vector3(-26f, -13f);
                spawnPoints[1].localStartPos = new Vector3(-13f, 3f);
                spawnPoints[2].localStartPos = new Vector3(13f, 3f);
                spawnPoints[3].localStartPos = new Vector3(26f, -13f);
            }
            else if (MapManager.instance.currentLevelID == 5)
            {
                spawnPoints[0].localStartPos = new Vector3(-22f, 6f);
                spawnPoints[1].localStartPos = new Vector3(-22f, -13f);
                spawnPoints[2].localStartPos = new Vector3(22f, 6f);
                spawnPoints[3].localStartPos = new Vector3(22f, -13f);
            }
            else if (MapManager.instance.currentLevelID == 6)
            {
                spawnPoints[0].localStartPos = new Vector3(7f, 5f);
                spawnPoints[1].localStartPos = new Vector3(25f, -11f);
                spawnPoints[2].localStartPos = new Vector3(-7f, 5f);
                spawnPoints[3].localStartPos = new Vector3(-25f, -11f);
            }
            else if (MapManager.instance.currentLevelID == 7)
            {
                spawnPoints[0].localStartPos = new Vector3(-29f, -5f);
                spawnPoints[1].localStartPos = new Vector3(29f, -5f);
                spawnPoints[2].localStartPos = new Vector3(-9f, 0f);
                spawnPoints[3].localStartPos = new Vector3(9f, 0f);
            }
            else if (MapManager.instance.currentLevelID == 8)
            {
                spawnPoints[0].localStartPos = new Vector3(23f, 9.6f);
                spawnPoints[1].localStartPos = new Vector3(-23f, 9.6f);
                spawnPoints[2].localStartPos = new Vector3(18f, 7.4f);
                spawnPoints[3].localStartPos = new Vector3(-18f, 7.4f);
            }
            else if (MapManager.instance.currentLevelID == 9)
            {
                spawnPoints[0].localStartPos = new Vector3(25f, 0f);
                spawnPoints[1].localStartPos = new Vector3(10.5f, 3f);
                spawnPoints[2].localStartPos = new Vector3(-25f, -0f);
                spawnPoints[3].localStartPos = new Vector3(-10.5f, 3f);
            }
            else if (MapManager.instance.currentLevelID == 10)
            {
                spawnPoints[0].localStartPos = new Vector3(22.8f, -11f);
                spawnPoints[1].localStartPos = new Vector3(17f, 1.3f);
                spawnPoints[2].localStartPos = new Vector3(-22.8f, -11f);
                spawnPoints[3].localStartPos = new Vector3(-17f, 1.3f);
            }
            else if (MapManager.instance.currentLevelID == 11)
            {
                spawnPoints[0].localStartPos = new Vector3(8.7f, 4f);
                spawnPoints[1].localStartPos = new Vector3(14f, -10f);
                spawnPoints[2].localStartPos = new Vector3(-8.7f, 4f);
                spawnPoints[3].localStartPos = new Vector3(-14f, -10);
            }
            else if (MapManager.instance.currentLevelID == 12)
            {
                spawnPoints[0].localStartPos = new Vector3(26f, -7.5f);
                spawnPoints[1].localStartPos = new Vector3(-26f, -7.5f);
                spawnPoints[2].localStartPos = new Vector3(16.8f, 6.6f);
                spawnPoints[3].localStartPos = new Vector3(-16.8f, 6.6f);
            }
            else if (MapManager.instance.currentLevelID == 13)
            {
                spawnPoints[0].localStartPos = new Vector3(18.4f, -4.5f);
                spawnPoints[1].localStartPos = new Vector3(-18.4f, -4.5f);
                spawnPoints[2].localStartPos = new Vector3(25.7f, 8.5f);
                spawnPoints[3].localStartPos = new Vector3(-25.7f, 8.5f);
            }
            else if (MapManager.instance.currentLevelID == 14)
            {
                spawnPoints[0].localStartPos = new Vector3(27.3f, 3.3f);
                spawnPoints[1].localStartPos = new Vector3(-27.3f, 3.3f);
                spawnPoints[2].localStartPos = new Vector3(16.5f, 4.5f);
                spawnPoints[3].localStartPos = new Vector3(-16.5f, 4.5f);
            }
            else if (MapManager.instance.currentLevelID == 15)
            {
                spawnPoints[0].localStartPos = new Vector3(13.2f, 0.46f);
                spawnPoints[1].localStartPos = new Vector3(-13.2f, 0.46f);
                spawnPoints[2].localStartPos = new Vector3(26.4f, -6f);
                spawnPoints[3].localStartPos = new Vector3(-26.4f, -6f);
            }
            else if (MapManager.instance.currentLevelID == 16)
            {
                spawnPoints[0].localStartPos = new Vector3(30.3f, -6.9f);
                spawnPoints[1].localStartPos = new Vector3(-30.3f, -6.9f);
                spawnPoints[2].localStartPos = new Vector3(17.7f, 1.6f);
                spawnPoints[3].localStartPos = new Vector3(-17.7f, 1.6f);
            }
            else if (MapManager.instance.currentLevelID == 17)
            {
                spawnPoints[0].localStartPos = new Vector3(24.8f, -12.6f);
                spawnPoints[1].localStartPos = new Vector3(-24.8f, -12.6f);
                spawnPoints[2].localStartPos = new Vector3(8f, 1.8f);
                spawnPoints[3].localStartPos = new Vector3(-8f, 1.8f);
            }
            else if (MapManager.instance.currentLevelID == 18)
            {
                spawnPoints[0].localStartPos = new Vector3(10f, 0.2f);
                spawnPoints[1].localStartPos = new Vector3(-10f, 0.2f);
                spawnPoints[2].localStartPos = new Vector3(20f, -14.5f);
                spawnPoints[3].localStartPos = new Vector3(-20f, -14.5f);
            }
            else if (MapManager.instance.currentLevelID == 19)
            {
                spawnPoints[0].localStartPos = new Vector3(17.8f, -9f);
                spawnPoints[1].localStartPos = new Vector3(-17.8f, -9f);
                spawnPoints[2].localStartPos = new Vector3(31.3f, 7.3f);
                spawnPoints[3].localStartPos = new Vector3(-31.3f, 7.3f);
            }
            else if (MapManager.instance.currentLevelID == 20)
            {
                spawnPoints[0].localStartPos = new Vector3(-7f, 2f);
                spawnPoints[1].localStartPos = new Vector3(7f, 2f);
                spawnPoints[2].localStartPos = new Vector3(-27f, -4f);
                spawnPoints[3].localStartPos = new Vector3(27f, -4f);
            }
            else if (MapManager.instance.currentLevelID == 21)
            {
                spawnPoints[0].localStartPos = new Vector3(-9f, -4f);
                spawnPoints[1].localStartPos = new Vector3(9f, -4f);
                spawnPoints[2].localStartPos = new Vector3(20f, -13f);
                spawnPoints[3].localStartPos = new Vector3(20f, -13f);
            }
            else if (MapManager.instance.currentLevelID == 22)
            {
                spawnPoints[0].localStartPos = new Vector3(-22.5f, 3f);
                spawnPoints[1].localStartPos = new Vector3(22.5f, 3f);
                spawnPoints[2].localStartPos = new Vector3(-10f, 0f);
                spawnPoints[3].localStartPos = new Vector3(10f, 0f);
            }
            else if (MapManager.instance.currentLevelID == 23)
            {
                spawnPoints[0].localStartPos = new Vector3(-12f, 3f);
                spawnPoints[1].localStartPos = new Vector3(12f, 3f);
                spawnPoints[2].localStartPos = new Vector3(-12f, -10f);
                spawnPoints[3].localStartPos = new Vector3(12f, -10f);
            }
            else if (MapManager.instance.currentLevelID == 24)
            {
                spawnPoints[0].localStartPos = new Vector3(-33f, -6f);
                spawnPoints[1].localStartPos = new Vector3(33f, -6f);
                spawnPoints[2].localStartPos = new Vector3(-13f, 2f);
                spawnPoints[3].localStartPos = new Vector3(13f, 2f);
            }
            else if (MapManager.instance.currentLevelID == 25)
            {
                spawnPoints[0].localStartPos = new Vector3(-25f, 5f);
                spawnPoints[1].localStartPos = new Vector3(25f, 5f);
                spawnPoints[2].localStartPos = new Vector3(-15f, 5f);
                spawnPoints[3].localStartPos = new Vector3(15f, 5f);
            }
            else if (MapManager.instance.currentLevelID == 26)
            {
                spawnPoints[0].localStartPos = new Vector3(-27f, 7f);
                spawnPoints[1].localStartPos = new Vector3(27f, 7f);
                spawnPoints[2].localStartPos = new Vector3(-27f, -5f);
                spawnPoints[3].localStartPos = new Vector3(27f, -5f);
            }
            else if (MapManager.instance.currentLevelID == 27)
            {
                spawnPoints[0].localStartPos = new Vector3(-22f, -1f);
                spawnPoints[1].localStartPos = new Vector3(22f, -1f);
                spawnPoints[2].localStartPos = new Vector3(-22f, 7f);
                spawnPoints[3].localStartPos = new Vector3(22f, 7f);
            }
            else if (MapManager.instance.currentLevelID == 28)
            {
                spawnPoints[0].localStartPos = new Vector3(-28f, -2f);
                spawnPoints[1].localStartPos = new Vector3(28f, -2f);
                spawnPoints[2].localStartPos = new Vector3(-28f, -10f);
                spawnPoints[3].localStartPos = new Vector3(28f, -10f);
            }
            else if (MapManager.instance.currentLevelID == 29)
            {
                spawnPoints[0].localStartPos = new Vector3(-32f, 3f);
                spawnPoints[1].localStartPos = new Vector3(32f, 3f);
                spawnPoints[2].localStartPos = new Vector3(-18f, 5f);
                spawnPoints[3].localStartPos = new Vector3(18f, 5f);
            }
            else if (MapManager.instance.currentLevelID == 30)
            {
                spawnPoints[0].localStartPos = new Vector3(-5f, -2f);
                spawnPoints[1].localStartPos = new Vector3(5f, -2f);
                spawnPoints[2].localStartPos = new Vector3(-20f, -6f);
                spawnPoints[3].localStartPos = new Vector3(-20f, -6f);
            }
            else if (MapManager.instance.currentLevelID == 31)
            {
                spawnPoints[0].localStartPos = new Vector3(-8f, 10f);
                spawnPoints[1].localStartPos = new Vector3(8f, 10f);
                spawnPoints[2].localStartPos = new Vector3(-16f, -15f);
                spawnPoints[3].localStartPos = new Vector3(16f, -15f);
            }
            else if (MapManager.instance.currentLevelID == 33)
            {
                spawnPoints[0].localStartPos = new Vector3(-32f, 5f);
                spawnPoints[1].localStartPos = new Vector3(32f, 5f);
                spawnPoints[2].localStartPos = new Vector3(-15f, 5f);
                spawnPoints[3].localStartPos = new Vector3(15f, 5f);
            }
            else if (MapManager.instance.currentLevelID == 34)
            {
                spawnPoints[0].localStartPos = new Vector3(-34f, 2f);
                spawnPoints[1].localStartPos = new Vector3(34f, 2f);
                spawnPoints[2].localStartPos = new Vector3(-13f, 2f);
                spawnPoints[3].localStartPos = new Vector3(13f, 2f);
            }
            else if (MapManager.instance.currentLevelID == 35)
            {
                spawnPoints[0].localStartPos = new Vector3(-30.5f, 2.9f);
                spawnPoints[1].localStartPos = new Vector3(30.5f, 2.9f);
                spawnPoints[2].localStartPos = new Vector3(-12.5f, 2.5f);
                spawnPoints[3].localStartPos = new Vector3(12.5f, 2.5f);
            }
            else if (MapManager.instance.currentLevelID == 36)
            {
                spawnPoints[0].localStartPos = new Vector3(-23.75f, -12.5f);
                spawnPoints[1].localStartPos = new Vector3(23.75f, -12.5f);
                spawnPoints[2].localStartPos = new Vector3(-9f, -12.5f);
                spawnPoints[3].localStartPos = new Vector3(9f, -12.5f);
            }
            else if (MapManager.instance.currentLevelID == 37)
            {
                spawnPoints[0].localStartPos = new Vector3(-13f, -16f);
                spawnPoints[1].localStartPos = new Vector3(13f, -16f);
                spawnPoints[2].localStartPos = new Vector3(-28f, 3.9f);
                spawnPoints[3].localStartPos = new Vector3(28f, 3.9f);
            }
            else if (MapManager.instance.currentLevelID == 38)
            {
                spawnPoints[0].localStartPos = new Vector3(-25f, -7f);
                spawnPoints[1].localStartPos = new Vector3(25f, -7f);
                spawnPoints[2].localStartPos = new Vector3(-10f, -3f);
                spawnPoints[3].localStartPos = new Vector3(10f, -3f);
            }
            else if (MapManager.instance.currentLevelID == 39)
            {
                spawnPoints[0].localStartPos = new Vector3(-23f, 0f);
                spawnPoints[1].localStartPos = new Vector3(23f, 0f);
                spawnPoints[2].localStartPos = new Vector3(-15f, -11f);
                spawnPoints[3].localStartPos = new Vector3(15f, -11f);
            }
            else if (MapManager.instance.currentLevelID == 40)
            {
                spawnPoints[0].localStartPos = new Vector3(-15f, -16f);
                spawnPoints[1].localStartPos = new Vector3(15f, -16f);
                spawnPoints[2].localStartPos = new Vector3(-28f, 0f);
                spawnPoints[3].localStartPos = new Vector3(28f, 0f);
            }
            else if (MapManager.instance.currentLevelID == 41)
            {
                spawnPoints[0].localStartPos = new Vector3(-25f, -4f);
                spawnPoints[1].localStartPos = new Vector3(25f, -4f);
                spawnPoints[2].localStartPos = new Vector3(-8f, 2.5f);
                spawnPoints[3].localStartPos = new Vector3(8f, 2.5f);
            }
            else if (MapManager.instance.currentLevelID == 42)
            {
                spawnPoints[0].localStartPos = new Vector3(-11.5f, -14f);
                spawnPoints[1].localStartPos = new Vector3(11.5f, -14f);
                spawnPoints[2].localStartPos = new Vector3(-30f, 6f);
                spawnPoints[3].localStartPos = new Vector3(30f, 6f);
            }
            else if (MapManager.instance.currentLevelID == 43)
            {
                spawnPoints[0].localStartPos = new Vector3(-20f, 7f);
                spawnPoints[1].localStartPos = new Vector3(20f, 7f);
                spawnPoints[2].localStartPos = new Vector3(-21f, -17f);
                spawnPoints[3].localStartPos = new Vector3(21f, -17f);
            }
            else if (MapManager.instance.currentLevelID == 44)
            {
                spawnPoints[0].localStartPos = new Vector3(-20f, -2f);
                spawnPoints[1].localStartPos = new Vector3(20f, 2f);
                spawnPoints[2].localStartPos = new Vector3(-27f, -17f);
                spawnPoints[3].localStartPos = new Vector3(-27f, -17f);
            }
            else if (MapManager.instance.currentLevelID == 45)
            {
                spawnPoints[0].localStartPos = new Vector3(-12f, 0f);
                spawnPoints[1].localStartPos = new Vector3(12f, 0f);
                spawnPoints[2].localStartPos = new Vector3(-26f, 0f);
                spawnPoints[3].localStartPos = new Vector3(26f, 0f);
            }
            else if (MapManager.instance.currentLevelID == 46)
            {
                spawnPoints[0].localStartPos = new Vector3(-22f, -4f);
                spawnPoints[1].localStartPos = new Vector3(22f, 4f);
                spawnPoints[2].localStartPos = new Vector3(-7f, -4f);
                spawnPoints[3].localStartPos = new Vector3(7f, -4f);
            }
            else if (MapManager.instance.currentLevelID == 47)
            {
                spawnPoints[0].localStartPos = new Vector3(-14.5f, -10f);
                spawnPoints[1].localStartPos = new Vector3(14.5f, 10f);
                spawnPoints[2].localStartPos = new Vector3(-9f, 3.8f);
                spawnPoints[3].localStartPos = new Vector3(9f, 3.8f);
            }
            else if (MapManager.instance.currentLevelID == 48)
            {
                spawnPoints[0].localStartPos = new Vector3(-31f, 2.5f);
                spawnPoints[1].localStartPos = new Vector3(31f, 2.5f);
                spawnPoints[2].localStartPos = new Vector3(-20f, 0f);
                spawnPoints[3].localStartPos = new Vector3(20f, 0f);
            }
            else if (MapManager.instance.currentLevelID == 49)
            {
                spawnPoints[0].localStartPos = new Vector3(-26f, 5.5f);
                spawnPoints[1].localStartPos = new Vector3(26f, 5.5f);
                spawnPoints[2].localStartPos = new Vector3(-19f, -10f);
                spawnPoints[3].localStartPos = new Vector3(19f, -10f);
            }
            else if (MapManager.instance.currentLevelID == 51)
            {
                spawnPoints[0].localStartPos = new Vector3(-23.5f, -13f);
                spawnPoints[1].localStartPos = new Vector3(23.5f, -13f);
                spawnPoints[2].localStartPos = new Vector3(-19f, 2f);
                spawnPoints[3].localStartPos = new Vector3(19f, 2f);
            }
            else if (MapManager.instance.currentLevelID == 52)
            {
                spawnPoints[0].localStartPos = new Vector3(-28f, -4f);
                spawnPoints[1].localStartPos = new Vector3(28f, -4f);
                spawnPoints[2].localStartPos = new Vector3(-10f, -14f);
                spawnPoints[3].localStartPos = new Vector3(10f, -14f);
            }
            else if (MapManager.instance.currentLevelID == 53)
            {
                spawnPoints[0].localStartPos = new Vector3(-15f, -7f);
                spawnPoints[1].localStartPos = new Vector3(15f, -7f);
                spawnPoints[2].localStartPos = new Vector3(-15f, 9f);
                spawnPoints[3].localStartPos = new Vector3(15f, 9f);
            }
            else if (MapManager.instance.currentLevelID == 54)
            {
                spawnPoints[0].localStartPos = new Vector3(-30f, 0f);
                spawnPoints[1].localStartPos = new Vector3(30f, 0f);
                spawnPoints[2].localStartPos = new Vector3(-28f, -10f);
                spawnPoints[3].localStartPos = new Vector3(28f, -10f);
            }
            else if (MapManager.instance.currentLevelID == 55)
            {
                spawnPoints[0].localStartPos = new Vector3(-28f, -10f);
                spawnPoints[1].localStartPos = new Vector3(28f, -10f);
                spawnPoints[2].localStartPos = new Vector3(-10f, -4f);
                spawnPoints[3].localStartPos = new Vector3(10f, -4f);
            }
            else if (MapManager.instance.currentLevelID == 56)
            {
                spawnPoints[0].localStartPos = new Vector3(-18.5f, 1f);
                spawnPoints[1].localStartPos = new Vector3(18.5f, 1f);
                spawnPoints[2].localStartPos = new Vector3(-8.5f, 0f);
                spawnPoints[3].localStartPos = new Vector3(8.5f, 0f);
            }
            else if (MapManager.instance.currentLevelID == 57)
            {
                spawnPoints[0].localStartPos = new Vector3(-4f, 7f);
                spawnPoints[1].localStartPos = new Vector3(4f, 7f);
                spawnPoints[2].localStartPos = new Vector3(-21f, 4f);
                spawnPoints[3].localStartPos = new Vector3(21f, 4f);
            }
            else if (MapManager.instance.currentLevelID == 58)
            {
                spawnPoints[0].localStartPos = new Vector3(-21f, 5f);
                spawnPoints[1].localStartPos = new Vector3(21f, 5f);
                spawnPoints[2].localStartPos = new Vector3(-21f, -12f);
                spawnPoints[3].localStartPos = new Vector3(21f, -12f);
            }
            else if (MapManager.instance.currentLevelID == 59)
            {
                spawnPoints[0].localStartPos = new Vector3(-30f, -7.5f);
                spawnPoints[1].localStartPos = new Vector3(30f, 7.5f);
                spawnPoints[2].localStartPos = new Vector3(-12f, 2.45f);
                spawnPoints[3].localStartPos = new Vector3(12f, 2.45f);
            }
            else if (MapManager.instance.currentLevelID == 60)
            {
                spawnPoints[0].localStartPos = new Vector3(-21.5f, 4f);
                spawnPoints[1].localStartPos = new Vector3(21.5f, 4f);
                spawnPoints[2].localStartPos = new Vector3(-21.5f, -3f);
                spawnPoints[3].localStartPos = new Vector3(21.5f, -3f);
            }
            else if (MapManager.instance.currentLevelID == 61)
            {
                spawnPoints[0].localStartPos = new Vector3(-32f, 2f);
                spawnPoints[1].localStartPos = new Vector3(32f, 2f);
                spawnPoints[2].localStartPos = new Vector3(-32f, -7f);
                spawnPoints[3].localStartPos = new Vector3(32f, -7f);
            }
            else if (MapManager.instance.currentLevelID == 62)
            {
                spawnPoints[0].localStartPos = new Vector3(-15f, 1f);
                spawnPoints[1].localStartPos = new Vector3(15f, 1f);
                spawnPoints[2].localStartPos = new Vector3(-32f, -7f);
                spawnPoints[3].localStartPos = new Vector3(32f, -7f);
            }
            else if (MapManager.instance.currentLevelID == 63)
            {
                spawnPoints[0].localStartPos = new Vector3(-16.5f, -3f);
                spawnPoints[1].localStartPos = new Vector3(16.5f, 3f);
                spawnPoints[2].localStartPos = new Vector3(-17f, -10f);
                spawnPoints[3].localStartPos = new Vector3(17f, -10f);
            }
            else if (MapManager.instance.currentLevelID == 64)
            {
                spawnPoints[0].localStartPos = new Vector3(-10f, -5f);
                spawnPoints[1].localStartPos = new Vector3(10f, -5f);
                spawnPoints[2].localStartPos = new Vector3(-10f, 5f);
                spawnPoints[3].localStartPos = new Vector3(10f, 5f);
            }
            else if (MapManager.instance.currentLevelID == 65)
            {
                spawnPoints[0].localStartPos = new Vector3(-10f, 3f);
                spawnPoints[1].localStartPos = new Vector3(10f, 3f);
                spawnPoints[2].localStartPos = new Vector3(-10f, -5f);
                spawnPoints[3].localStartPos = new Vector3(10f, -5f);
            }
            else if (MapManager.instance.currentLevelID == 66)
            {
                spawnPoints[0].localStartPos = new Vector3(-20f, -9f);
                spawnPoints[1].localStartPos = new Vector3(20f, -9f);
                spawnPoints[2].localStartPos = new Vector3(-27f, -9f);
                spawnPoints[3].localStartPos = new Vector3(27f, -9f);
            }
            else if (MapManager.instance.currentLevelID == 67)
            {
                spawnPoints[0].localStartPos = new Vector3(-26f, -17f);
                spawnPoints[1].localStartPos = new Vector3(26f, 17f);
                spawnPoints[2].localStartPos = new Vector3(-26f, -4f);
                spawnPoints[3].localStartPos = new Vector3(26f, 4f);
            }
            else if (MapManager.instance.currentLevelID == 68)
            {
                spawnPoints[0].localStartPos = new Vector3(-15f, 0f);
                spawnPoints[1].localStartPos = new Vector3(15f, 0f);
                spawnPoints[2].localStartPos = new Vector3(-22f, 0f);
                spawnPoints[3].localStartPos = new Vector3(22f, 0f);
            }
            else if (MapManager.instance.currentLevelID == 69)
            {
                spawnPoints[0].localStartPos = new Vector3(-17f, -8f);
                spawnPoints[1].localStartPos = new Vector3(17f, -8f);
                spawnPoints[2].localStartPos = new Vector3(-26.5f, -3f);
                spawnPoints[3].localStartPos = new Vector3(26.5f, -3f);
            }
        }
    }
}
