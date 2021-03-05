using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using Verse.AI;
using RimWorld;
using YC.RealDining.Resource.DefClass;

namespace YC.RealDining.Patch.DinnerTimeAbout
{
    [HarmonyPatch(typeof(ThoughtWorker_NeedFood))]
    [HarmonyPatch("CurrentStateInternal")]
    [HarmonyPatch(new Type[] { typeof(Pawn) })]
    class Patch_ThoghWokr_NedFod_CurrentStateInternal
    {
        [HarmonyPrefix]
        static bool Prefix(ref ThoughtState __result, Pawn p)
        {
            if (p.needs.food == null)
            {
                __result= ThoughtState.Inactive;
                return false;
            }
            if(p.RaceProps.Humanlike && p.timetable != null)
            {
                TimeAssignmentDef timeAssignmentDef =p.timetable.CurrentAssignment;
                if (timeAssignmentDef != TimeAssignmentDefDinner.DinnerDef)
                {
                    Need_Food food = p.needs.food;
                    if(p.timetable.GetAssignment((GenLocalDate.HourOfDay(p) + 1) % 24) == TimeAssignmentDefDinner.DinnerDef 
                        && food.CurLevelPercentage > p.RaceProps.FoodLevelPercentageWantEat * 0.45f && food.CurCategory >= HungerCategory.Hungry)
                    {//下一小时是dinner时间并且饥饿度百分比>0.45并且处于饥饿及以上类型状态
                        __result = ThoughtState.ActiveAtStage(7);
                        return false;
                    }
                }
            }
            switch (p.needs.food.CurCategory)
            {
                case HungerCategory.Fed:
                    __result= ThoughtState.Inactive;
                    return false;
                case HungerCategory.Hungry:
                    __result= ThoughtState.ActiveAtStage(0);
                    return false;
                case HungerCategory.UrgentlyHungry:
                    __result= ThoughtState.ActiveAtStage(1);
                    return false;
                case HungerCategory.Starving:
                    {
                        Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
                        int num = (firstHediffOfDef == null) ? 0 : firstHediffOfDef.CurStageIndex;
                        if (num > 4) num=4;
                        __result= ThoughtState.ActiveAtStage(2 + num);
                        return false;
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
