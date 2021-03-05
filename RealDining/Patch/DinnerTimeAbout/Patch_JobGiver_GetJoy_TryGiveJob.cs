using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using YC.RealDining.Resource.DefClass;
using HarmonyLib;
using UnityEngine;

namespace YC.RealDining.Patch.DinnerTimeAbout
{
    [HarmonyPatch(typeof(JobGiver_GetJoy))]
    [HarmonyPatch("TryGiveJob")]
    [HarmonyPatch(new Type[] { typeof(Pawn) })]
    class Patch_JobGiver_GetJoy_TryGiveJob
    {
        private static DefMap<JoyGiverDef, float> joyGiverChances=new DefMap<JoyGiverDef, float>();
        [HarmonyPrefix]
        static bool Prefix(JobGiver_GetJoy __instance,ref Job __result,Pawn pawn)
        {
            TimeAssignmentDef timeAssignmentDef = ((pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment);
            if (timeAssignmentDef != TimeAssignmentDefDinner.DinnerDef) return true;

            if (pawn.InBed() && HealthAIUtility.ShouldSeekMedicalRest(pawn))
            {
                __result= null;
                return false;
            }
            List<JoyGiverDef> allDefsListForReading = DefDatabase<JoyGiverDef>.AllDefsListForReading;
            JoyToleranceSet tolerances = pawn.needs.joy.tolerances;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                JoyGiverDef joyGiverDef = allDefsListForReading[i];
                joyGiverChances[joyGiverDef] = 0f;
                if (joyGiverDef.joyKind != JoyKindDefOf.Gluttonous && joyGiverDef.defName!= "SocialRelax") continue;//选择暴食和社交休闲娱乐
                //Log.Message("{0} getchance baoshi".Translate(pawn.Label));
                if (!pawn.needs.joy.tolerances.BoredOf(joyGiverDef.joyKind) && joyGiverDef.Worker.CanBeGivenTo(pawn))
                {
                    if (joyGiverDef.pctPawnsEverDo < 1f)
                    {
                        Rand.PushState(pawn.thingIDNumber ^ 63216713);
                        if (Rand.Value >= joyGiverDef.pctPawnsEverDo)
                        {
                            Rand.PopState();
                            goto IL_11A;
                        }
                        Rand.PopState();
                    }
                    float num = tolerances[joyGiverDef.joyKind];
                    float num2 = Mathf.Pow(1f - num, 5f);
                    num2 = Mathf.Max(0.001f, num2);
                    joyGiverChances[joyGiverDef] = joyGiverDef.Worker.GetChance(pawn) * num2;
                }
                IL_11A:;
            }
            int num3 = 0;
            JoyGiverDef def;
            while (num3 < joyGiverChances.Count && allDefsListForReading.TryRandomElementByWeight((JoyGiverDef d) => joyGiverChances[d], out def))
            {
                //if(pawn.needs.joy.CurLevel>0.3f && def.joyKind == JoyKindDefOf.Gluttonous && pawn.timetable.GetAssignment((GenLocalDate.HourOfDay(pawn) + 1) % 24) == TimeAssignmentDefDinner.DinnerDef)
                if(pawn.needs.joy.CurLevel < 0.95f || def.joyKind != JoyKindDefOf.Gluttonous)
                {
                    Job job = TryGiveJobFromJoyGiverDefDirect(def, pawn);
                    if (job != null)
                    {
                        __result = job;
                        return false;
                    }
                }
                //Log.Message("{0} trygivejoy".Translate(pawn.Label));
                joyGiverChances[def] = 0f;
                num3++;
            }
            __result = null;
            return false;
        }

        protected static Job TryGiveJobFromJoyGiverDefDirect(JoyGiverDef def, Pawn pawn)
        {
            //Log.Message("def {0} trygivejob".Translate(def.defName));
            return def.Worker.TryGiveJob(pawn);
        }
    }
}