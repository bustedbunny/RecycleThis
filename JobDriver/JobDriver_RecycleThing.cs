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
    class JobDriver_RecycleThing : JobDriver
    {
        private float workLeft;
        private float totalNeededWork;
        protected Thing Target => job.targetA.Thing;
        protected Thing Workbench => job.targetB.Thing;
        protected DesignationDef Designation => DesignationDefOf.RecycleThisRecycle;

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
            this.FailOnDestroyedNullOrForbidden(ThingToDestroyInd);
            if (job.targetA.Thing.Smeltable)
            {
                this.FailOnUnpoweredWorkbench(Workbench);
            }

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
                totalNeededWork = (int)JobDefOf.RecycleThisRecycle.GetModExtension<RecycleThisModExtension>()?.RecycleSpeed;
                workLeft = totalNeededWork;
            };
            doWork.tickAction = delegate
            {
                Pawn actor = doWork.actor;
                Job curJob = actor.jobs.curJob;
                float num;
                if (Target.def?.recipeMaker?.workSpeedStat != null && Workbench.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor) > 0.01)
                {
                    num = actor.GetStatValue(Target.def.recipeMaker.workSpeedStat);
                    num *= Workbench.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor);
                }
                else
                {
                    num = 1f;
                }

                workLeft -= num;
                actor.GainComfortFromCellIfPossible(chairsOnly: true);
                
                if (workLeft <= 0f)
                {
                    doWork.actor.jobs.curDriver.ReadyForNextToil();
                }

            };
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            doWork.WithProgressBar(ThingToDestroyInd, () => 1f - workLeft / totalNeededWork);
            if (job.targetA.Thing.Smeltable)
            {
                doWork.WithEffect(() => EffecterDefOf.Smelt, WorkBenchInd);
                doWork.PlaySustainerOrSound(() => SoundDefOf.Recipe_Smelt);
            }
            else
            {
                doWork.WithEffect(() => RimWorld.EffecterDefOf.Sow, WorkBenchInd);
                doWork.PlaySustainerOrSound(() => SoundDefOf.Recipe_Tailor);
            }
            yield return doWork;

            Toil toil = new Toil
            {
                initAction = delegate
                {
                    float efficiency = (Target.def.recipeMaker.efficiencyStat != null) ? Target.GetStatValue(Target.def.recipeMaker.efficiencyStat) : 1f;

                    foreach (Thing item in Target.SmeltProducts(efficiency))
                    {
                        GenSpawn.Spawn(item, this.pawn.Position, base.Map);
                    }

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

