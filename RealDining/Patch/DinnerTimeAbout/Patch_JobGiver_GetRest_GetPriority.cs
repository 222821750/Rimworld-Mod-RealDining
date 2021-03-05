using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using Verse.AI.Group;
using RimWorld;
using YC.RealDining.Resource.DefClass;

namespace YC.RealDining.Patch.DinnerTimeAbout
{
    [HarmonyPatch(typeof(JobGiver_GetRest))]
    [HarmonyPatch("GetPriority")]
    [HarmonyPatch(new Type[] { typeof(Pawn) })]
    class Patch_JobGiver_GetRest_GetPriority
    {
        [HarmonyPrefix]
        static bool Prefix(RestCategory ___minCategory, float ___maxLevelPercentage, ref float __result,Pawn pawn)
        {
            Need_Rest rest = pawn.needs.rest;
            if (rest == null)
            {
                __result= 0f;return false;
            }
            if (rest.CurCategory < ___minCategory)
            {
                __result= 0f; return false;
            }
            if (rest.CurLevelPercentage > ___maxLevelPercentage)
            {
                __result= 0f;return false;
            }
            if (Find.TickManager.TicksGame < pawn.mindState.canSleepTick)
            {
                __result= 0f;return false;
            }
            TimeAssignmentDef timeAssignmentDef;
            if (pawn.RaceProps.Humanlike)
            {
                timeAssignmentDef = ((pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment);
                if (timeAssignmentDef != TimeAssignmentDefDinner.DinnerDef) return true;

                Lord lord = pawn.GetLord();
                if (lord != null && !lord.CurLordToil.AllowSatisfyLongNeeds)
                {
                    __result= 0f;return false;
                }
                float curLevel = rest.CurLevel;
                if (curLevel < 0.3f)
                {
                    __result= 8f;
                }
                else __result= 0f;
                //Log.Message("{0} rest priority = {1}".Translate(pawn.Label, __result));

                return false;
            }
            else return true;
        }
    }
}
