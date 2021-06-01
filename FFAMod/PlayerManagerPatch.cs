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
            if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[0])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[1])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[2])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[3])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[4])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[5])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[6])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[7])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[8])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[9])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[10])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[11])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[12])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[13])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[14])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[15])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[16])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[17])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[18])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[19])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[20])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[21])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[22])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[23])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[24])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[25])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[26])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[27])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[28])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[29])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[30])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[31])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[32])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[33])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[34])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[35])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[36])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[37])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[38])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[39])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[40])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[41])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[42])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[43])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[44])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[45])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[46])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[47])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[48])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[49])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[50])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[51])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[52])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[53])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[54])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[55])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[56])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[57])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[58])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[59])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[60])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[61])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[62])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[63])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[64])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[65])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[66])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[67])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[68])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
            else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[69])
            {
                GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
                GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
            }
=======
                [HarmonyPatch("MovePlayers")]
        private static bool Prefix()
        {
if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[0])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[1])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[2])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[3])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[4])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[5])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[6])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[7])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[8])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[9])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[10])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[11])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[12])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[13])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[14])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[15])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[16])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[17])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[18])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[19])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[20])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[21])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[22])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[23])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[24])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[25])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[26])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[27])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[28])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[29])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[30])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[31])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[32])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[33])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[34])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[35])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[36])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[37])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[38])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[39])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[40])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[41])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[42])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[43])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[44])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[45])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[46])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[47])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[48])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[49])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[50])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[51])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[52])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[53])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[54])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[55])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[56])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[57])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[58])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[59])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[60])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[61])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[62])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[63])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[64])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[65])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[66])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[67])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[68])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
else if (GameObject.Find("SPAWN POINT 1").scene.name == MapManager.instance.levels[69])
          {
              GameObject.Find("SPAWN POINT 1").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 2").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 3").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
              GameObject.Find("SPAWN POINT 4").GetComponent<SpawnPoint>().localStartPos = new Vector3(0f, 0f);
          }
        }
    }
}
