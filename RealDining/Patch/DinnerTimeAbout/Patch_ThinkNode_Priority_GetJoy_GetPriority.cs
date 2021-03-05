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
    [HarmonyPatch(typeof(ThinkNode_Priority_GetJoy))]
    [HarmonyPatch("GetPriority")]
    [HarmonyPatch(new Type[] { typeof(Pawn)})]
    class Patch_ThinkNode_Priority_GetJoy_GetPriority
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result,Pawn pawn)
        {
            if (pawn.needs.joy == null)
            {
                __result=0f;
                return false;
            }
            if (Find.TickManager.TicksGame < 5000)
            {
                __result=0f;
                return false;
            }
            TimeAssignmentDef timeAssignmentDef = (pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment;
            if (timeAssignmentDef != TimeAssignmentDefDinner.DinnerDef) return true;

            if (JoyUtility.LordPreventsGettingJoy(pawn))
            {
                __result= 0f;
                return false;
            }
            /*float curLevel = pawn.needs.joy.CurLevel;
            if (curLevel < 0.95f)
            {
                __result= 7f;
            }
            else __result= 0f;*/
            __result = 7f;//dinner时间开心值满了也一直getjoy
            //Log.Message("{0} joy priority = {1}".Translate(pawn.Label,__result));
            //Log.Message("{0}'s dinner time".Translate(pawn.Label));
            return false;
        }
    }
}
