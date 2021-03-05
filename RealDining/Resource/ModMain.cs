using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace YC.RealDining.Resource
{
    public class ModMain:Mod
    {
        //public ModSetting modSetting;
        public ModMain(ModContentPack content):base(content)
        {
            GetSettings<ModSetting>();
        }

        public override string SettingsCategory()
        {
            return "RealDining_Mod_Settings".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(0, 0, inRect.width*=0.94f, inRect.height);
            Listing_Standard ls = new Listing_Standard();
            ls.BeginScrollView(inRect, ref ModSetting.scrollPos, ref rect);

            if (ls.ButtonText("Restore_default".Translate())) { ModSetting.InitSetting(); }
            ls.GapLine(20f);

            if (ls.ButtonTextLabeled("Random_degree_of_food_selection".Translate(), ModSetting.ValLevelLabel(ModSetting.randomLevel).Translate()))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                for (int i = 1; i <= 5; i++)
                {
                    int tmp = i;
                    list.Add(new FloatMenuOption(ModSetting.levelLabel[i].Translate(), delegate ()
                    {
                        //Log.Message(i.ToString());
                        ModSetting.randomLevel = ModSetting.randomLevelVal[tmp];
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            ls.Label("Random_degree_explain".Translate());
            ls.GapLine(20f);

            if (ls.ButtonTextLabeled("The_importance_of_the_mood".Translate(), ModSetting.ValLevelLabel(ModSetting.moodInfluenceX).Translate()))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                for (int i = 1; i <= 5; i++)
                {
                    int tmp = i;
                    list.Add(new FloatMenuOption(ModSetting.levelLabel[i].Translate(), delegate ()
                    {
                        ModSetting.moodInfluenceX = ModSetting.moodLevelVal[tmp];
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            ls.Label("Mood_degree_explain".Translate());
            ls.GapLine(20f);

            ls.Label("Repeated_food_selection_value".Translate() + "   " + (ModSetting.lastFoodInfluenceX * 10f).ToString());
            ModSetting.lastFoodInfluenceX = (float)Math.Round(ls.Slider(ModSetting.lastFoodInfluenceX * 10f, 0f, 10f)) / 10f;
            ls.Label("Repeated_food_selection_value2".Translate() + "   " + (ModSetting.llastFoodInfluenceX * 10f).ToString());
            ModSetting.llastFoodInfluenceX = (float)Math.Round(ls.Slider(ModSetting.llastFoodInfluenceX * 10f, 0f, 10f)) / 10f;
            ls.Label("Repeated_value_explain".Translate());
            ls.Label("Repeated_value_explain2".Translate());
            ls.GapLine(20f);

            ls.Label("Eat_threshold".Translate() + "   " + ModSetting.eatThreshold.ToString());
            ModSetting.eatThreshold = (float)Math.Round(ls.Slider(ModSetting.eatThreshold, 0.00f, 0.70f),2);
            ls.Label("Eat_threshold_explain".Translate());
            ls.GapLine(20f);

            if (ls.ButtonTextLabeled("Priority_NotInventory_Food".Translate(), ModSetting.priorityRoomFood.ToStringYesNo()))
            {
                ModSetting.priorityRoomFood = !ModSetting.priorityRoomFood;
            }

            if (ls.ButtonTextLabeled("Dinner_Time_Mode".Translate(), ModSetting.GetDinnerTimeModeStr().Translate()))
            {
                ModSetting.dinnerTimeMode++;
                ModSetting.dinnerTimeMode %= 3;
            }

            ls.EndScrollView(ref rect);
        }
    }
}
