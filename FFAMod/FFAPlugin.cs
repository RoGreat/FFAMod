using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace FFAMod
{
    [BepInPlugin("plugin.bepinex.rounds.all.for.free", "Free For All Mod", "0.5.0.0")]
    [BepInProcess("Rounds.exe")]
    public class FFAPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            Logger.LogInfo("Free For All Mod Loaded");
            var harmony = new Harmony("mod.rounds.all.for.free");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}