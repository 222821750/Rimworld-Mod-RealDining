using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace YC.RealDining.Resource
{
    public class ModData:GameComponent
    {
        //public static int test = 0;
        public static HadAteFoodType lastFoodType=new HadAteFoodType();
        public static HadAteFoodType llastFoodType = new HadAteFoodType();
        public static Dictionary<string, float> foodClassRandomVal = new Dictionary<string, float>();
        public static string findedInventoryFoodID;
        
        //public static Dictionary<string, string> lastFoodType;
        public Game game;

        public ModData(Game game)
        {
            this.game = game;
        }

        public ModData()
        {

        }

        public static string GetLastFoodType(Pawn pawn)
        {
            if (lastFoodType is null) lastFoodType = new HadAteFoodType();
            return lastFoodType.GetHadAteFoodType(pawn);
        }
        public static void SetLastFoodType(Pawn pawn, string foodType)
        {
            if (lastFoodType is null) lastFoodType = new HadAteFoodType();
            lastFoodType.SetHadAteFoodType(pawn, foodType);
        }

        public static string GetLlastFoodType(Pawn pawn)
        {
            if (llastFoodType is null) llastFoodType = new HadAteFoodType();
            return llastFoodType.GetHadAteFoodType(pawn);
        }
        public static void SetLlastFoodType(Pawn pawn, string foodType)
        {
            if (llastFoodType is null) llastFoodType = new HadAteFoodType();
            llastFoodType.SetHadAteFoodType(pawn, foodType);
        }

        public override void ExposeData()
        {
            //Scribe_Values.Look<int>(ref test,"testint",0);
            //Scribe_Collections.Look(ref lastFoodType, "LastFoodType_Map", LookMode.Value, LookMode.Value);
            Scribe_Deep.Look<HadAteFoodType>(ref lastFoodType, "lastFoodType", Array.Empty<object>());
            Scribe_Deep.Look<HadAteFoodType>(ref llastFoodType, "llastFoodType", Array.Empty<object>());
            
            //test++;
            //Log.Message("exposedata test {0}".Translate(test));
        }
    }
}
