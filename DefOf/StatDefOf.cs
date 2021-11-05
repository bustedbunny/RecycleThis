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
    public static class StatDefOf
    {
        public static StatDef SmeltingSpeed;
        public static StatDef WorkTableWorkSpeedFactor;

        static StatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RimWorld.StatDefOf));
        }
    }


}
