using Verse;
using HarmonyLib;
using System.Reflection;

namespace YC.RealDining.Patch
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            //Log.Message("StaticConstructorOnStartup begin!");
            var harmony = new Harmony("YC.RealDining");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
