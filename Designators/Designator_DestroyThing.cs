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
    public class Designator_DestroyThing : Designator
    {
        public override int DraggableDimensions => 2;
        protected override DesignationDef Designation => DesignationDefOf.RecycleThisDestroy;

        public Designator_DestroyThing()
        {
            defaultLabel = "DesignatorDestroyThing".Translate();
            defaultDesc = "DesignatorDestroyThingDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("DestroyThisGizmo", true);
            useMouseIcon = true;
            soundSucceeded = RimWorld.SoundDefOf.Designate_Haul;
            hotKey = KeyBindingDefOf.Misc4;
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

        public override AcceptanceReport CanDesignateThing(Thing t)
        {

            if (base.Map.designationManager.DesignationOn(t, Designation) != null)
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
