using HarmonyLib;
using UnityEngine;

namespace FFAMod
{
    [HarmonyPatch(typeof(PlayerManager))]
    internal class PlayerManagerPatch : PlayerManager
    {
        [HarmonyPatch("GetClosestPlayerInTeam")]
        private static bool Prefix(ref Player __result, Vector3 position, int team, bool needVision = false)
        {
            __result = GetClosestPlayerInTeamPatch(position, team, needVision);
            return false;
        }

        private static Player GetClosestPlayerInTeamPatch(Vector3 position, int team, bool needVision = false)
        {
            float num = float.MaxValue;
            float num2 = 0;
            int index = 0;
            Player result = null;
            var array = new float[4];
            for (int i = 0; i < instance.players.Count; i++)
            {
                num2 = Vector2.Distance(position, instance.players[i].transform.position);
                if (!instance.players[i].data.dead && instance.players[i].playerID != team - 1)
                {
                    array[i] = num2;
                }
            }
            for (int i = 1; i < instance.players.Count; i++)
            {
                if (array[i] < array[0])
                {
                    num2 = array[i];
                    index = i;
                }
            }
            if ((!needVision || instance.CanSeePlayer(position, instance.players[index]).canSee) && num2 < num)
            {
                num = num2;
                result = instance.players[index];
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
    }
}
