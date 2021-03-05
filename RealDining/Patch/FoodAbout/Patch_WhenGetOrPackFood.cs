using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using YC.RealDining.Resource;
using HarmonyLib;
using Verse.AI;

namespace YC.RealDining.Patch.FoodAbout
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch("SpawnedFoodSearchInnerScan")]
    [HarmonyPatch(new Type[]{typeof(Pawn),typeof(IntVec3),typeof(List<Thing>),typeof(PathEndMode),
        typeof(TraverseParms),typeof(float),typeof(Predicate<Thing>)})]
    class Patch_SpawnedFoodSearchInnerScan
    {
        [HarmonyPrefix]
        static bool Prefix(ref Thing __result, Pawn eater, IntVec3 root, List<Thing> searchSet,
            PathEndMode peMode, TraverseParms traverseParams, float maxDistance = 9999f, Predicate<Thing> validator = null)
        {
            if (!eater.RaceProps.Humanlike) return true;
            //Log.Message("getfood foodclassrandomval clear");
            ModData.foodClassRandomVal.Clear();
            //for(int i = 0; i < 12; i++) Log.Message("time {0} Assignment {1}".Translate(i, eater.timetable.times[i].label));
            return true;
        }
    }

    [HarmonyPatch(typeof(JobGiver_PackFood))]
    [HarmonyPatch("TryGiveJob")]
    [HarmonyPatch(new Type[] { typeof(Pawn)})]
    class Patch_PackFood_TryGiveJob
    {
        [HarmonyPrefix]
        static bool Prefix(ref Job __result,Pawn pawn)
        {
            if (!pawn.RaceProps.Humanlike) return true;
            //Log.Message("{0} packfood foodclassrandomval clear".Translate(pawn.Label));
            ModData.foodClassRandomVal.Clear();
            return true;
        }
    }

}
