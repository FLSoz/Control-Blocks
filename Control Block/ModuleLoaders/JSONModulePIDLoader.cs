using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LogManager;
using NLog;

namespace Control_Block.ModuleLoaders
{
    public class JSONModulePIDLoader : JSONModuleLoader
    {
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		internal static void ConfigureLogger(Manager.LogTarget target)
		{
			Manager.RegisterLogger(logger, target);
		}

		public override string GetModuleKey()
        {
            return "ModulePID";
        }

		public override bool CreateModuleForBlock(int blockID, ModdedBlockDefinition def, TankBlock block, JToken jToken)
		{
			logger.Trace(jToken);
			if (jToken.Type == JTokenType.Object)
			{
				JObject obj = (JObject)jToken;
				try
				{
					ModulePID pid = base.GetOrAddComponent<ModulePID>(block);
					pid.enableHoldPosition = base.TryParse(obj, "HoldPosition", pid.enableHoldPosition);
					pid.manualTargetChangeRate = base.TryParse(obj, "ManualTargetChangeRate", pid.manualTargetChangeRate);
					pid.useTargetHeight = base.TryParse(obj, "UseTargetHeight", pid.useTargetHeight);
					pid.staticHeight = base.TryParse(obj, "StaticHeight", pid.staticHeight);
					pid.staticHeight = !base.TryParse(obj, "DynamicHeight", !pid.staticHeight);
					JToken jtoken;
					if (obj.TryGetValue("Hover", out jtoken))
					{
						bool debug = false;
						bool enabled = true;
						float kP = 300.0f;
						float kI = 10.0f;
						float kD = 600.0f;
						if (jtoken.Type == JTokenType.Object)
						{
							JObject axisParams = (JObject)jToken;
							debug = base.TryParse(axisParams, "debug", debug);
							kP = base.TryParse(axisParams, "kP", kP);
							kI = base.TryParse(axisParams, "kI", kI);
							kD = base.TryParse(axisParams, "kD", kD);
						}
						pid.AddParameters(PIDController.GenerateParameterInstance(PIDController.PIDParameters.PIDAxis.Hover, kP, kI, kD, debug, enabled));
					}
					if (obj.TryGetValue("Acceleration", out jtoken))
					{
						bool debug = false;
						bool enabled = true;
						float kP = 200.0f;
						float kI = 10.0f;
						float kD = 500.0f;
						if (jtoken.Type == JTokenType.Object)
						{
							JObject axisParams = (JObject)jToken;
							debug = base.TryParse(axisParams, "debug", debug);
							kP = base.TryParse(axisParams, "kP", kP);
							kI = base.TryParse(axisParams, "kI", kI);
							kD = base.TryParse(axisParams, "kD", kD);
						}
						pid.AddParameters(PIDController.GenerateParameterInstance(PIDController.PIDParameters.PIDAxis.Accel, kP, kI, kD, debug, enabled));
					}
					if (obj.TryGetValue("Strafe", out jtoken))
					{
						bool debug = false;
						bool enabled = true;
						float kP = 200.0f;
						float kI = 10.0f;
						float kD = 500.0f;
						if (jtoken.Type == JTokenType.Object)
						{
							JObject axisParams = (JObject)jToken;
							debug = base.TryParse(axisParams, "debug", debug);
							kP = base.TryParse(axisParams, "kP", kP);
							kI = base.TryParse(axisParams, "kI", kI);
							kD = base.TryParse(axisParams, "kD", kD);
						}
						pid.AddParameters(PIDController.GenerateParameterInstance(PIDController.PIDParameters.PIDAxis.Strafe, kP, kI, kD, debug, enabled));
					}
				}
				catch (Exception e)
				{
					logger.Error(e);
					logger.Error("Destroying added ModuleBlockMover");
					ModulePID failedComponent = block.GetComponent<ModulePID>();
					if (failedComponent != null)
					{
						UnityEngine.GameObject.Destroy(failedComponent);
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}
}
