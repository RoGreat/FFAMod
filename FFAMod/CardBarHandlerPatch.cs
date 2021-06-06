using HarmonyLib;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
namespace FFAMod
{
    [HarmonyPatch(typeof(CardBarHandler))]
    class CardBarHandlerPatch
    {
        [HarmonyPatch("Start")]
        private static void Prefix(CardBarHandler __instance, ref CardBar[] ___cardBars)
        {
            CardBarHandler.instance = __instance;
            ___cardBars = __instance.GetComponentsInChildren<CardBar>();
            var bar3 = Object.Instantiate(___cardBars[0], CardBarHandler.instance.transform);
            bar3.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ProceduralImage>().color=PlayerSkinBank.GetPlayerSkinColors(2).backgroundColor;
            bar3.name = "Bar3";
            bar3.transform.position = ___cardBars[0].transform.position + Vector3.down * 4f;
            bar3.GetComponentInParent<CardBar>();
            var bar4 = Object.Instantiate(___cardBars[1], CardBarHandler.instance.transform);
            bar4.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ProceduralImage>().color=PlayerSkinBank.GetPlayerSkinColors(3).backgroundColor;
            bar4.name = "Bar4";
            bar4.transform.position = ___cardBars[1].transform.position + Vector3.down * 4f;
            bar4.GetComponentInParent<CardBar>();
            ___cardBars.AddToArray(bar3);
            ___cardBars.AddToArray(bar4);
        }
    }
}
