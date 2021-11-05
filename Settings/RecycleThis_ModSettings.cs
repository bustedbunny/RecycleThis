using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace RecycleThis
{
    public class RecycleThis_ModSettings : ModSettings
    {

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }

    public class RecycleThisMod : Mod
    {
        public RecycleThisMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("RecycleThis.patch");
            harmony.PatchAll();
        }
    }
}
