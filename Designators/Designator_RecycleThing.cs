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
    public class Designator_RecycleThing : Designator
    {
        public override int DraggableDimensions => 2;
        protected override DesignationDef Designation => DesignationDefOf.RecycleThisRecycle;

        public Designator_RecycleThing()
        {
            defaultLabel = "DesignatorRecycleThing".Translate();
            defaultDesc = "DesignatorRecycleThingDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("RecycleThisGizmo", true);
            useMouseIcon = true;
            soundSucceeded = RimWorld.SoundDefOf.Designate_Haul;
            hotKey = KeyBindingDefOf.Misc3;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(base.Map) || c.Fogged(base.Map))
            {
                return false;
            }

            foreach (Thing thing in c.GetThingList(base.Map))
            {
                if (thing != null && CanDesignateThing(thing))
                {
                    return true;
                }
            }
            return false;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            List<Thing> list = base.Map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].def.stackLimit < 2 && (list[i].def.IsApparel || list[i].def.IsWeapon))
                {
                    DesignateThing(list[i]);
                }
            }
        }

        private bool SmeltingIsUseful(Thing t)
        {
            if (t.def.smeltProducts != null && t.def.smeltProducts.Count > 0) return true;

            List<ThingDefCountClass> costListAdj = t.def.CostListAdjusted(t.Stuff);
            for (int j = 0; j < costListAdj.Count; j++)
            {
                if (!costListAdj[j].thingDef.intricate)
                {
                    int num = GenMath.RoundRandom((float)costListAdj[j].count * 0.25f);
                    if (num > 0)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {

            if (base.Map.designationManager.DesignationOn(t, Designation) != null)
            {
                return false;
            }
            if (!SmeltingIsUseful(t))
            {
                return false;
            }
            if (t.def.stackLimit < 2 && (t.def.IsApparel || t.def.IsWeapon))
            {
                return true;
            }

            return false;

        }

        public override void DesignateThing(Thing t)
        {
            base.Map.designationManager.RemoveAllDesignationsOn(t);
            base.Map.designationManager.AddDesignation(new Designation(t, Designation));
        }

    }
}
