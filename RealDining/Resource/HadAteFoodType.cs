using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace YC.RealDining.Resource
{
    public class HadAteFoodType:IExposable
    {
        private Dictionary<string, string> map = new Dictionary<string, string>();
        public static readonly string StrEmpty = "-1";

        public string GetHadAteFoodType(Pawn pawn)
        {
            if (map is null) map = new Dictionary<string, string>();
            if (!map.ContainsKey(pawn.GetUniqueLoadID())) return StrEmpty;
            return map[pawn.GetUniqueLoadID()];
        }
        public void SetHadAteFoodType(Pawn pawn,string foodType)
        {
            if (map is null) map = new Dictionary<string, string>();
            map[pawn.GetUniqueLoadID()] = foodType;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref map, "LastFoodType_Map",LookMode.Value,LookMode.Value);
        }
    }
}
