﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FFAMod
{
    [HarmonyPatch(typeof(PlayerManager))]
    internal class PlayerManagerPatch : PlayerManager
    {
        [HarmonyPatch("GetOtherTeam")]
        private static bool Prefix(ref int __result, int team)
        {
            __result = GetOtherTeamPatch(team);
            return false;
        }

        public static int GetOtherTeamPatch(int team, int offset = 1)
        {
            int[] array;
            if (instance.players.Count == 4)
            {
                array = new int[4] { 0, 1, 2, 3 };
            }
            else if (instance.players.Count == 3)
            {
                array = new int[3] { 0, 1, 2 };
            }
            else
            {
                array = new int[2] { 0, 1 };
            }
            return array[(team + offset) % array.Length];
        }

        [HarmonyPatch("GetClosestPlayerInTeam")]
        private static bool Prefix(ref Player __result, Vector3 position, int team, bool needVision = false)
        {
            __result = GetClosestPlayerInTeamPatch(position, team, needVision);
            return false;
        }

        private static Player GetClosestPlayerInTeamPatch(Vector3 position, int team, bool needVision = false)
        {
            switch (instance.players.Count)
            {
                case 2:
                    team = GetOtherTeamPatch(team, 1);
                    break;
                case 3:
                    team = GetOtherTeamPatch(team, 2);
                    break;
                case 4:
                    team = GetOtherTeamPatch(team, 3);
                    break;
                default:
                    team = 0;
                    break;
            }
            float num = float.MaxValue;
            float num2 = 0;
            Player[] players = instance.players.ToArray();
            Player result = null;
            var dictionary = new Dictionary<int, float>();
            for (int i = 0; i < players.Length; i++)
            {
                num2 = Vector2.Distance(position, players[i].transform.position);
                if (!players[i].data.dead && players[i].teamID != team)
                    dictionary.Add(i, num2);
            }
            if (dictionary.Count == 0)
                return result;
            int j = dictionary.OrderBy(x => x.Value).First().Key;
            num2 = dictionary.OrderBy(x => x.Value).First().Value;
            if ((!needVision || instance.CanSeePlayer(position, players[j]).canSee) && num2 < num)
            {
                result = players[j];
            }
            return result;
        }

        public static int TeamsAlivePatch()
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            for (int i = 0; i < instance.players.Count; i++)
            {
                if (instance.players[i].teamID == 0 && !instance.players[i].data.dead)
                {
                    flag = true;
                }
                if (instance.players[i].teamID == 1 && !instance.players[i].data.dead)
                {
                    flag2 = true;
                }
                if (instance.players[i].teamID == 2 && !instance.players[i].data.dead)
                {
                    flag3 = true;
                }
                if (instance.players[i].teamID == 3 && !instance.players[i].data.dead)
                {
                    flag4 = true;
                }
            }
            int num = 0;
            if (flag)
            {
                num++;
            }
            if (flag2)
            {
                num++;
            }
            if (flag3)
            {
                num++;
            }
            if (flag4)
            {
                num++;
            }
            return num;
        }
    }
}
