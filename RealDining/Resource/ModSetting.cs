using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace YC.RealDining.Resource
{
    public class ModSetting:ModSettings
    {
        public const float moodInfluenceXDefault = 0.88f;
        public const float randomLevelDefault = 103f;
        public const float lastFoodInfluenceXDefault = 0.4f;
        public const float llastFoodInfluenceXDefault = 0.6f;
        public const float eatThresholdDefault = 0.25f;
        public const int dinnerTimeModeDefault = 0;//默认在左边

        public static string[] levelLabel = new string[] { "", "Lowest", "Low", "Medium", "High", "Highest" };
        public static float[] moodLevelVal = new float[]
            {0f, moodInfLevelLowest, moodInfLevelLow, moodInfLevelMid, moodInfLevelHigh, moodInfLevelHighest };
        public static float[] randomLevelVal = new float[]
            {0f, randomLevelLowest, randomLevelLow, randomLevelMid, randomLevelHigh, randomLevelHighest};

        public const float randomLevelHighest = 300f;
        public const float randomLevelHigh = 200f;
        public const float randomLevelMid = randomLevelDefault;
        public const float randomLevelLow = 50f;
        public const float randomLevelLowest = 0f;

        public const float moodInfLevelHighest = 1.5f;
        public const float moodInfLevelHigh = 1.25f;
        public const float moodInfLevelMid = 1.03f;
        public const float moodInfLevelLow = moodInfluenceXDefault;
        public const float moodInfLevelLowest = 0.68f;

        public static float moodInfluenceX = moodInfluenceXDefault;
        public static float randomLevel = randomLevelDefault;
        public static float lastFoodInfluenceX = lastFoodInfluenceXDefault;
        public static float llastFoodInfluenceX = llastFoodInfluenceXDefault;
        public static float eatThreshold = eatThresholdDefault;
        public static UnityEngine.Vector2 scrollPos = UnityEngine.Vector2.zero;
        public static bool priorityRoomFood;//优先非背包内的食物
        public static int dinnerTimeMode = dinnerTimeModeDefault;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref moodInfluenceX, "moodInfluenceX", moodInfluenceXDefault);
            Scribe_Values.Look(ref randomLevel, "randomLevel", randomLevelDefault);
            Scribe_Values.Look(ref lastFoodInfluenceX, "lastFoodInfluenceX", lastFoodInfluenceXDefault);
            Scribe_Values.Look(ref llastFoodInfluenceX, "llastFoodInfluenceX", llastFoodInfluenceXDefault);
            Scribe_Values.Look(ref eatThreshold, "eatThreshold", eatThresholdDefault);
            Scribe_Values.Look(ref priorityRoomFood, "priorityRoomFood", false);
            Scribe_Values.Look(ref dinnerTimeMode, "dinnerTimeMode", dinnerTimeModeDefault);
            //Log.Message("setting");
        }

        public static void InitSetting()
        {
            moodInfluenceX = moodInfluenceXDefault;
            randomLevel = randomLevelDefault;
            lastFoodInfluenceX = lastFoodInfluenceXDefault;
            llastFoodInfluenceX = llastFoodInfluenceXDefault;
            eatThreshold = eatThresholdDefault;
            priorityRoomFood = false;
            dinnerTimeMode = dinnerTimeModeDefault;
        }

        public static string ValLevelLabel(float val)
        {
            if (val == moodInfluenceX)
            {
                switch (val)
                {
                    case moodInfLevelLowest:
                        return "Lowest";
                    case moodInfLevelLow:
                        return "Low";
                    case moodInfLevelMid:
                        return "Medium";
                    case moodInfLevelHigh:
                        return "High";
                    case moodInfLevelHighest:
                        return "Highest";
                }
            }
            else if (val == randomLevel)
            {
                switch (val)
                {
                    case randomLevelLowest:
                        return "Lowest";
                    case randomLevelLow:
                        return "Low";
                    case randomLevelMid:
                        return "Medium";
                    case randomLevelHigh:
                        return "High";
                    case randomLevelHighest:
                        return "Highest";
                }
            }
            return "ValLevelLabel Error";
        }

        public static float ValLevelNum(float val)
        {
            if (val == moodInfluenceX)
            {
                switch (val)
                {
                    case moodInfLevelLowest:
                        return 1f;
                    case moodInfLevelLow:
                        return 2f;
                    case moodInfLevelMid:
                        return 3f;
                    case moodInfLevelHigh:
                        return 4f;
                    case moodInfLevelHighest:
                        return 5f;
                }
            }
            else if (val == randomLevel)
            {
                switch (val)
                {
                    case randomLevelLowest:
                        return 1f;
                    case randomLevelLow:
                        return 2f;
                    case randomLevelMid:
                        return 3f;
                    case randomLevelHigh:
                        return 4f;
                    case randomLevelHighest:
                        return 5f;
                }
            }
            return -1f;
        }

        public static float ValNumLevel(float val,float num)
        {
            if (val == moodInfluenceX)
            {
                switch (num)
                {
                    case 1f:
                        return moodInfLevelLowest;
                    case 2f:
                        return moodInfLevelLow;
                    case 3f:
                        return moodInfLevelMid;
                    case 4f:
                        return moodInfLevelHigh;
                    case 5f:
                        return moodInfLevelHighest;
                }
            }
            else if (val == randomLevel)
            {
                switch (num)
                {
                    case 1f:
                        return randomLevelLowest;
                    case 2f:
                        return randomLevelLow;
                    case 3f:
                        return randomLevelMid;
                    case 4f:
                        return randomLevelHigh;
                    case 5f:
                        return randomLevelHighest;
                }
            }
            return -100f;
        }

        public static string GetDinnerTimeModeStr()
        {
            switch (dinnerTimeMode)
            {
                case 0:
                    return "Dinner_Time_Mode0";
                case 1:
                    return "Dinner_Time_Mode1";
                case 2:
                    return "Dinner_Time_Mode2";
            }
            return "dinnerTimeMode null";
        }
    }
}
