using HarmonyLib;

namespace FFAMod
{
    [HarmonyPatch(typeof(PointVisualizer))]
    internal class PointVisualizerPatch : PointVisualizer
    {
        [HarmonyPatch("DoShowPoints")]
        private static void Postfix()
        {
            if (GM_ArmsRacePatch.WinningTeamID == 2)
            {
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(2).winText;
            }
            else if (GM_ArmsRacePatch.WinningTeamID == 3)
            {
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(3).winText;
            }
            if (GM_ArmsRacePatch.WinningTeamID == 2)
            {
                if (GM_ArmsRacePatch.P3Points == 1)
                {
                    HalfRed();
                    return;
                }
                RoundRed();
                return;
            }
            else if (GM_ArmsRacePatch.WinningTeamID == 3)
            {
                if (GM_ArmsRacePatch.P4Points == 1)
                {
                    HalfGreen();
                    return;
                }
                RoundGreen();
                return;
            }
        }

        private static void HalfRed()
        {
            instance.text.text = "HALF RED";
        }

        private static void RoundRed()
        {
            instance.text.text = "ROUND RED";
        }

        private static void HalfGreen()
        {
            instance.text.text = "HALF GREEN";
        }

        private static void RoundGreen()
        {
            instance.text.text = "ROUND GREEN";
        }
    }
}
