using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace RecycleThis
{
    public class WorkGiver_RecycleThis : WorkGiver_Scanner
    {
        protected JobDef RecycleThing => JobDefOf.RecycleThisRecycle;

        protected DesignationDef Recycle => DesignationDefOf.RecycleThisRecycle;
        public override PathEndMode PathEndMode => PathEndMode.Touch;
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (pawn.Map.designationManager.AnySpawnedDesignationOfDef(Recycle))
            {
                return false;
            }
            return true;
        }
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {

            foreach (Designation item in pawn.Map.designationManager.SpawnedDesignationsOfDef(Recycle))
            {
                yield return item.target.Thing;
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!pawn.Map.designationManager.AllDesignationsOn(t).Any(x => x.def == DesignationDefOf.RecycleThisRecycle))
            {
                return null;
            }
            if (!pawn.CanReserve(t, 1, 1, null, forced) || t.IsBurning() || t.IsForbidden(pawn))
            {
                JobFailReason.Is("RecycleThisItemIsUnavailable".Translate(t));
                return null;
            }
            Thing Workbench;
            if (t.Smeltable)
            {
                Workbench = RecycleThisUtility.ClosestSuitableWorkbenchSmelt(t, pawn, forced);
            }
            else
            {
                Workbench = RecycleThisUtility.ClosestSuitableWorkbenchScrap(t, pawn, forced);
            }
            if (Workbench == null)
            {
                JobFailReason.Is("RecycleThisNoAvailableWorkbenches".Translate());
                return null;
            }
            if (!Workbench?.TryGetComp<CompRefuelable>()?.HasFuel ?? false)
            {
                if (!RefuelWorkGiverUtility.CanRefuel(pawn, Workbench, forced))
                {
                    JobFailReason.Is("RecycleThisCannotRefuel".Translate(Workbench));
                    return null;
                }
                return RefuelWorkGiverUtility.RefuelJob(pawn, Workbench, forced);
            }
            Job job;
            job = JobMaker.MakeJob(RecycleThing, t, Workbench, Workbench.InteractionCell);
            job.count = 1;
            return job;
        }



    }
}
