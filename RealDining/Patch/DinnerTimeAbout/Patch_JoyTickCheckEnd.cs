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
using YC.RealDining.Resource;

namespace YC.RealDining.Patch.DinnerTimeAbout
{
    [HarmonyPatch(typeof(JoyUtility))]
    [HarmonyPatch("JoyTickCheckEnd")]
    [HarmonyPatch(new Type[] { typeof(Pawn),typeof(JoyTickFullJoyAction),typeof(float),typeof(Building) })]
    class Patch_JoyTickCheckEnd
    {
        [HarmonyPrefix]
        static bool Prefix(Pawn pawn, JoyTickFullJoyAction fullJoyAction = JoyTickFullJoyAction.EndJob, float extraJoyGainFactor = 1f, Building joySource = null)
        {
            Job curJob = pawn.CurJob;
            if (curJob.def.joyKind == null)
            {
                Log.Warning("This method can only be called for jobs with joyKind.", false);
                return false; ;
            }
            if (joySource != null)
            {
                if (joySource.def.building.joyKind != null && pawn.CurJob.def.joyKind != joySource.def.building.joyKind)
                {
                    Log.ErrorOnce("Joy source joyKind and jobDef.joyKind are not the same. building=" + joySource.ToStringSafe<Building>() + ", jobDef=" + pawn.CurJob.def.ToStringSafe<JobDef>(), joySource.thingIDNumber ^ 876598732, false);
                }
                extraJoyGainFactor *= joySource.GetStatValue(StatDefOf.JoyGainFactor, true);
            }
            if (pawn.needs.joy == null)
            {
                pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);
                return false;
            }

            TimeAssignmentDef timeAssignmentDef = (pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment;
            if(timeAssignmentDef!=TimeAssignmentDefDinner.DinnerDef) return true;

            pawn.needs.joy.GainJoy(extraJoyGainFactor * curJob.def.joyGainRate * 0.36f / 2500f, curJob.def.joyKind);
            if (curJob.def.joySkill != null)
            {
                pawn.skills.GetSkill(curJob.def.joySkill).Learn(curJob.def.joyXpPerTick, false);
            }
            if (pawn.needs.food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat + ModSetting.eatThreshold && pawn.IsHashIntervalTick(60))//dinner饱腹值以下时结束娱乐
            {
                bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;
                Thing thing;
                ThingDef thingDef;
                if (FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out thing, out thingDef, true, true, false, true, false, pawn.IsWildMan(), false, false, FoodPreferability.Undefined))
                {
                    //Log.Message("joytickcheckend find {0}".Translate(thing.GetUniqueLoadID()));
                    if (fullJoyAction == JoyTickFullJoyAction.EndJob)
                    {
                        pawn.jobs.curDriver.EndJobWith(JobCondition.Succeeded);
                        return false;
                    }
                    if (fullJoyAction == JoyTickFullJoyAction.GoToNextToil)
                    {
                        pawn.jobs.curDriver.ReadyForNextToil();
                    }
                } 
            }
            return false;
        }
    }
}
