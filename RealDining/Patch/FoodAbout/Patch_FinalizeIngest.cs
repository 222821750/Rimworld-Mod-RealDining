using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using YC.RealDining.Resource;
using Verse.AI;
using UnityEngine;

namespace YC.RealDining.Patch.FoodAbout
{
    [HarmonyPatch(typeof(Toils_Ingest))]
    [HarmonyPatch("FinalizeIngest")]
    [HarmonyPatch(new Type[] { typeof(Pawn),typeof(TargetIndex)})]
    class Patch_FinalizeIngest
    {
        [HarmonyPrefix]
        static bool Prefix(ref Toil __result, Pawn ingester, TargetIndex ingestibleInd)
        {
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                Thing thing = curJob.GetTarget(ingestibleInd).Thing;
                if (ingester.needs.mood != null && thing.def.IsNutritionGivingIngestible && thing.def.ingestible.chairSearchRadius > 10f)
                {
                    if (!(ingester.Position + ingester.Rotation.FacingCell).HasEatSurface(actor.Map) && ingester.GetPosture() == PawnPosture.Standing && !ingester.IsWildMan())
                    {
                        ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.AteWithoutTable, null);
                    }
                    Room room = ingester.GetRoom(RegionType.Set_Passable);
                    if (room != null)
                    {
                        int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
                        if (ThoughtDefOf.AteInImpressiveDiningRoom.stages[scoreStageIndex] != null)
                        {
                            ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(ThoughtDefOf.AteInImpressiveDiningRoom, scoreStageIndex), null);
                        }
                    }
                }
                float num = ingester.needs.food.NutritionWanted;
                if (curJob.overeat)
                {
                    num = Mathf.Max(num, 0.75f);
                }
                float num2 = thing.Ingested(ingester, num);
                if (!ingester.Dead)
                {
                    ingester.needs.food.CurLevel += num2;
                }
                ingester.records.AddTo(RecordDefOf.NutritionEaten, num2);
                if(thing.def.IsNutritionGivingIngestible && ingester.RaceProps.Humanlike)//是饭并且是人
                {
                    MoodAdd(ingester, thing);//判断并添加相应心情
                    RecordLastFood(ingester, thing);//记录最后一次吃的食物
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            __result = toil;
            return false;
        }

        static public void RecordLastFood(Pawn pawn,Thing thing)
        {
            //Log.Message("{0} newfood = {1} lastfood = {2} llastfood = {3}".Translate(pawn.Label, thing.def.defName,
            //    ModData.GetLastFoodType(pawn),ModData.GetLlastFoodType(pawn)));
            string lastFood = ModData.GetLastFoodType(pawn);
            if (lastFood != HadAteFoodType.StrEmpty) ModData.SetLlastFoodType(pawn, lastFood);
            ModData.SetLastFoodType(pawn, thing.def.defName);
        }
        static public void MoodAdd(Pawn pawn,Thing thing)
        {
            if (IsBadFood(pawn, thing)) return;//这一次吃的是坏食物直接无加成
            string lastFood = ModData.GetLastFoodType(pawn);
            if (lastFood != HadAteFoodType.StrEmpty && lastFood != thing.def.defName)//与上次吃的不同（上次可以为坏食物）最起码可以有一点加成
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("EatFirstDifferentFood"));
                string llastFood = ModData.GetLlastFoodType(pawn);
                //上上次和上次和这次都不同
                if(llastFood != HadAteFoodType.StrEmpty && llastFood!=thing.def.defName && llastFood !=lastFood )
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("EatSecondDifferentFood"));
                }
                //Log.Message("give mood");
            }
        }
        static public bool IsBadFood(Pawn pawn, Thing thing)
        {
            List<ThoughtDef> list = FoodUtility.ThoughtsFromIngesting(pawn, thing, thing.def);//eater对食物的看法
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].stages[0].baseMoodEffect < 0) return true;//掉心情的为“坏食物”
            }
            return false;
        }
    }
}
