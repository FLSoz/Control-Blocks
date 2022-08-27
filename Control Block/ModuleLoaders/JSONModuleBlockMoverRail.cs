using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Control_Block;

namespace Control_Block.ModuleLoaders
{
    public class JSONModuleBlockMoverRail : JSONModuleLoader
    {
        private static Logger logger;
        internal static void ConfigureLogger(Logger.TargetConfig target)
        {
            logger = new Logger("Rail", target);
        }

        public override bool CreateModuleForBlock(int blockID, ModdedBlockDefinition def, TankBlock block, JToken data)
        {
            logger.Trace(data.ToString());
            if (data.Type == JTokenType.Object)
            {
                JObject obj = (JObject)data;
                try
                {
                    string initializer = base.TryParse(obj, "Type", "BFRailSegment");

                    if (initializer == "BFRailSegment")
                    {
                        ModuleBMSegment rail = base.GetOrAddComponent<ModuleBMSegment>(block);
                        return SetBFRail(rail, obj);
                    }
                    else
                    {
                        ModuleBMRail piston = base.GetOrAddComponent<ModuleBMRail>(block);
                        SetBFRailPiston(piston);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    logger.Error("Destroying added ModuleBlockMover");
                    ModuleBMSegment failedModuleRail = block.GetComponent<ModuleBMSegment>();
                    ModuleBMRail failedModulePiston = block.GetComponent<ModuleBMRail>();
                    if (failedModuleRail != null)
                    {
                        UnityEngine.GameObject.Destroy(failedModuleRail);
                    }
                    if (failedModulePiston != null)
                    {
                        UnityEngine.GameObject.Destroy(failedModulePiston);
                    }
                    return false;
                }
            }
            return false;
        }

        internal static void SetBFRailPiston(ModuleBMRail piston)
        {
            piston.m_thisHeadType = "BF_Rail_0";

            piston.usePosCurves = true;
            piston.useRotCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(), new AnimationCurve(), new AnimationCurve()
            };
            piston.rotCurves = new AnimationCurve[]
            {
                new AnimationCurve(), new AnimationCurve(), new AnimationCurve(), new AnimationCurve()
            };
            piston.PartCount = 1;
            piston.TrueMaxVELOCITY = 0.3f;
            piston.TrueLimitVALUE = 64f;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,0,1) // Pickup-AP
            };
            piston.SFX = TechAudio.SFXType.GSODrillSmall;
            piston.SFXVolume = 1f;

            piston.starterAnim = new AttachPoint()
            {
                apPos = Vector3.up * 0.5f,
                blockPos = IntVector3.up,
                apDirForward = Vector3.up,
                apDirUp = Vector3.forward,
                AnimLength = 0.5f,
                AnimPosChange = Vector3.up * 0.5f,
                Tangent = Vector3.up
            };
        }

        internal bool SetBFRail(ModuleBMSegment rail, JObject data) {
            string railType = base.TryParse(data, "HeadType", "BF_Rail_0");

            rail.blockMoverHeadType = railType;
            rail.AnimWeight = CustomParser.LenientTryParseFloat(data, "AnimWeight", rail.AnimWeight);

            if (data.TryGetValue("StartAP", out JToken startAP) && startAP.Type == JTokenType.Object)
            {
                logger.Trace($"Setting Start AP:\n{startAP}");
                JObject jObject = startAP as JObject;
                rail.startAP = new AttachPoint()
                {
                    AnimLength = CustomParser.LenientTryParseFloat(jObject, "AnimLength", 1f),
                    AnimPosChange = CustomParser.LenientTryParseVector3(jObject, "AnimPosChange", Vector3.zero),
                    DisableFreeJoint = CustomParser.TryGetBool(jObject, false, "DisableFreeJoint"),
                    Tangent = CustomParser.LenientTryParseVector3(jObject, "Tangent", Vector3.forward),
                    apPos = CustomParser.LenientTryParseVector3(jObject, "apPos", Vector3.forward),
                    blockPos = CustomParser.LenientTryParseIntVector3(jObject, "blockPos", IntVector3.forward),
                    apDirForward = CustomParser.LenientTryParseIntVector3(jObject, "apDirForward", IntVector3.forward),
                    apDirUp = CustomParser.LenientTryParseIntVector3(jObject, "apDirUp", IntVector3.forward)
                };
            }

            if (data.TryGetValue("EndAP", out JToken endAP) && endAP.Type == JTokenType.Object)
            {
                logger.Trace($"Setting End AP:\n{endAP}");
                JObject jObject = endAP as JObject;
                rail.endAP = new AttachPoint()
                {
                    AnimLength = CustomParser.LenientTryParseFloat(jObject, "AnimLength", 1f),
                    AnimPosChange = CustomParser.LenientTryParseVector3(jObject, "AnimPosChange", Vector3.zero),
                    DisableFreeJoint = CustomParser.TryGetBool(jObject, false, "DisableFreeJoint"),
                    Tangent = CustomParser.LenientTryParseVector3(jObject, "Tangent", Vector3.forward),
                    apPos = CustomParser.LenientTryParseVector3(jObject, "apPos", Vector3.forward),
                    blockPos = CustomParser.LenientTryParseIntVector3(jObject, "blockPos", IntVector3.forward),
                    apDirForward = CustomParser.LenientTryParseIntVector3(jObject, "apDirForward", IntVector3.forward),
                    apDirUp = CustomParser.LenientTryParseIntVector3(jObject, "apDirUp", IntVector3.forward)
                };
            }

            return true;
        }

        public override string GetModuleKey()
        {
            return "ModuleRail";
        }
    }
}
