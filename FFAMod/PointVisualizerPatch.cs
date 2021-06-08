using HarmonyLib;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace FFAMod
{
    [HarmonyPatch(typeof(PointVisualizer))]
    internal class PointVisualizerPatch
    {
        [HarmonyPatch("DoShowPoints")]
        private static void Prefix()
        {
            var instance = PointVisualizer.instance;
            // Reset colors
            for (int i = 1; i <= 3; i++)
            {
                instance.orangeBall.GetChild(i).GetComponent<ProceduralImage>().color = new Color(0.9176f, 0.3601f, 0f);
                instance.blueBall.GetChild(i).GetComponent<ProceduralImage>().color = new Color(0.1701f, 0.4324f, 0.9245f);
            }
        }

        [HarmonyPatch("DoShowPoints")]
        private static void Postfix()
        {
            var roundCounterSmall = (RoundCounter)AccessTools.Field(typeof(UIHandler), "roundCounterSmall").GetValue(UIHandler.instance);
            if (roundCounterSmall.transform.Find("P3") == null && PlayerManager.instance.players.Count >= 3)
            {
                GameObject childObject = Object.Instantiate(GameObject.Find("P1"), roundCounterSmall.gameObject.transform);
                childObject.name = "P3";
                childObject.transform.position = GameObject.Find("P1").transform.position + Vector3.down * 2.15f;
                Object.Destroy(childObject.GetComponent<Populate>());
                var children = childObject.GetComponentsInChildren<ProceduralImage>();
                foreach (var child in children)
                {
                    child.GetComponent<ProceduralImage>().color = new Color(0.3387f, 0.3696f, 0.4057f);
                }
                if (PlayerManager.instance.players.Count == 4)
                {
                    GameObject childObject2 = Object.Instantiate(GameObject.Find("P2"), roundCounterSmall.gameObject.transform);
                    childObject2.name = "P4";
                    childObject2.transform.position = GameObject.Find("P2").transform.position + Vector3.down * 2.15f;
                    Object.Destroy(childObject2.GetComponent<Populate>());
                    var children2 = childObject2.GetComponentsInChildren<ProceduralImage>();
                    foreach (var child in children2)
                    {
                        child.GetComponent<ProceduralImage>().color = new Color(0.3387f, 0.3696f, 0.4057f);
                    }
                }
            }
            var instance = PointVisualizer.instance;
            if (GM_ArmsRacePatch.winningTeamID == 2)
            {
                // Red Color
                for (int i = 1; i <= 3; i++)
                {
                    instance.orangeBall.GetChild(i).GetComponent<ProceduralImage>().color = new Color(0.8627f, 0.0784f, 0.2353f);
                }
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(2).winText;
                if (GM_ArmsRacePatch.p3Points == 1)
                {
                    instance.orangeFill.fillAmount = 0.5f;
                    HalfRed();
                    foreach (var child in roundCounterSmall.transform.Find("P3").GetComponentsInChildren<ProceduralImage>())
                    {
                        if (child.GetComponent<ProceduralImage>().color == new Color(0.3387f, 0.3696f, 0.4057f))
                        {
                            child.GetComponent<ProceduralImage>().color = PlayerSkinBank.GetPlayerSkinColors(2).winText;
                            break;
                        }
                    }
                    return;
                }
                instance.orangeFill.fillAmount = 1f;
                RoundRed();
                foreach (var child in roundCounterSmall.transform.Find("P3").GetComponentsInChildren<ProceduralImage>())
                {
                    if (child.transform.localScale == new Vector3(0.3f, 0.3f, 0.3f) && child.GetComponent<ProceduralImage>().color != new Color(0.3387f, 0.3696f, 0.4057f))
                    {
                        child.transform.localScale = new Vector3(1, 1, 1);
                        break;
                    }
                }
                return;
            }
            else if (GM_ArmsRacePatch.winningTeamID == 3)
            {
                // Green color
                for (int i = 1; i <= 3; i++)
                {
                    instance.blueBall.GetChild(i).GetComponent<ProceduralImage>().color = new Color(0.1961f, 0.8039f, 0.1961f);
                }
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(3).winText;
                if (GM_ArmsRacePatch.p4Points == 1)
                {
                    instance.blueFill.fillAmount = 0.5f;
                    HalfGreen();
                    foreach (var child in roundCounterSmall.transform.Find("P4").GetComponentsInChildren<ProceduralImage>())
                    {
                        if (child.GetComponent<ProceduralImage>().color == new Color(0.3387f, 0.3696f, 0.4057f))
                        {
                            child.GetComponent<ProceduralImage>().color = PlayerSkinBank.GetPlayerSkinColors(3).winText;
                            break;
                        }
                    }
                    return;
                }
                instance.blueFill.fillAmount = 1f;
                RoundGreen();
                foreach (var child in roundCounterSmall.transform.Find("P4").GetComponentsInChildren<ProceduralImage>())
                {
                    if (child.transform.localScale == new Vector3(0.3f, 0.3f, 0.3f) && child.GetComponent<ProceduralImage>().color != new Color(0.3387f, 0.3696f, 0.4057f))
                    {
                        child.transform.localScale = new Vector3(1, 1, 1);
                        break;
                    }
                }
                return;
            }
        }

        private static void HalfRed()
        {
            PointVisualizer.instance.text.text = "HALF RED";
        }

        private static void RoundRed()
        {
            PointVisualizer.instance.text.text = "ROUND RED";
        }

        private static void HalfGreen()
        {
            PointVisualizer.instance.text.text = "HALF GREEN";
        }

        private static void RoundGreen()
        {
            PointVisualizer.instance.text.text = "ROUND GREEN";
        }
    }
}
