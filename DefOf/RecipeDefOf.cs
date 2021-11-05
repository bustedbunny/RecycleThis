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
    public static class RecipeDefOf
    {
        public static RecipeDef SmeltWeapon;
        public static RecipeDef SmeltApparel;
        public static RecipeDef BurnApparel;

        public static RecipeDef RecycleThis;

        static RecipeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RimWorld.RecipeDefOf));
        }
    }


}
