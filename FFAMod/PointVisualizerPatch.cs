using HarmonyLib;

namespace FFAMod
{
    [HarmonyPatch(typeof(PointVisualizer))]
    internal class PointVisualizerPatch : PointVisualizer
    {
        [HarmonyPatch("DoShowPoints")]
        private static void Postfix()
        {
            if (GM_ArmsRacePatch.winningTeamID == 2)
            {
                if (GM_ArmsRacePatch.p3Points > 1)
                {
                    RoundRed();
                    return;
                }
                HalfRed();
                return;
            }
            else if (GM_ArmsRacePatch.winningTeamID == 3)
            {
                if (GM_ArmsRacePatch.p4Points > 1)
                {
                    RoundGreen();
                    return;
                }
                HalfGreen();
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
