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
    public static class EffecterDefOf
    {
        public static EffecterDef Cremate;
        public static EffecterDef Smelt;


        static EffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RimWorld.EffecterDefOf));
        }
    }


}
