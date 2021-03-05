using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using YC.RealDining.Resource.DefClass;
using YC.RealDining.Resource;
using HarmonyLib;

namespace YC.RealDining.Patch.DinnerTimeAbout
{
    [HarmonyPatch(typeof(JobGiver_GetFood))]
    [HarmonyPatch("GetPriority")]
    [HarmonyPatch(new Type[] { typeof(Pawn)})]
    class Patch_JobGiver_GetFood_GetPriority
    {
        [HarmonyPrefix]
        static bool Prefix(HungerCategory ___minCategory, float ___maxLevelPercentage, ref float __result,Pawn pawn)
        {
            //Log.Message("1");
            Need_Food food = pawn.needs.food;
            if (food == null)
            {
                __result = 0f;return false;
            }
            //Log.Message("2");
            if (pawn.needs.food.CurCategory < HungerCategory.Starving && FoodUtility.ShouldBeFedBySomeone(pawn))
            {
                __result = 0f;return false;
            }
            //Log.Message("3");
            if (food.CurCategory < ___minCategory)
            {
                __result = 0f;return false;
            }
            //Log.Message("4");
            if (food.CurLevelPercentage > ___maxLevelPercentage)
            {
                __result = 0f;return false;
            }
            //Log.Message("5");
            if (pawn.RaceProps.Humanlike)
            {
                if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat + ModSetting.eatThreshold)
                {
                    if(pawn.timetable == null)
                    {
                        if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat) __result = 9.5f;
                        else __result = 0f;
                        return false;
                    }
                    //Log.Message("6");
                    TimeAssignmentDef timeAssignmentDef = (pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment;
                    if (timeAssignmentDef != TimeAssignmentDefDinner.DinnerDef)
                    {
                        //Log.Message("7");
                        if (pawn.timetable.GetAssignment((GenLocalDate.HourOfDay(pawn) + 1) % 24) == TimeAssignmentDefDinner.DinnerDef && food.CurLevelPercentage > pawn.RaceProps.FoodLevelPercentageWantEat * 0.48f)
                        {//下一小时是dinner时间并且饥饿度百分比>0.45就不吃饭
                            __result = 0f;
                            return false;
                        }
                        if (pawn.timetable.GetAssignment((GenLocalDate.HourOfDay(pawn) + 2) % 24) == TimeAssignmentDefDinner.DinnerDef && food.CurLevelPercentage > pawn.RaceProps.FoodLevelPercentageWantEat * 0.8f)
                        {//下2小时是dinner时间并且饥饿度百分比>0.8就不吃饭
                            __result = 0f;
                            return false;
                        }
                        //Log.Message("8");
                        if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat) __result = 9.5f;
                        else __result = 0f;
                        return false;
                    }
                    if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat) __result = 9.5f;
                    else
                    {
                        __result = 7.25f;
                    }
                    return false;
                }
                else __result = 0;
                //Log.Message("{0} food priority = {1}".Translate(pawn.Label, __result));
            }
            else if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
            {
                __result = 9.5f;
            }
            else __result = 0f;
            return false;
        }
    }
}
