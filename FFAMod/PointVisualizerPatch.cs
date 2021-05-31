using HarmonyLib;
using Sonigon;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FFAMod
{
    [HarmonyPatch(typeof(PointVisualizer))]
    internal class PointVisualizerPatch
    {
        /*
        public static Transform redBall;
        public static Transform greenBall;
        private static RectTransform redBallRT;
        private static RectTransform greenBallRT;
        public static Image redFill;
        public static Image greenFill;
        private static Vector3 redVel;
        private static Vector3 greenVel;
        private static Vector3 redSP;
        private static Vector3 greenSP;
        private static float ballBaseSize = 200f;
        private static float ballSmallSize = 20f;
        private static float bigBallScale = 900f;

        public static PointVisualizerPatch instance;
        private static int orangePoints;
        private static int bluePoints;
        private static int orangeRounds;
        private static int blueRounds;

        [HarmonyPatch("Start")]
        private static void Prefix()
        {
            redBallRT = redBall.GetComponent<RectTransform>();
            greenBallRT = greenBall.GetComponent<RectTransform>();
            redSP = redBall.GetComponent<RectTransform>().anchoredPosition + Vector2.down * 0.5f;
            greenSP = greenBall.GetComponent<RectTransform>().anchoredPosition + Vector2.down * 0.5f;
        }

        [HarmonyPatch("DoWinSequence")]
        private static bool Prefix(ref IEnumerator __result, int orangePoints, int bluePoints, int orangeRounds, int blueRounds)
        {
            PointVisualizerPatch.orangePoints = orangePoints;
            PointVisualizerPatch.bluePoints = bluePoints;
            PointVisualizerPatch.orangeRounds = orangeRounds;
            PointVisualizerPatch.blueRounds = blueRounds;
            __result = DoWinSequence();
            return false;
        }

        private static IEnumerator DoWinSequence()
        {
            var instance = PointVisualizer.instance;
            yield return new WaitForSecondsRealtime(0.35f);
            SoundManager.Instance.Play(instance.soundWinRound, instance.transform);
            instance.ResetBalls();
            instance.bg.SetActive(true);
            instance.blueBall.gameObject.SetActive(true);
            instance.orangeBall.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.2f);
            GamefeelManager.instance.AddUIGameFeelOverTime(10f, 0.1f);
            instance.DoShowPoints(orangePoints, bluePoints, orangeWinner);
            yield return new WaitForSecondsRealtime(0.35f);
            SoundManager.Instance.Play(instance.sound_UI_Arms_Race_A_Ball_Shrink_Go_To_Left_Corner, instance.transform);
            float c = 0f;
            while (c < instance.timeToScale)
            {
                if (orangeWinner)
                {
                    instance.orangeBallRT.sizeDelta = Vector2.LerpUnclamped(instance.orangeBallRT.sizeDelta, Vector2.one * instance.ballSmallSize, instance.scaleCurve.Evaluate(c / instance.timeToScale));
                }
                else
                {
                    instance.blueBallRT.sizeDelta = Vector2.LerpUnclamped(instance.blueBallRT.sizeDelta, Vector2.one * instance.ballSmallSize, instance.scaleCurve.Evaluate(c / instance.timeToScale));
                }
                c += Time.unscaledDeltaTime;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(instance.timeBetween);
            c = 0f;
            while (c < instance.timeToMove)
            {
                if (orangeWinner)
                {
                    instance.orangeBall.position = Vector3.LerpUnclamped(instance.orangeBall.position, UIHandler.instance.roundCounterSmall.GetPointPos(0), instance.scaleCurve.Evaluate(c / instance.timeToMove));
                }
                else
                {
                    instance.blueBall.position = Vector3.LerpUnclamped(instance.blueBall.position, UIHandler.instance.roundCounterSmall.GetPointPos(1), instance.scaleCurve.Evaluate(c / instance.timeToMove));
                }
                c += Time.unscaledDeltaTime;
                yield return null;
            }
            SoundManager.Instance.Play(instance.sound_UI_Arms_Race_B_Ball_Go_Down_Then_Expand, instance.transform);
            if (orangeWinner)
            {
                instance.orangeBall.position = UIHandler.instance.roundCounterSmall.GetPointPos(0);
            }
            else
            {
                instance.blueBall.position = UIHandler.instance.roundCounterSmall.GetPointPos(1);
            }
            yield return new WaitForSecondsRealtime(instance.timeBetween);
            c = 0f;
            while (c < instance.timeToMove)
            {
                if (!orangeWinner)
                {
                    instance.orangeBall.position = Vector3.LerpUnclamped(instance.orangeBall.position, CardChoiceVisuals.instance.transform.position, instance.scaleCurve.Evaluate(c / instance.timeToMove));
                }
                else
                {
                    instance.blueBall.position = Vector3.LerpUnclamped(instance.blueBall.position, CardChoiceVisuals.instance.transform.position, instance.scaleCurve.Evaluate(c / instance.timeToMove));
                }
                c += Time.unscaledDeltaTime;
                yield return null;
            }
            if (!orangeWinner)
            {
                instance.orangeBall.position = CardChoiceVisuals.instance.transform.position;
            }
            else
            {
                instance.blueBall.position = CardChoiceVisuals.instance.transform.position;
            }
            yield return new WaitForSecondsRealtime(instance.timeBetween);
            c = 0f;
            while (c < instance.timeToScale)
            {
                if (!orangeWinner)
                {
                    instance.orangeBallRT.sizeDelta = Vector2.LerpUnclamped(instance.orangeBallRT.sizeDelta, Vector2.one * instance.bigBallScale, instance.scaleCurve.Evaluate(c / instance.timeToScale));
                }
                else
                {
                    instance.blueBallRT.sizeDelta = Vector2.LerpUnclamped(instance.blueBallRT.sizeDelta, Vector2.one * instance.bigBallScale, instance.scaleCurve.Evaluate(c / instance.timeToScale));
                }
                c += Time.unscaledDeltaTime;
                yield return null;
            }
            SoundManager.Instance.Play(instance.sound_UI_Arms_Race_C_Ball_Pop_Shake, instance.transform);
            GamefeelManager.instance.AddUIGameFeelOverTime(10f, 0.2f);
            CardChoiceVisuals.instance.Show((!orangeWinner) ? 0 : 1, false);
            UIHandler.instance.roundCounterSmall.UpdateRounds(orangeRounds, blueRounds);
            UIHandler.instance.roundCounterSmall.UpdatePoints(0, 0);
            instance.DoShowPoints(0, 0, orangeWinner);
            instance.Close();
            yield break;
        }


        [HarmonyPostfix]
        [HarmonyPatch("ResetPoints")]
        private static void Postfix1()
        {
            redFill.fillAmount = 0f;
            greenFill.fillAmount = 0f;
        }

        [HarmonyPostfix]
        [HarmonyPatch("ResetBalls")]
        private static void Postfix2()
        {
            redBallRT.sizeDelta = Vector2.one * ballBaseSize;
            greenBallRT.sizeDelta = Vector2.one * ballBaseSize;
            redBall.GetComponent<RectTransform>().anchoredPosition = redSP;
            greenBall.GetComponent<RectTransform>().anchoredPosition = greenSP;
            redVel = Vector3.zero;
            greenVel = Vector3.zero;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch("Close")]
        private static void Postfix4()
        {
            greenBall.gameObject.SetActive(false);
            redBall.gameObject.SetActive(false);
        }
        */

        [HarmonyPostfix]
        [HarmonyPatch("DoShowPoints")]
        private static void Postfix3()
        {
            var instance = PointVisualizer.instance;
            // redBall = Object.Instantiate(instance.orangeBall);
            // greenBall = Object.Instantiate(instance.blueBall);

            if (GM_ArmsRacePatch.winningTeamID == 2)
            {
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(2).winText;
            }
            else if (GM_ArmsRacePatch.winningTeamID == 3)
            {
                instance.text.color = PlayerSkinBank.GetPlayerSkinColors(3).winText;
            }
            if (GM_ArmsRacePatch.winningTeamID == 2)
            {
                if (GM_ArmsRacePatch.p3Points == 1)
                {
                    HalfRed();
                    return;
                }
                RoundRed();
                return;
            }
            else if (GM_ArmsRacePatch.winningTeamID == 3)
            {
                if (GM_ArmsRacePatch.p4Points == 1)
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
