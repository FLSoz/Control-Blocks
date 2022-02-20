using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using System.IO;
using Control_Block.ModuleLoaders;
using NLog;
using NLog.Targets;
using LogManager;

namespace Control_Block
{
    public class ControlBlocksMod : ModBase
    {
        internal const string HarmonyID = "aceba1.controlblocks";
        internal static Harmony harmony = new Harmony(HarmonyID);
        internal static bool inited = false;

        internal static readonly string TTSteamDir = Path.GetFullPath(Path.Combine(
            AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.GetName().Name == "Assembly-CSharp").First().Location
            .Replace("Assembly-CSharp.dll", ""), @"../../"
        ));
        // internal static readonly string TTSteamDir = @"E:/Steam/steamapps/common/TerraTech";
        internal static readonly string ModLogsDir = Path.Combine(TTSteamDir, "Logs/Control_Blocks");

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        internal static LogLevel logLevel = LogLevel.Info;
        internal static void ConfigureLogger()
        {
            Manager.LogConfig config = new Manager.LogConfig
            {
                layout = "${longdate} | ${level:uppercase=true:padding=-5:alignmentOnTruncation=left} | ${logger:shortName=true} | ${message}  ${exception}",
                keepOldFiles = false,
                defaultMinLevel = logLevel
            };
            Manager.RegisterLogger(logger, config);
        }

        private static void ReadLoggingLevel()
        {
            string logLevelStr = null;
            string generalLevel = CommandLineReader.GetArgument("+log_level");
            if (generalLevel != null)
            {
                logLevelStr = generalLevel;
            }
            string modLevel = CommandLineReader.GetArgument("+control_blocks_log_level");
            if (modLevel != null)
            {
                logLevelStr = modLevel;
            }

            if (logLevelStr != null)
            {
                logLevel = LogLevel.FromString(logLevelStr);
            }
            Console.WriteLine($"[ControlBlocks] Logging at level {logLevel}");
        }

        public void ManagedEarlyInit()
        {
            ReadLoggingLevel();

            // Main mod logging
            ControlBlocksMod.ConfigureLogger();

            // JSON Module loader logging
            Manager.LogTarget target = Manager.RegisterLoggingTarget("ModuleLoaders", new Manager.TargetConfig
            {
                layout = "${longdate} | ${level:uppercase=true:padding=-5:alignmentOnTruncation=left} | ${logger:shortName=true} | ${message}  ${exception}",
                path = "Control_Blocks"
            });
            JSONModuleBlockMoverPiston.ConfigureLogger(target);
            JSONModuleBlockMoverRail.ConfigureLogger(target);
            JSONModuleBlockMoverSwivel.ConfigureLogger(target);
            JSONModuleMTMagLoader.ConfigureLogger(target);
            JSONModulePIDLoader.ConfigureLogger(target);

            // Module logging
            ModuleBlockMover.ConfigureLogger();

            Manager.LogTarget railTarget = Manager.RegisterLoggingTarget("Rails", new Manager.TargetConfig
            {
                layout = "${longdate} | ${level:uppercase=true:padding=-5:alignmentOnTruncation=left} | ${logger:shortName=true} | ${message}  ${exception}",
                path = "Control_Blocks"
            });
            ModuleBMRail.ConfigureLogger(railTarget);
            ModuleBMSegment.ConfigureLogger(railTarget);

            // UI logging

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

        public override bool HasEarlyInit()
        {
            return true;
        }

        public override void DeInit()
        {
            ModuleBlockMover.DeInitNetworking();
            harmony.UnpatchAll(HarmonyID);
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
                if (AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.FullName).Where(name => name.Contains("ModManager")).Count() > 0)
                {
                    logger.Warn("EARLY INIT was CALLED for {Class}, but 0ModManager is present!", this.GetType().Name);
                }
                else
                {
                    logger.Warn("EARLY INIT was CALLED for {Class}, but 0ModManager is MISSING!", this.GetType().Name);
                    this.ManagedEarlyInit();
                }
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

            if (HasMotionBlocks(out Type floaterType, out MethodBase floaterFixedUpdate))
            {
                PatchMotionBlocks(floaterType, floaterFixedUpdate);
            }
        }

        internal static bool HasMotionBlocks(out Type floaterType, out MethodBase floaterFixedUpdate)
        {
            IEnumerable<Assembly> assembliesSearch = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.GetName().Name.Contains("Motion Blocks"));
            if (assembliesSearch.Count() > 0)
            {
                floaterType = assembliesSearch.First().GetType("MotionBlocks.ModuleFloater");
                floaterFixedUpdate = floaterType.GetMethod("FixedUpdate", BindingFlags.Instance| BindingFlags.Public | BindingFlags.NonPublic);

                MaxStrength = floaterType.GetField("MaxStrength", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MaxHeight = floaterType.GetField("MaxHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                VelocityDampen = floaterType.GetField("VelocityDampen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                return true;
            }
            floaterType = null;
            floaterFixedUpdate = null;
            return false;
        }

        internal static void PatchMotionBlocks(Type floaterType, MethodBase floaterFixedUpdate)
        {
            logger.Info("Patching Motion Blocks compatibility");
            MethodInfo prefixMethod = typeof(ControlBlocksMod).GetMethod("PrefixModuleFloater", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            logger.Info("Fetched generic Prefix function");
            MethodInfo wrappedPrefixMethod = prefixMethod.MakeGenericMethod(new Type[] { floaterType });
            logger.Info("Generated constructed prefix function");
            harmony.Patch(floaterFixedUpdate, prefix: new HarmonyMethod(wrappedPrefixMethod));
        }

        internal static FieldInfo MaxStrength;
        internal static FieldInfo MaxHeight;
        internal static FieldInfo VelocityDampen;

        internal static bool PrefixModuleFloater<T>(ref T __instance)
        {
            TankBlock block = (__instance as Module).block;
            if (block.IsAttached && block.tank != null && !block.tank.beam.IsActive)
            {
                PIDController pidController = block.tank.gameObject.GetComponent<PIDController>();
                if (pidController)
                {
                    float velocityDampen = (float) ControlBlocksMod.VelocityDampen.GetValue(__instance);
                    float maxHeight = (float)ControlBlocksMod.MaxHeight.GetValue(__instance);
                    float maxStrength = (float)ControlBlocksMod.MaxStrength.GetValue(__instance);

                    Vector3 blockCenter = block.centreOfMassWorld;
                    float blockForce = (maxStrength / maxHeight) * (maxHeight - blockCenter.y) - block.tank.rbody.GetPointVelocity(blockCenter).y * velocityDampen;
                    Vector3 force = Vector3.up;
                    if (maxStrength > 0)
                    {
                        force *= Mathf.Clamp(blockForce, 0f, maxStrength * 1.25f);
                    }
                    else
                    {
                        force *= Mathf.Clamp(blockForce, maxStrength * 1.25f, 0f);
                    }
                    block.tank.rbody.AddForceAtPosition(force, blockCenter, ForceMode.Impulse);
                    pidController.nonGravityThrust += force;

                    Vector3 localVector = block.tank.transform.InverseTransformVector(blockCenter);
                    pidController.nonManagedTorque += Vector3.Cross(localVector, force);
                    return false;
                }
            }
            return true;
        }
    }
}
