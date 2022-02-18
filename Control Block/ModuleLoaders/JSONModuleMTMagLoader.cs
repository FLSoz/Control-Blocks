using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Control_Block.Utils;
using UnityEngine;
using LogManager;
using NLog;

namespace Control_Block.ModuleLoaders
{
    public class JSONModuleMTMagLoader : JSONModuleLoader
	{
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		internal static void ConfigureLogger(Manager.LogTarget target)
		{
			Manager.RegisterLogger(logger, target);
		}

		public override string GetModuleKey()
        {
            return "ModuleMTMagnet";
        }

		public override bool CreateModuleForBlock(int blockID, ModdedBlockDefinition def, TankBlock block, JToken jToken)
		{
			logger.Trace(jToken);
			if (jToken.Type == JTokenType.Object)
			{
				JObject obj = (JObject)jToken;

				try
				{
					ModuleMTMagnet mtMag = base.GetOrAddComponent<ModuleMTMagnet>(block);
					mtMag.Identity = base.TryParseEnum<ModuleMTMagnet.MTMagTypes>(obj, "Identity", ModuleMTMagnet.MTMagTypes.Ball);
					mtMag.TransformCorrection = base.TryParse(obj, "TransformCorrection", mtMag.TransformCorrection);
					mtMag.VelocityCorrection = base.TryParse(obj, "VelocityCorrection", mtMag.VelocityCorrection);
					if (obj.TryGetValue("Effector", out JToken jtoken))
					{
						mtMag.Effector = CustomParser.GetVector3(jtoken, mtMag.Effector);
					}
					switch(mtMag.Identity)
                    {
						case ModuleMTMagnet.MTMagTypes.Ball:
							mtMag.ConfigureNewJoint = new Action<ModuleMTMagnet, ModuleMTMagnet>(CBallJoint);
							break;
						case ModuleMTMagnet.MTMagTypes.Fixed:
							mtMag.ConfigureNewJoint = new Action<ModuleMTMagnet, ModuleMTMagnet>(CFixedJoint);
							break;
						case ModuleMTMagnet.MTMagTypes.Swivel:
							mtMag.ConfigureNewJoint = new Action<ModuleMTMagnet, ModuleMTMagnet>(CSwivelJoint);
							break;
						case ModuleMTMagnet.MTMagTypes.LargeBall:
							mtMag.ConfigureNewJoint = new Action<ModuleMTMagnet, ModuleMTMagnet>(CBallJoint);
							break;
                    }
					return true;
				}
				catch (Exception e)
				{
					logger.Error(e);
					logger.Error("Destroying added ModuleBlockMover");
					ModuleMTMagnet failedMag = block.GetComponent<ModuleMTMagnet>();
					if (failedMag != null)
					{
						UnityEngine.GameObject.Destroy(failedMag);
					}
					return false;
                }
			}
			return false;
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
	}
}
