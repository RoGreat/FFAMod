//using HarmonyLib;
//using UnityEngine;

//namespace FFAMod
//{
//    [HarmonyPatch(typeof(UIHandler))]
//    internal class UIHandlerPatch
//    {
//        public static RoundCounter roundCounterSmall2;
//        private static RoundCounter roundCounter2;

//        [HarmonyPostfix]
//        [HarmonyPatch("Awake")]
//        private static void Postfix1()
//        {
//            roundCounterSmall2 = Object.Instantiate(UIHandler.instance.roundCounterSmall, UIHandler.instance.transform.Find("Canvas"));
//            roundCounterSmall2.gameObject.name = "RoundCounterSmall2";
//            roundCounterSmall2.gameObject.transform.position = UIHandler.instance.roundCounterSmall.transform.position + Vector3.down * 2f;
//            UIHandler.instance.roundCounter.gameObject.transform.position = UIHandler.instance.roundCounter.transform.position + Vector3.left * 8f;
//            roundCounter2 = Object.Instantiate(UIHandler.instance.roundCounter, UIHandler.instance.transform.Find("Canvas"));
//            roundCounter2.gameObject.name = "RoundCounter2";
//            roundCounter2.gameObject.transform.position = roundCounter2.transform.position + Vector3.right * 8f;
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch("ShowRoundOver")]
//        private static void Postfix2()
//        {
//            var instance = UIHandler.instance;
//            roundCounter2.gameObject.SetActive(true);
//            instance.roundBackgroundPart.Play();
//            instance.roundTextPart.Play();
//            instance.roundCounterAnim.PlayIn();
//            roundCounter2.UpdateRounds(GM_ArmsRacePatch.p3Rounds, GM_ArmsRacePatch.p4Rounds);
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch("SetNumberOfRounds")]
//        private static void Postfix3(int roundsToWinGame)
//        {
//            var setNumberOfRounds = AccessTools.Method(typeof(RoundCounter), "SetNumberOfRounds");
//            setNumberOfRounds.Invoke(roundCounter2, new object[] { roundsToWinGame });
//            setNumberOfRounds.Invoke(roundCounterSmall2, new object[] { roundsToWinGame });
//        }

//        [HarmonyPatch("ShowRoundCounterSmall")]
//        private static void Prefix()
//        {
//            roundCounterSmall2.gameObject.SetActive(true);
//            roundCounterSmall2.UpdateRounds(GM_ArmsRacePatch.p3Rounds, GM_ArmsRacePatch.p4Rounds);
//            roundCounterSmall2.UpdatePoints(GM_ArmsRacePatch.p3Points, GM_ArmsRacePatch.p4Points);
//        }
//    }
//}