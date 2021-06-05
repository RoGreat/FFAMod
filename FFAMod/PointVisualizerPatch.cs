using HarmonyLib;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace FFAMod
{
    [HarmonyPatch(typeof(PointVisualizer))]
    internal class PointVisualizerPatch
    {
        [HarmonyPatch("DoShowPoints")]
        private static void Postfix()
        {
            if (GameObject.Find("P3") == null)
            {
            GameObject childObject = GameObject.Instantiate(GameObject.Find("P1")) as GameObject;
            childObject.transform.parent = GameObject.Find("RoundsSmall").transform;
            childObject.name = "P3";
            childObject.transform.localScale = new Vector3(1,1,1);
            childObject.transform.position = new Vector3(-34.1654f, 16.2717f, -95f);
            childObject.transform.localPosition = new Vector3(-79.3617f, -43.7602f, 0f);
            GameObject.Destroy(childObject.GetComponent<Populate>());
            var children = childObject.GetComponentsInChildren<ProceduralImage>();
            foreach (var child in children)
            {
                child.GetComponent<ProceduralImage>().color = new Color(0.3387f, 0.3696f, 0.4057f);
            }
            GameObject childObject2 = GameObject.Instantiate(GameObject.Find("P2")) as GameObject;
            childObject2.transform.parent = GameObject.Find("RoundsSmall").transform;
            childObject2.name = "P4";
            childObject2.transform.localScale = new Vector3(1,1,1);
            childObject2.transform.position = new Vector3(-33.8455f, 15.0724f, -95f);
            childObject2.transform.localPosition = new Vector3(-79.3617f, -76.1387f, 0f);
            GameObject.Destroy(childObject2.GetComponent<Populate>());
            var children2 = childObject2.GetComponentsInChildren<ProceduralImage>();
            foreach (var child in children2)
            {
                child.GetComponent<ProceduralImage>().color = new Color(0.3387f, 0.3696f, 0.4057f);
            }
            }
            var instance = PointVisualizer.instance;
            if (GM_ArmsRacePatch.winningTeamID == 2)
            {
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(2).winText;
                if (GM_ArmsRacePatch.p3Points == 1)
                {
                    instance.orangeFill.fillAmount = 0.5f;
                    HalfRed();
                    foreach (var child in GameObject.Find("P3").GetComponentsInChildren<ProceduralImage>())
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
                foreach (var child in GameObject.Find("P3").GetComponentsInChildren<ProceduralImage>())
                {
                    if (child.transform.localScale == new Vector3(0.3f,0.3f,0.3f))
                    {
                        child.transform.localScale = new Vector3(1,1,1);
                        break;
                    }
                }
                return;
            }
            else if (GM_ArmsRacePatch.winningTeamID == 3)
            {
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(3).winText;
                if (GM_ArmsRacePatch.p4Points == 1)
                {
                    instance.blueFill.fillAmount = 0.5f;
                    HalfGreen();
                    foreach (var child in GameObject.Find("P4").GetComponentsInChildren<ProceduralImage>())
                    {
                        if (child.GetComponent<ProceduralImage>().color == new Color(0.3387f, 0.3696f, 0.4057f))
                        {
                            child.GetComponent<ProceduralImage>().color = PlayerSkinBank.GetPlayerSkinColors(3).winText;
                        }
                    }
                    return;
                }
                instance.blueFill.fillAmount = 1f;
                RoundGreen();
                foreach (var child in GameObject.Find("P4").GetComponentsInChildren<ProceduralImage>())
                {
                    if (child.transform.localScale == new Vector3(0.3f,0.3f,0.3f))
                    {
                        child.transform.localScale = new Vector3(1,1,1);
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
