using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using TechComponentInjector;
using Control_Block.ModuleLoaders;

namespace Control_Block
{
    public class ControlBlocksMod : ModBase
    {
        internal const string HarmonyID = "aceba1.controlblocks";
        internal static Harmony harmony = new Harmony(HarmonyID);
        internal static bool inited = false;

        public override void DeInit()
        {
            ModuleBlockMover.DeInitNetworking();
            harmony.UnpatchAll(HarmonyID);
        }

        public override bool HasEarlyInit()
        {
            return true;
        }

        internal static bool PistonHeart = false;

        internal static void WorldShift()
        {
            PistonHeart = !PistonHeart;
        }

        public override void EarlyInit()
        {
            if (!inited)
            {
                GameObject _holder = new GameObject();
                //_holder.AddComponent<OptionMenuPiston>();
                //_holder.AddComponent<OptionMenuSwivel>();
                _holder.AddComponent<OptionMenuSteeringRegulator>();
                _holder.AddComponent<OptionMenuMover>();
                _holder.AddComponent<OptionMenuHoverPID>();
                _holder.AddComponent<LogGUI>();
                _holder.AddComponent<AdjustAttachPosition>();
                new GameObject().AddComponent<GUIOverseer>();
                ManWorldTreadmill.inst.OnBeforeWorldOriginMove.Subscribe(WorldShift);
                UnityEngine.Object.DontDestroyOnLoad(_holder);
                inited = true;
            }
        }

        public override void Init()
        {
            JSONBlockLoader.RegisterModuleLoader(new JSONModuleBlockMoverPiston());
            JSONBlockLoader.RegisterModuleLoader(new JSONModuleBlockMoverRail());
            JSONBlockLoader.RegisterModuleLoader(new JSONModuleBlockMoverSwivel());
            JSONBlockLoader.RegisterModuleLoader(new JSONModuleMTMagLoader());
            JSONBlockLoader.RegisterModuleLoader(new JSONModulePIDLoader());

            ModuleBlockMover.InitiateNetworking();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
