using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace RecycleThis
{
    [HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
    public class Patch
    {
        public static void Postfix(ref ReverseDesignatorDatabase __instance)
        {
            __instance.AllDesignators.Add(new Designator_DestroyThing());
            __instance.AllDesignators.Add(new Designator_RecycleThing());
        }
    }



}
