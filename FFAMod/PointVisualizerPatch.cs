/*
using Photon.Pun;
using HarmonyLib;

namespace FFAMod
{
    [HarmonyPatch(typeof(PointVisualizer))]
    internal partial class PointVisualizerPatch : PointVisualizer
    {
        private static PointVisualizerPatch instancePatch;

        private void Awake()
        {
            instancePatch = this;
        }

        [HarmonyPatch("DoShowPoints")]
        private static void Postfix()
        {
            if (GM_ArmsRacePatch.winningTeamID == 2)
            {
                if (GM_ArmsRacePatch.p3Points > 1)
                {
                    instancePatch.RoundRed();
                    return;
                }
                instancePatch.HalfRed();
                return;
            }
            else if (GM_ArmsRacePatch.winningTeamID == 3)
            {
                if (GM_ArmsRacePatch.p4Points > 1)
                {
                    instancePatch.RoundYellow();
                    return;
                }
                instancePatch.HalfYellow();
                return;
            }
        }

        private void HalfRed()
        {
            text.text = "HALF RED";
        }

        private void RoundRed()
        {
            text.text = "HALF RED";
        }

        private void HalfYellow()
        {
            text.text = "HALF YELLOW";
        }

        private void RoundYellow()
        {
            text.text = "HALF YELLOW";
        }
    }
}
*/
