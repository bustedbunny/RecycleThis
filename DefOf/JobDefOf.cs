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
    public static class JobDefOf
    {
        public static JobDef RecycleThisDestroy;
        public static JobDef RecycleThisRecycle;

        static JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
        }
    }
}