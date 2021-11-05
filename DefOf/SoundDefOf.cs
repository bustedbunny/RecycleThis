using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace RecycleThis
{
    [RimWorld.DefOf]
    public static class SoundDefOf
    {
        public static SoundDef Recipe_Cremate;
        public static SoundDef Recipe_Smelt;
        public static SoundDef Recipe_Tailor;


        static SoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RimWorld.SoundDefOf));
        }
    }


}
