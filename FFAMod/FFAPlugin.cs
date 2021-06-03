using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace FFAMod
{
    [BepInPlugin("plugin.bepinex.mod.rounds.all.for.free", "Free For All Mod", "0.7.0.0")]
    [BepInProcess("Rounds.exe")]
    public class FFAPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            var harmony = new Harmony("mod.rounds.all.for.free");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo("Free For All Mod Loaded");
        }
    }
}