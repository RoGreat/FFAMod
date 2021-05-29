using HarmonyLib;
using Sonigon;

namespace FFAMod
{
    [HarmonyPatch(typeof(ApplyCardStats))]
    class ApplyCardStatsPatches
    {
        [HarmonyPatch("OFFLINE_Pick")]
        private static bool Prefix(Player[] players, ref Player ___playerToUpgrade, ApplyCardStats __instance)
        {
            // Have to skip AddCard for now
            if (players[0].teamID > 1)
            {
                ___playerToUpgrade = players[0];
                AccessTools.Method(typeof(ApplyCardStats), "ApplyStats").Invoke(__instance, null);
                CardBarPatch.play = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch("RPCA_Pick")]
        private static bool Prefix(int[] actorIDs, ref Player ___playerToUpgrade, ApplyCardStats __instance)
        {
            // Have to skip AddCard for now
            Player playerActorID = (Player)AccessTools.Method(typeof(PlayerManager), "GetPlayerWithActorID").Invoke(PlayerManager.instance, new object[] { actorIDs[0] });
            if (playerActorID.teamID > 1)
            {
                ___playerToUpgrade = playerActorID;
                AccessTools.Method(typeof(ApplyCardStats), "ApplyStats").Invoke(__instance, null);
                CardBarPatch.play = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CardBar))]
    class CardBarPatch
    {
        [HarmonyPatch("Update")]
        private static void Postfix(CardBar __instance)
        {
            if (play)
            {
                SoundManager.Instance.Play(__instance.soundCardPick, __instance.transform);
                play = false;
            }
        }

        public static bool play;
    }
}
