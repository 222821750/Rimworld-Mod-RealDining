using System;
using RimWorld;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using YC.RealDining.Resource;

namespace YC.RealDining.Patch.FoodAbout
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch("FoodOptimality")]
    [HarmonyPatch(new Type[] { typeof(Pawn),typeof(Thing),typeof(ThingDef),typeof(float),typeof(bool)})]
    class Patch_FoodOptimality
    {
        private static readonly SimpleCurve FoodOptimalityEffectFromMoodCurve = new SimpleCurve//只读常量的曲线函数
        {
            {
                new CurvePoint(-100f, -600f),
                true
            },
            {
                new CurvePoint(-10f, -100f),
                true
            },
            {
                new CurvePoint(-5f, -70f),
                true
            },
            {
                new CurvePoint(-1f, -50f),
                true
            },
            {
                new CurvePoint(0f, 0f),
                true
            },
            {
                new CurvePoint(100f, 800f),
                true
            }
        };
        [HarmonyPrefix]
        //原函数返回食物的优先选择程度值
        static bool Prefix(ref float __result, Pawn eater, Thing foodSource, ThingDef foodDef, float dist, bool takingToInventory = false)
        {
            //Log.Message("{0} start analysis food {1}".Translate(eater.Label, foodDef.label));
            float num = 300f;//基础数值
            num -= dist;//距离
            switch (foodDef.ingestible.preferability)//垃圾食物
            {
                case FoodPreferability.NeverForNutrition:
                    __result= -9999999f;
                    return false;
                case FoodPreferability.DesperateOnly:
                    num -= 150f;
                    break;
                case FoodPreferability.DesperateOnlyForHumanlikes:
                    if (eater.RaceProps.Humanlike)
                    {
                        num -= 150f;
                    }
                    break;
            }
            CompRottable compRottable = foodSource.TryGetComp<CompRottable>();//变质组件
            if (compRottable != null)
            {
                if (compRottable.Stage == RotStage.Dessicated)//已经变质风干的食物
                {
                    __result= -9999999f;
                    return false;
                }
                //会快要变质的新鲜食物
                if (!takingToInventory && compRottable.Stage == RotStage.Fresh && compRottable.TicksUntilRotAtCurrentTemp < 30000)
                {
                    num += 13.3f;
                }
            }
            bool badFood = false;
            if (eater.needs != null && eater.needs.mood != null)
            {
                float theXi = ModSetting.moodInfluenceX;//心情影响附加系数
                //心情接近轻微崩溃阈值时增大心情影响附加系数
                //心情在严重崩溃阈值以下
                if (eater.needs.mood.CurLevel < eater.mindState.mentalBreaker.BreakThresholdExtreme) theXi += 0.77f;
                //心情在中度崩溃阈值以下
                else if (eater.needs.mood.CurLevel < eater.mindState.mentalBreaker.BreakThresholdMajor) theXi += 0.60f;
                //心情在轻微崩溃阈值以下
                else if (eater.needs.mood.CurLevel < eater.mindState.mentalBreaker.BreakThresholdMinor) theXi += 0.43f;
                //心情接近轻微崩溃阈值
                else if (eater.needs.mood.CurLevel < eater.mindState.mentalBreaker.BreakThresholdMinor + 0.06f) theXi += 0.25f;
                //Log.Message("mood {0} theXi {1}".Translate(eater.needs.mood.CurLevel, theXi));
                List<ThoughtDef> list = FoodUtility.ThoughtsFromIngesting(eater, foodSource, foodDef);//eater对食物的看法
                for (int i = 0; i < list.Count; i++)
                {
                    //计算出心情影响曲线函数值后乘以心情影响附加系数
                    num += FoodOptimalityEffectFromMoodCurve.Evaluate(list[i].stages[0].baseMoodEffect)*theXi;
                    if (list[i].stages[0].baseMoodEffect < 0) badFood = true;//掉心情的为“坏食物”
                }
            }
            if (foodDef.ingestible != null)
            {
                if (eater.RaceProps.Humanlike)
                {
                    num += foodDef.ingestible.optimalityOffsetHumanlikes;//食物类型人类接受程度
                }
                else if (eater.RaceProps.Animal)
                {
                    num += foodDef.ingestible.optimalityOffsetFeedingAnimals;
                }
            }
            if (!badFood && compRottable!=null && eater.RaceProps.Humanlike)//不是“坏食物”并且会变质的，只对人类有效的附加随机值
            {
                float randomt;
                if (!ModData.foodClassRandomVal.ContainsKey(foodDef.defName))
                {
                    randomt = Rand.Range(0f, ModSetting.randomLevel);
                    ModData.foodClassRandomVal[foodDef.defName] = randomt;
                }
                else randomt = ModData.foodClassRandomVal[foodDef.defName];

                float downXi = 1f;
                if (ModSetting.priorityRoomFood && ModData.findedInventoryFoodID == foodSource.GetUniqueLoadID())
                {
                    ModData.findedInventoryFoodID = null;
                    downXi = 0.2f;
                }
                //Log.Message("downXi = " + downXi);
                //通过增加一个随机值来提高食物的优选程度
                if (ModData.GetLastFoodType(eater) == foodDef.defName) num += randomt * ModSetting.lastFoodInfluenceX * downXi;
                else if (ModData.GetLlastFoodType(eater) == foodDef.defName) num += randomt * ModSetting.llastFoodInfluenceX * downXi;
                else num += randomt * downXi;

            }
            //Log.Warning("warn");
            //Log.Message("food {0} val = {1}".Translate(foodSource.GetUniqueLoadID(), num));
            __result= num;//函数返回值：优选程度
            return false;//跳过原函数
        }
    }
}
