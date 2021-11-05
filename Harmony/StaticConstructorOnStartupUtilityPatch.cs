using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace RecycleThis
{
    [HarmonyPatch(typeof(StaticConstructorOnStartupUtility), "CallAll")]
    class StaticConstructorOnStartupUtilityPatch
    {
        public static void Postfix()
        {
            try
            {
                RuntimeHelpers.RunClassConstructor(typeof(RecycleThisUtility).TypeHandle);
            }
            catch (Exception ex)
            {
                Log.Error(string.Concat("Error in static constructor of ", typeof(RecycleThisUtility), ": ", ex));
            }
        }

    }
}
