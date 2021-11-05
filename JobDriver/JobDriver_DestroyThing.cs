using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RecycleThis
{
    class JobDriver_DestroyThing : JobDriver
    {
        private float workLeft;
        private float totalNeededWork;
        protected Thing Target => job.targetA.Thing;
        protected Thing Workbench => job.targetB.Thing;
        protected DesignationDef Designation => DesignationDefOf.RecycleThisDestroy;

        private const TargetIndex ThingToDestroyInd = TargetIndex.A;
        private const TargetIndex WorkBenchInd = TargetIndex.B;
        private const TargetIndex InteractionCellInd = TargetIndex.C;



        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (this.pawn.Reserve(this.job.GetTarget(WorkBenchInd), this.job, 1, 1, null))
            {
                return this.pawn.Reserve(this.job.GetTarget(ThingToDestroyInd), this.job, 1, 1, null);
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {

            this.FailOnThingMissingDesignation(ThingToDestroyInd, Designation);
            this.FailOnForbidden(ThingToDestroyInd);
            this.FailOnUnpoweredWorkbench(Workbench);

            yield return Toils_Reserve.Reserve(ThingToDestroyInd);

            yield return Toils_Goto.GotoThing(ThingToDestroyInd, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(ThingToDestroyInd);

            yield return Toils_Haul.StartCarryThing(ThingToDestroyInd, putRemainderInQueue: false);

            yield return Toils_Goto.GotoThing(WorkBenchInd, PathEndMode.InteractionCell).FailOnSomeonePhysicallyInteracting(WorkBenchInd);

            yield return Toils_JobTransforms.SetTargetToIngredientPlaceCell(WorkBenchInd, ThingToDestroyInd, InteractionCellInd);

            yield return Toils_Haul.PlaceHauledThingInCell(WorkBenchInd, null, false);

            yield return Toils_Reserve.Reserve(ThingToDestroyInd, 1);

            Toil doWork = new Toil().FailOnForbidden(ThingToDestroyInd);
            doWork.initAction = delegate
            {
                totalNeededWork = (int)JobDefOf.RecycleThisDestroy?.GetModExtension<RecycleThisModExtension>()?.DestroySpeed;
                workLeft = totalNeededWork;
            };
            doWork.tickAction = delegate
            {
                Pawn actor = doWork.actor;
                Job curJob = actor.jobs.curJob;
                workLeft -= 1f;
                actor.GainComfortFromCellIfPossible(chairsOnly: true);


                if (workLeft <= 0f)
                {
                    doWork.actor.jobs.curDriver.ReadyForNextToil();
                }
            };
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            doWork.WithProgressBar(ThingToDestroyInd, () => 1f - workLeft / totalNeededWork);
            doWork.WithEffect(() => EffecterDefOf.Cremate, WorkBenchInd);
            doWork.PlaySustainerOrSound(() => SoundDefOf.Recipe_Cremate);
            yield return doWork;

            Toil toil = new Toil
            {
                initAction = delegate
                {
                    Target.Destroy(DestroyMode.Vanish);
                    base.Map.designationManager.RemoveAllDesignationsOn(Target);
                }
            };
            yield return toil;

            yield return Toils_Reserve.Release(WorkBenchInd);

            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workLeft, "workLeft", 0f);
            Scribe_Values.Look(ref totalNeededWork, "totalNeededWork", 0f);
        }
    }

}

