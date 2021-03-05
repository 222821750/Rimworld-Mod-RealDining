using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace YC.RealDining.Resource.DefClass
{
    public class TimeAssignmentDefDinner:TimeAssignmentDef
    {
        public static TimeAssignmentDef DinnerDef;
        public TimeAssignmentDefDinner()
        {
            //Log.Message("instance");
            DinnerDef = this as TimeAssignmentDef;
        }
    }
}
