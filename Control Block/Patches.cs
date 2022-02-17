using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace Control_Block
{
    public class Class1
    {
        const string MoverText = "\n Right click to configure this block.\n\n" +
            "This is a BlockMover. Blocks attached to the head of this will have their own physics separate from the body they are on, yet still restrained to the same tech. Like a multi-tech, but a single tech. ClusterTech.";
        const string FakeMoverText = "\n Right click to configure this block.\n\n" +
            "This is a decorative BlockMover. It does not separate physics bodies or move blocks at all, but it still posesses the programmability of a standard BlockMover.";

        internal static void CBallJoint(ModuleMTMagnet origin, ModuleMTMagnet body)
        {
            var Joint = origin.block.tank.gameObject.AddComponent<ConfigurableJoint>();
            Joint.autoConfigureConnectedAnchor = false;
            Joint.anchor = origin.LocalPosWithEffector;
            Joint.connectedAnchor = body.LocalPosWithEffector;
            Joint.enableCollision = true;
            Joint.connectedBody = body.block.tank.rbody;
            Joint.xMotion = ConfigurableJointMotion.Locked;
            Joint.yMotion = ConfigurableJointMotion.Locked;
            Joint.zMotion = ConfigurableJointMotion.Locked;
            Joint.angularXMotion = ConfigurableJointMotion.Free;
            Joint.angularYMotion = ConfigurableJointMotion.Free;
            Joint.angularZMotion = ConfigurableJointMotion.Free;
            origin.joint = Joint;
        }

        internal static void CFixedJoint(ModuleMTMagnet origin, ModuleMTMagnet body)
        {
            var Joint = origin.block.tank.gameObject.AddComponent<ConfigurableJoint>();
            Joint.autoConfigureConnectedAnchor = false;
            Joint.anchor = origin.LocalPosWithEffector;
            Joint.connectedAnchor = body.LocalPosWithEffector;
            Joint.enableCollision = true;
            Joint.connectedBody = body.block.tank.rbody;
            Joint.xMotion = ConfigurableJointMotion.Locked;
            Joint.yMotion = ConfigurableJointMotion.Locked;
            Joint.zMotion = ConfigurableJointMotion.Locked;
            Joint.angularXMotion = ConfigurableJointMotion.Locked;
            Joint.angularYMotion = ConfigurableJointMotion.Locked;
            Joint.angularZMotion = ConfigurableJointMotion.Locked;
            origin.joint = Joint;
        }

        internal static void CSwivelJoint(ModuleMTMagnet origin, ModuleMTMagnet body)
        {
            var Joint = origin.block.tank.gameObject.AddComponent<ConfigurableJoint>();
            Joint.autoConfigureConnectedAnchor = false;
            Joint.anchor = origin.LocalPosWithEffector;
            Joint.axis = origin.transform.up;
            Joint.secondaryAxis = -body.transform.up;
            Joint.connectedAnchor = body.LocalPosWithEffector;
            Joint.enableCollision = true;
            Joint.connectedBody = body.block.tank.rbody;
            Joint.xMotion = ConfigurableJointMotion.Locked;
            Joint.yMotion = ConfigurableJointMotion.Locked;
            Joint.zMotion = ConfigurableJointMotion.Locked;
            Joint.angularXMotion = ConfigurableJointMotion.Free;
            Joint.angularYMotion = ConfigurableJointMotion.Locked;
            Joint.angularZMotion = ConfigurableJointMotion.Locked;
            origin.joint = Joint;
        }

        public static string LogAllComponents(Transform SearchIn, bool Reflection = false, string Indenting = "")
        {
            string result = "";
            Component[] c = SearchIn.GetComponents<Component>();
            foreach (Component comp in c)
            {
                result += "\n" + Indenting + comp.name + " : " + comp.GetType().Name;
                if (comp is MeshRenderer renderer) result += " : Material (" + renderer.material.name + ")";
                if (comp is Animation anim)
                {
                    var clipc = anim.GetClipCount();
                    result += $" : Animation ({clipc} clips)";
                }
            }
            for (int i = SearchIn.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = SearchIn.transform.GetChild(i);
                result += LogAllComponents(child, Reflection, Indenting + "  ");
            }
            return result;
        }
    }
    internal class AdjustAttachPosition : MonoBehaviour
    {
        //public static Vector3 oLocalPos;
        public static Vector3 PointerPos;
        static readonly int PointerLayerMask = Globals.inst.layerTank.mask | Globals.inst.layerTankIgnoreTerrain.mask | Globals.inst.layerScenery.mask | Globals.inst.layerPickup.mask | Globals.inst.layerTerrain.mask;
        const float PointerDistance = 512f;
        private int lastBlockCount = -1;

        void LateUpdate()
        {
            try // Must put it in a try catch block because sometimes the camera dies internally
            {
                var ray = Singleton.camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, PointerDistance, PointerLayerMask, QueryTriggerInteraction.Ignore))
                {
                    PointerPos = hitInfo.point;
                }
                else
                {
                    PointerPos = ray.GetPoint(PointerDistance);
                }
                if (ManPointer.inst.DraggingItem != null && ManPointer.inst.DraggingFocusTech != null)
                {
                    float num = float.MaxValue;
                    Tank tank = ManPointer.inst.DraggingFocusTech;
                    ClusterBody tankbody = null;
                    var cast = PhysicsUtils.RaycastAllNonAlloc(Singleton.camera.ScreenPointToRay(Input.mousePosition), ManPointer.inst.PickupRange, Globals.inst.layerTank.mask, QueryTriggerInteraction.Ignore);
                    foreach (var hit in cast)
                    {
                        float distance = hit.distance;
                        if (distance < num)
                        {
                            num = distance;
                            ClusterBody component = hit.collider.GetComponent<ModuleBlockMover.ModuleBMPart>()?.parent.Holder;
                            if (component == null) component = hit.collider.GetComponentInParent<ClusterBody>();
                            if (component != null && (/*tank == null || */component.coreTank == tank))
                                tankbody = component;
                            else
                                tankbody = null;
                        }
                    }
                    if (tankbody != null)
                    {
                        if (Patches.DoOffsetAttachParticles == 0 || Patches.FocusedBody != tankbody || lastBlockCount != tank.blockman.blockCount)
                            ManTechBuilder.inst.ResetAPCollection();
                        lastBlockCount = tank.blockman.blockCount;

                        Patches.FocusedBody = tankbody;
                        Patches.FocusedTech = tankbody.coreTank.trans;
                        //if (tank == null)
                        //    ManPointer.inst.DraggingFocusTech = tankbody.coreTank;

                        Patches.DoOffsetAttachParticles = 4;
                    }
                    else if (Patches.DoOffsetAttachParticles == 0 && Patches.FocusedBody != null)
                    {
                        Patches.FocusedBody = null;
                        ManTechBuilder.inst.ResetAPCollection();
                        lastBlockCount = -1;
                    }
                }
            }
            catch { /* fail silently */ }
        }
    }

    internal class Patches
    {
        static Vector3 oP;
        static Quaternion oQ;
        public static Transform FocusedTech;
        public static ClusterBody FocusedBody;
        public static byte DoOffsetAttachParticles = 0;

        /// <summary>
        /// <see cref="BlockManager.TableCache"/>
        /// </summary>
        static FieldInfo BlockPlacementCollector_m_BlockTableCache = typeof(BlockPlacementCollector).GetField("m_BlockTableCache", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPatch(typeof(BlockPlacementCollector), "ResetState")]
        static class SeepInCustomAPTable
        {
            static void Postfix(BlockPlacementCollector __instance)
            {
                if (DoOffsetAttachParticles != 0 && FocusedBody != null && __instance.PlacementsValid)
                {
                    var tableCache = (BlockManager.TableCache)BlockPlacementCollector_m_BlockTableCache.GetValue(__instance);
                    byte[,,] newTable = new byte[BlockManager.MaxBlockLimit, BlockManager.MaxBlockLimit, BlockManager.MaxBlockLimit];
                    foreach (var pair in FocusedBody.ClusterAPBitField)
                    {
                        var i = pair.Key + tableCache.blockTableCentre;
                        newTable[i.x, i.y, i.z] = pair.Value;
                    }
                    tableCache.apTable = newTable;
                    BlockPlacementCollector_m_BlockTableCache.SetValue(__instance, tableCache);
                }
            }
        }

        [HarmonyPatch(typeof(ManTechBuilder), "Update")]
        static class UpdateAttachParticles_Offset
        {
            static void Prefix()
            {
                if (DoOffsetAttachParticles != 0)
                _Offset();
            }
            static void Postfix()
            {
                if (DoOffsetAttachParticles != 0)
                _undoOffset();
            }
        }

        [HarmonyPatch(typeof(ManPointer), "Update")]
        static class ManPointer_Offset
        {
            static void Prefix()
            {
                if (DoOffsetAttachParticles != 0)
                    _Offset();
            }
            static void Postfix()
            {
                if (DoOffsetAttachParticles != 0)
                    _undoOffset();
            }
        }

        static void _Offset()
        {
                if (FocusedBody == null || FocusedTech == null)
                {
                    DoOffsetAttachParticles = 0;
                    return;
                }
                oP = FocusedTech.position;
                oQ = FocusedTech.rotation;
                FocusedTech.position = FocusedBody.transform.position;
                FocusedTech.rotation = FocusedBody.transform.rotation;
        }

        static void _undoOffset()
        {
            DoOffsetAttachParticles--;
            FocusedTech.position = oP;
            FocusedTech.rotation = oQ;
        }
    }
}