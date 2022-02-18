using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using LogManager;
using NLog;

namespace Control_Block.ModuleLoaders
{
    public class JSONModuleBlockMoverSwivel : JSONModuleLoader
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        internal static void ConfigureLogger(Manager.LogTarget target)
        {
            Manager.RegisterLogger(logger, target);
        }

        public override bool CreateModuleForBlock(int blockID, ModdedBlockDefinition def, TankBlock block, JToken data)
        {
            logger.Trace(data);
            if (data.Type == JTokenType.Object)
            {
                JObject obj = (JObject)data;
                try
                {
                    ModuleBlockMover swivel = base.GetOrAddComponent<ModuleBlockMover>(block);

                    string initializer = base.TryParse(obj, "Function", "SetSmallSwivel");
                    MethodInfo method = typeof(JSONModuleBlockMoverSwivel).GetMethod(initializer, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    method.Invoke(null, new object[] { swivel });

                    return true;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    logger.Error("Destroying added ModuleBlockMover");
                    ModuleBlockMover failedModule = block.GetComponent<ModuleBlockMover>();
                    if (failedModule != null)
                    {
                        UnityEngine.GameObject.Destroy(failedModule);
                    }
                    return false;
                }
            }
            return false;
        }

        public override string GetModuleKey()
        {
            return "ModuleSwivel";
        }

        internal static void SetMediumSwivel(ModuleBlockMover swivel)
        {
            swivel.IsPlanarVALUE = true;
            swivel.useRotCurves = true;
            swivel.rotCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,0f,0f,1f), new Keyframe(360f,360f,1f,0f)),
                new AnimationCurve()
            };
            swivel.PartCount = 1;
            swivel.TrueMaxVELOCITY = 9;
            swivel.TrueLimitVALUE = 360;
            swivel.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0),
                new IntVector3(1,1,0),
                new IntVector3(0,1,1),
                new IntVector3(1,1,1)
            };
            swivel.SFX = TechAudio.SFXType.GCTripleBore;
            swivel.SFXVolume = 0.1f;
        }
        internal static void SetInlineSwivel(ModuleBlockMover swivel)
        {
            swivel.IsPlanarVALUE = true;
            swivel.useRotCurves = true;
            swivel.rotCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,0f,0f,1f), new Keyframe(360f,360f,1f,0f)),
                new AnimationCurve()
            };
            swivel.PartCount = 1;
            swivel.TrueMaxVELOCITY = 12;
            swivel.TrueLimitVALUE = 360;
            swivel.startblockpos = new IntVector3[]
            {
                new IntVector3(0,0,-1),
                new IntVector3(1,0,0),
                new IntVector3(0,0,1),
                new IntVector3(-1,0,0)
            };
            swivel.SFX = TechAudio.SFXType.GSODrillSmall;
            swivel.SFXVolume = 0.1f;
        }
        internal static void SetMediumInlineSwivel(ModuleBlockMover swivel)
        {
            swivel.IsPlanarVALUE = true;
            swivel.useRotCurves = true;
            swivel.rotCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,0f,0f,1f), new Keyframe(360f,360f,1f,0f)),
                new AnimationCurve()
            };
            swivel.PartCount = 1;
            swivel.TrueMaxVELOCITY = 6;
            swivel.TrueLimitVALUE = 360;
            swivel.InvPointWeightRatio = 0.12f;
            swivel.startblockpos = new IntVector3[]
            {
                new IntVector3(-1,0,0),
                new IntVector3(-1,0,1),
                new IntVector3(0,0,-1),
                new IntVector3(1,0,-1),
                new IntVector3(2,0,0),
                new IntVector3(2,0,1),
                new IntVector3(0,0,2),
                new IntVector3(1,0,2),
            };
            swivel.SFX = TechAudio.SFXType.GCBuzzSaw;
            swivel.SFXVolume = 0f;
        }
        internal static void SetSmallSwivel(ModuleBlockMover swivel)
        {
            swivel.IsPlanarVALUE = true;
            swivel.useRotCurves = true;
            swivel.rotCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,0f,0f,1f), new Keyframe(360f,360f,1f,0f)),
                new AnimationCurve()
            };
            swivel.PartCount = 1;
            swivel.TrueMaxVELOCITY = 6;
            swivel.TrueLimitVALUE = 360;
            swivel.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0)
            };
            swivel.SFX = TechAudio.SFXType.GSODrillSmall;
            swivel.SFXVolume = 0.2f;
        }
        internal static void SetDoubleSwivel(ModuleBlockMover swivel)
        {
            swivel.IsPlanarVALUE = true;
            swivel.useRotCurves = true;
            swivel.rotCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,0f,0f,1f), new Keyframe(360f,360f,1f,0f)),
                new AnimationCurve()
            };
            swivel.PartCount = 1;
            swivel.TrueMaxVELOCITY = 9;
            swivel.TrueLimitVALUE = 360;
            swivel.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0),
                new IntVector3(0,-1,0)
            };
            swivel.SFX = TechAudio.SFXType.GSODrillSmall;
            swivel.SFXVolume = 1f;
        }
    }
}
