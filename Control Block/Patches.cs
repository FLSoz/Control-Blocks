using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace Control_Block
{
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