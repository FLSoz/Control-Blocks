using System;
using System.Linq;
using System.Reflection;
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

            logLevel = LogLevel.FromString(logLevelStr);
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
        }
    }
}
