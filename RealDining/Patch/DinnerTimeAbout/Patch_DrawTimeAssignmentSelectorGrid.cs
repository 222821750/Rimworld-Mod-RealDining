using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using Verse.AI;
using UnityEngine;
using Verse.Sound;
using YC.RealDining.Resource.DefClass;
using YC.RealDining.Resource;

namespace YC.RealDining.Patch.DinnerTimeAbout
{
    [HarmonyPatch(typeof(TimeAssignmentSelector))]
    [HarmonyPatch("DrawTimeAssignmentSelectorGrid")]
    [HarmonyPatch(new Type[] {typeof(Rect) })]
    class Patch_DrawTimeAssignmentSelectorGrid
    {
        [HarmonyPrefix]
        static bool Prefix(Rect rect)
        {
            //Log.Message("dinnerdef label={0} color={1}".Translate(TimeAssignmentDefDinner.DinnerDef.label, TimeAssignmentDefDinner.DinnerDef.color.ToString()));
            rect.yMax -= 2f;
            Rect rect2 = rect;
            rect2.xMax = rect2.center.x;
            rect2.yMax = rect2.center.y;
            //DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Anything);
            if (ModSetting.dinnerTimeMode == 0)
            {
                rect2.x += rect2.width * 3;
            }
            else if (ModSetting.dinnerTimeMode == 1)
            {
                rect2.x += rect2.width * 4;
            }
            else rect2.x += rect2.width * 6;
            //DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Work);
            DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefDinner.DinnerDef);
            //rect2.y += rect2.height;
            //rect2.x -= rect2.width;
            //rect2.x -= rect2.width;
            //DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Joy);
            //rect2.x += rect2.width;
            //DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Sleep);
            return true;
        }

        private static void DrawTimeAssignmentSelectorFor(Rect rect, TimeAssignmentDef ta)
        {
            rect = rect.ContractedBy(2f);
            GUI.DrawTexture(rect, ta.ColorTexture);
            if (Widgets.ButtonInvisible(rect, true))
            {
                TimeAssignmentSelector.selectedAssignment = ta;
                SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
            }
            GUI.color = Color.white;
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = Color.white;
            Widgets.Label(rect, ta.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
            if (TimeAssignmentSelector.selectedAssignment == ta)
            {
                Widgets.DrawBox(rect, 2);
            }
        }
    }
}
