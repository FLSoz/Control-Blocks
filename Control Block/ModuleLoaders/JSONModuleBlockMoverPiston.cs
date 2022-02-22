using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LogManager;
using NLog;
using System.Reflection;

namespace Control_Block.ModuleLoaders
{
    public class JSONModuleBlockMoverPiston : JSONModuleLoader
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
                JObject obj = (JObject) data;
                try
                {
                    ModuleBlockMover piston = base.GetOrAddComponent<ModuleBlockMover>(block);

                    string initializer = base.TryParse(obj, "Function", "SetGSOPiston");
                    MethodInfo method = typeof(JSONModuleBlockMoverPiston).GetMethod(initializer, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    method.Invoke(null, new object[] { piston });

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
            return "ModulePiston";
        }

        internal static void SetGSOPiston(ModuleBlockMover piston)
        {
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, .375f, 0f, 0f)), //shaft
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f)), //block top
                new AnimationCurve()
            };
            piston.PartCount = 2;
            piston.TrueMaxVELOCITY = 0.08f;
            piston.TrueLimitVALUE = 1f;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0)
            };
            piston.SFX = TechAudio.SFXType.GSODrillSmall;
            piston.SFXVolume = 1f;
        }
        internal static void SetGeoCorpPiston(ModuleBlockMover piston)
        {
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(2f, .5f, 0f, 0f)),
                new AnimationCurve(),
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(2f, 1.1f, 0f, 0f)),
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(2f, 2f, 0f, 0f)),
                new AnimationCurve()
            };
            piston.PartCount = 3;
            piston.TrueMaxVELOCITY = 0.06f;
            piston.TrueLimitVALUE = 2;
            //piston.StretchModifier = 2; piston.MaxStr = 2;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,2,0),
                new IntVector3(0,2,1),
                new IntVector3(1,2,0),
                new IntVector3(1,2,1)
            };
            piston.SFX = TechAudio.SFXType.GCPlasmaCutter;
            piston.SFXVolume = 1f;
        }
        internal static void SetHawkeyePiston(ModuleBlockMover piston)
        {
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, .5f), new Keyframe(1f, .5f, .5f,  0f), new Keyframe(2f,  .5f,  0f,  0f), new Keyframe(3f,  .5f, 0f, 0f)), //shaft bottom
                new AnimationCurve(),
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, .5f), new Keyframe(1f, .5f, .5f, .5f), new Keyframe(2f,   1f, .5f,  0f), new Keyframe(3f,   1f, 0f, 0f)), //shaft mid bottom
                new AnimationCurve(),
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, .5f), new Keyframe(1f, .5f, .5f, .5f), new Keyframe(2f,   1f, .5f,  1f), new Keyframe(3f,   2f, 1f, 0f)), //shaft mid top
                new AnimationCurve(),
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, .5f), new Keyframe(1f, .5f, .5f,  1f), new Keyframe(2f, 1.5f,  1f,  1f), new Keyframe(3f, 2.5f, 1f, 0f)), //shaft top
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 1f), new Keyframe(2f, 2f, 1f, 1f), new Keyframe(3f, 3f, 1f, 0f)), //block top
                new AnimationCurve()
            };
            piston.PartCount = 5;
            piston.TrueMaxVELOCITY = 0.075f;
            piston.TrueLimitVALUE = 3;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0),
                new IntVector3(0,0,-1)
            };
            piston.SFX = TechAudio.SFXType.GCTripleBore;
            piston.SFXVolume = 0.9f;
        }
        internal static void SetHawkeyePanelPiston(ModuleBlockMover piston)
        {
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,  0f,  0f, .5f), new Keyframe(1f, .5f, .5f,  0f), new Keyframe(2f, .5f,  0f,  0f)), //shaft bottom
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,  0f,  0f, .5f), new Keyframe(1f, .5f, .5f, .5f), new Keyframe(2f,  1f, .5f,  0f)), //shaft mid bottom
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,  0f,  0f, .5f), new Keyframe(1f, .5f, .5f,  1f), new Keyframe(2f,1.5f,  1f,  0f)), //shaft mid top
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,  0f,  0f,  1f), new Keyframe(1f,  1f,  1f,  1f), new Keyframe(2f,  2f,  1f,  0f)), //block top
                new AnimationCurve()
            };
            piston.PartCount = 4;
            piston.TrueMaxVELOCITY = 0.075f;
            piston.TrueLimitVALUE = 2;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,-1)
            };
            piston.SFX = TechAudio.SFXType.GCTripleBore;
            piston.SFXVolume = 1f;
        }
        internal static void SetHawkeyePanelDecoPiston(ModuleBlockMover piston)
        {
            piston.CanOnlyBeLockJoint = true;
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f,  0f,  0f, .6f), new Keyframe(1f, 1f, .6f,  0f), new Keyframe(2f, 1f,  0f,  0f)), //shaft bottom
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f,  0f, .6f), new Keyframe(1f, 1f, .6f, .6f), new Keyframe(2f,  2f, .6f,  0f)), //shaft mid bottom
                new AnimationCurve(),
            };
            piston.PartCount = 2;
            piston.TrueMaxVELOCITY = 0.075f;
            piston.TrueLimitVALUE = 2;
            piston.startblockpos = new IntVector3[0];
            piston.SFX = TechAudio.SFXType.GCTripleBore;
            piston.SFXVolume = 1f;
        }
        internal static void SetBFPiston(ModuleBlockMover piston)
        {
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f)), //block top
                new AnimationCurve()
            };
            piston.PartCount = 1;
            piston.TrueMaxVELOCITY = 0.15f;
            piston.TrueLimitVALUE = 1f;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0)
            };
            piston.SFX = TechAudio.SFXType.FlameThrowerPlasma;
            piston.SFXVolume = 1f;
        }
        internal static void SetVENPiston(ModuleBlockMover piston)
        {
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 0.3f, 0.3f, 0.3f), new Keyframe(2f, 0.6f, 0f, 0f)), //shaft_1
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 0.6f, 0.65f, 0.65f), new Keyframe(2f, 1.5f, 0f, 0f)), //shaft_2
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 1f, 1f), new Keyframe(2f, 2f, 0f, 0f)), //head
                new AnimationCurve(),
            };
            piston.useRotCurves = true;
            piston.rotCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 22.5f, 0f, 0f), new Keyframe(1f, 10f, -15f, -15f), new Keyframe(2f, -17.5f, 0f, 0f)), //shaft_1
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(), //shaft_2
                new AnimationCurve(),

                new AnimationCurve(),
                new AnimationCurve(), //head
                new AnimationCurve(),
            };
            piston.PartCount = 3;
            piston.TrueMaxVELOCITY = 0.12f;
            piston.TrueLimitVALUE = 2f;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0)
            };
            piston.SFX = TechAudio.SFXType.VENFlameThrower;
            piston.SFXVolume = 1f;
        }
        internal static void SetRRFloatingPiston(ModuleBlockMover piston)
        {
            piston.usePosCurves = true;
            piston.posCurves = new AnimationCurve[]
            {
                new AnimationCurve(),
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(5f, 5f, 1f, 0f)), //block top
                new AnimationCurve()
            };
            piston.PartCount = 1;
            piston.TrueMaxVELOCITY = 0.07f;
            piston.InvPointWeightRatio = 0.12f;
            piston.TrueLimitVALUE = 5f;
            piston.startblockpos = new IntVector3[]
            {
                new IntVector3(0,1,0)
            };
            piston.SFX = TechAudio.SFXType.GCBuzzSaw;
            piston.SFXVolume = 0f;

            // Setup line renderer
            SimpleConnectLineRenderer lineRenderer = piston.gameObject.GetComponent<SimpleConnectLineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = piston.gameObject.AddComponent<SimpleConnectLineRenderer>();
            }

            GameObject parentGO = piston.gameObject;
            GameObject pistonHead = null;
            for (int i = 0; i < parentGO.transform.childCount; i++)
            {
                Transform child = parentGO.transform.GetChild(i);
                if (child.name.Contains("BlockMover"))
                {
                    pistonHead = child.gameObject;
                }
            }

            // set refObj to be the second BlockMover Part
            lineRenderer.refObj = pistonHead;
            lineRenderer.strPos = new Vector3(0f, 0.3f, 0f);
            lineRenderer.refPos = new Vector3(0f, 0.4f, 0f);
            lineRenderer.width = 0.6f;
            lineRenderer.material = GetAnchorBeamMat();
        }

        internal static Material skyAnchorBeamMat = null;
        internal static Material GetAnchorBeamMat()
        {
            if (skyAnchorBeamMat != null)
            {
                return skyAnchorBeamMat;
            }
            else
            {
                foreach (Material mat in Resources.FindObjectsOfTypeAll<Material>())
                {
                    if (mat.name == "MAT_BF_SkyAnchor_Beam")
                    {
                        skyAnchorBeamMat = mat;
                        return mat;
                    }
                }
                return null;
            }
        }
    }
}
