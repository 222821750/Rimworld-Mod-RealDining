using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using YC.RealDining.Resource;
using Verse.AI;
using UnityEngine;

namespace YC.RealDining.Patch.FoodAbout
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch("BestFoodInInventory")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Pawn),typeof(FoodPreferability),
        typeof(FoodPreferability),typeof(float),typeof(bool) })]
    class Patch_BestFoodInInventory
    {
        [HarmonyPostfix]
        static void Postfix(ref Thing __result, Pawn holder, Pawn eater = null, FoodPreferability minFoodPref = FoodPreferability.NeverForNutrition,
            FoodPreferability maxFoodPref = FoodPreferability.MealLavish, float minStackNutrition = 0f, bool allowDrug = false)
        {
            if (!ModSetting.priorityRoomFood) return;
            ModData.findedInventoryFoodID = null;

            if (holder == null || holder.inventory == null)
            {
                return;
            }
            if (eater == null || eater.GetUniqueLoadID() == holder.GetUniqueLoadID())//操作者和吃饭者得为一个生物
            {
                if (!holder.IsColonist) return;
                if (holder.RaceProps.Humanlike && holder.Spawned && holder.needs.food.CurLevelPercentage > holder.RaceProps.FoodLevelPercentageWantEat * 0.45f
                    && holder.Map.areaManager.Home[holder.Position])
                {//为人类 存在 在居住区 食物需要百分比大于想吃百分比*0.45
                    //Log.Message("down inventory food");
                    ModData.findedInventoryFoodID = __result.GetUniqueLoadID();
                    return;//记录找到的背包食物
                }
            }
            //Log.Message("normal inventory food");
        }
    }
}
