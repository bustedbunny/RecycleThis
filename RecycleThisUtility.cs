﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace RecycleThis
{
    public static class RecycleThisUtility
    {
        public static readonly List<ThingDef> destroyBenches;
        public static readonly List<ThingDef> smeltBenches;
        public static readonly List<ThingDef> scrapBenches;

        static RecycleThisUtility()
        {
            List<ThingDef> smeltlist = new List<ThingDef>();
            List<ThingDef> scraplist = new List<ThingDef>();
            List<ThingDef> destroylist = new List<ThingDef>();
            foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
            {

                if (item.AllRecipes.Any(x => (x == RecipeDefOf.SmeltWeapon) || (x == RecipeDefOf.SmeltApparel)))
                {
                    smeltlist.Add(item);
                }
                else if (item.AllRecipes.Any(x => (x.defName == "Make_Apparel_BasicShirt") || (x.defName == "Make_Apparel_TribalA")))
                {
                    scraplist.Add(item);
                    var i = item.AllRecipes;
                    item.allRecipesCached.Insert(0, RecipeDefOf.RecycleThis);
                }
                else if (item.AllRecipes.Any(x => (x == RecipeDefOf.SmeltWeapon || x == RecipeDefOf.BurnApparel)))
                {
                    destroylist.Add(item);
                }
            }
            destroyBenches = destroylist;
            smeltBenches = smeltlist;
            scrapBenches = scraplist;
        }

        public static void CreateLists()
        {
            foreach (RecipeDef item in DefDatabase<RecipeDef>.AllDefs)
            {
                if (item.defName == "RecycleThis")
                {
                    item.recipeUsers.AddRange(RecycleThisUtility.scrapBenches);
                }
            }
        }
        public static T FailOnUnpoweredWorkbench<T>(this T f, Thing x) where T : IJobEndable
        {
            f.AddEndCondition(delegate
            {
                IBillGiver billGiver = x as IBillGiver;
                return (billGiver.CurrentlyUsableForBills()) ? JobCondition.Ongoing : JobCondition.Incompletable;
            });
            return f;
        }

        private static TraverseParms TraverseParms(Pawn pawn) => new TraverseParms()
        {
            pawn = pawn,
            mode = TraverseMode.ByPawn,
            maxDanger = Danger.Unspecified,
            fenceBlocked = false,
            canBashFences = false,
            canBashDoors = false
        };

        private static ThingRequest thingRequest = ThingRequest.ForGroup(ThingRequestGroup.Undefined);

        public static Thing ClosestSuitableWorkbench(Thing thing, Pawn pawn, List<ThingDef> benches, bool forced = false)
        {
            bool val(Thing bench)
            {
                if (!pawn.CanReserve(bench, 1, -1, null, forced))
                {
                    return false;
                }
                IBillGiver billGiver = bench as IBillGiver;
                if (billGiver.UsableForBillsAfterFueling())
                {
                    return true;
                }
                return false;
            }
            List<Thing> list = new List<Thing>();
            foreach (ThingDef def in benches)
            {
                list.AddRange(pawn.Map.listerThings.ThingsOfDef(def));
            }
            return GenClosest.ClosestThingReachable(thing.Position, thing.Map, thingRequest, PathEndMode.InteractionCell, TraverseParms(pawn), 9999f, val, list);
        }
        public static Thing ClosestSuitableWorkBenchDestroy(Thing thing, Pawn pawn, bool forced = false)
        {
            return ClosestSuitableWorkbench(thing, pawn, destroyBenches, forced);
        }
        public static Thing ClosestSuitableWorkbenchSmelt(Thing thing, Pawn pawn, bool forced = false)
        {
            return ClosestSuitableWorkbench(thing, pawn, smeltBenches, forced);
        }
        public static Thing ClosestSuitableWorkbenchScrap(Thing thing, Pawn pawn, bool forced = false)
        {
            return ClosestSuitableWorkbench(thing, pawn, scrapBenches, forced);
        }
    }
}
