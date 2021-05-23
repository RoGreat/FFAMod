using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;

namespace FFAMod
{
    static class Main
    {
        public static UnityModManager.ModEntry mod;

        static void Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony("mod.rounds.all.for.free");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            mod = modEntry;
        }
    }
}
