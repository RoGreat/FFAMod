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
        private static void Prefix()
        {
            if (PlayerManager.instance.players.Count < 3)
                return;
            if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[0])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-30f, -5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-9f, -13f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(9f, -13f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(30f, -5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[1])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, 12f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-30f, -5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, 12f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(30f, -5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[2])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26f, 12f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-24f, -11f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(26f, 12f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(24f, -11f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[3])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27f, -14f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-11f, 4f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(11f, 4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(27f, -14f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[4])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26f, -13f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-13f, 3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(13f, 3f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(26f, -13f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[5])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22f, 6f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22f, -13f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(22f, 6f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(22f, -13f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[6])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(7f, 5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(25f, -11f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-7f, 5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-25f, -11f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[7])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-29f, -5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(29f, -5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-9f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(9f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[8])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(23f, 9.6f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-23f, 9.6f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(18f, 7.4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-18f, 7.4f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[9])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(25f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(10.5f, 3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-25f, -0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10.5f, 3f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[10])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(22.8f, -11f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(17f, 1.3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22.8f, -11f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-17f, 1.3f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[11])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(8.7f, 4f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(14f, -10f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-8.7f, 4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-14f, -10);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[12])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(26f, -7.5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26f, -7.5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(16.8f, 6.6f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-16.8f, 6.6f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[13])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(18.4f, -4.5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-18.4f, -4.5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(25.7f, 8.5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-25.7f, 8.5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[14])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(27.3f, 3.3f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27.3f, 3.3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(16.5f, 4.5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-16.5f, 4.5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[15])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(13.2f, 0.46f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-13.2f, 0.46f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(26.4f, -6f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26.4f, -6f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[16])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(30.3f, -6.9f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-30.3f, -6.9f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(17.7f, 1.6f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-17.7f, 1.6f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[17])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(24.8f, -12.6f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-24.8f, -12.6f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(8f, 1.8f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-8f, 1.8f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[18])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, 0.2f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, 0.2f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, -14.5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, -14.5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[19])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(17.8f, -9f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(-17.8f, -9f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(31.3f, 7.3f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-31.3f, 7.3f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[20])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-7f, 2f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(7f, 2f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27f, -4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(27f, -4f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[21])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-9f, -4f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(9f, -4f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, -13f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, -13f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[22])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22.5f, 3f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(22.5f, 3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[23])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-12f, 3f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(12f, 3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-12f, -10f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(12f, -10f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[24])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-33f, -6f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(33f, -6f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-13f, 2f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(13f, 2f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[25])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-25f, 5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(25f, 5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, 5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, 5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[26])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27f, 7f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(27f, 7f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27f, -5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(27f, -5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[27])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22f, -1f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(22f, -1f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22f, 7f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(22f, 7f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[28])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-28f, -2f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(28f, -2f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-28f, -10f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(28f, -10f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[29])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-32f, 3f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(32f, 3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-18f, 5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(18f, 5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[30])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-5f, -2f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(5f, -2f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, -6f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, -6f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[31])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-8f, 10f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(8f, 10f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-16f, -15f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(16f, -15f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[33])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-32f, 5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(32f, 5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, 5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, 5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[34])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-34f, 2f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(34f, 2f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-13f, 2f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(13f, 2f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[35])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-30.5f, 2.9f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(30.5f, 2.9f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-12.5f, 2.5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(12.5f, 2.5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[36])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-23.75f, -12.5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(23.75f, -12.5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-9f, -12.5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(9f, -12.5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[37])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-13f, -16f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(13f, -16f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-28f, 3.9f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(28f, 3.9f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[38])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-25f, -7f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(25f, -7f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, -3f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, -3f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[39])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-23f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(23f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, -11f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, -11f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[40])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, -16f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, -16f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-28f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(28f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[41])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-25f, -4f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(25f, -4f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-8f, 2.5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(8f, 2.5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[42])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-11.5f, -15f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(11.5f, -15f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-30f, 6f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(30f, 6f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[43])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, 7f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, 7f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-21f, -17f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(21f, -17f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[44])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, -2f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, 2f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27f, -17f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27f, -17f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[45])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-12f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(12f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(26f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[46])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22f, -4f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(22f, 4f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-7f, -4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(7f, -4f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[47])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-14.5f, -10f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(14.5f, 10f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-9f, 3.8f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(9f, 3.8f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[48])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-31f, 2.5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(31f, 2.5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[49])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26f, 5.5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(26f, 5.5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-19f, -10f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(19f, -10f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[51])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-23.5f, -13f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(23.5f, -13f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-19f, 2f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(19f, 2f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[52])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-28f, -4f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(28f, -4f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, -14f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, -14f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[53])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, -7f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, -7f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, 9f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, 9f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[54])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-30f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(30f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-28f, -10f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(28f, -10f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[55])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-28f, -10f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(28f, -10f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, -4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, -4f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[56])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-18.5f, 1f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(18.5f, 1f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-8.5f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(8.5f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[57])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-4f, 7f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(4f, 7f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-21f, 4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(21f, 4f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[58])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-21f, 5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(21f, 5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-21f, -12f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(21f, -12f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[59])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-30f, -7.5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(30f, 7.5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-12f, 2.45f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(12f, 2.45f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[60])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-21.5f, 4f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(21.5f, 4f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-21.5f, -3f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(21.5f, -3f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[61])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-32f, 2f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(32f, 2f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-32f, -7f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(32f, -7f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[62])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, 1f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, 1f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-32f, -7f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(32f, -7f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[63])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-16.5f, -3f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(16.5f, 3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-17f, -10f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(17f, -10f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[64])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, -5f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, -5f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, 5f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, 5f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[65])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, 3f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, 3f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-10f, -8f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(10f, -8f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[66])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-20f, -9f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(20f, -9f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-27f, -9f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(27f, -9f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[67])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26f, -17f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(26f, 17f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26f, -4f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(26f, 4f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[68])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-15f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(15f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-22f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(22f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[69])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(-17f, -8f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(17f, -8f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(-26.5f, -3f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(26.5f, -3f);
            }
        }
    }
}
