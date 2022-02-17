using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Control_Block.Utils
{
    internal class CustomParser
    {
		internal static Vector3 GetVector3(JToken token, Vector3 defaultValue)
		{
			Vector3 result = defaultValue;
			if (token.Type == JTokenType.Object)
			{
				JObject jData = (JObject)token;

				if (jData.TryGetValue("x", out JToken xToken) && (xToken.Type == JTokenType.Integer || xToken.Type == JTokenType.Float))
				{
					result.x = xToken.ToObject<float>();

				}
				else if (jData.TryGetValue("X", out xToken) && (xToken.Type == JTokenType.Integer || xToken.Type == JTokenType.Float))
				{
					result.x = xToken.ToObject<float>();
				}

				if (jData.TryGetValue("y", out JToken yToken) && (yToken.Type == JTokenType.Integer || yToken.Type == JTokenType.Float))
				{
					result.y = yToken.ToObject<float>();

				}
				else if (jData.TryGetValue("Y", out yToken) && (yToken.Type == JTokenType.Integer || yToken.Type == JTokenType.Float))
				{
					result.y = yToken.ToObject<float>();
				}

				if (jData.TryGetValue("z", out JToken zToken) && (zToken.Type == JTokenType.Integer || zToken.Type == JTokenType.Float))
				{
					result.z = zToken.ToObject<float>();

				}
				else if (jData.TryGetValue("Z", out zToken) && (zToken.Type == JTokenType.Integer || zToken.Type == JTokenType.Float))
				{
					result.z = zToken.ToObject<float>();
				}
			}
			else if (token.Type == JTokenType.Array)
			{
				JArray jList = (JArray)token;
				for (int i = 0; i < Math.Min(3, jList.Count); i++)
				{
					switch (i)
					{
						case 0:
							result.x = jList[i].ToObject<float>();
							break;
						case 1:
							result.y = jList[i].ToObject<float>();
							break;
						case 2:
							result.z = jList[i].ToObject<float>();
							break;
					}
				}
			}
			return result;
		}

		internal static IntVector3 GetVector3Int(JToken token, IntVector3 defaultValue)
		{
			IntVector3 result = defaultValue;
			if (token.Type == JTokenType.Object)
			{
				JObject jData = (JObject)token;

				if (jData.TryGetValue("x", out JToken xToken) && xToken.Type == JTokenType.Integer)
				{
					result.x = xToken.ToObject<int>();

				}
				else if (jData.TryGetValue("X", out xToken) && xToken.Type == JTokenType.Integer)
				{
					result.x = xToken.ToObject<int>();
				}

				if (jData.TryGetValue("y", out JToken yToken) && yToken.Type == JTokenType.Integer)
				{
					result.y = yToken.ToObject<int>();

				}
				else if (jData.TryGetValue("Y", out yToken) && yToken.Type == JTokenType.Integer)
				{
					result.y = yToken.ToObject<int>();
				}

				if (jData.TryGetValue("z", out JToken zToken) && zToken.Type == JTokenType.Integer)
				{
					result.z = zToken.ToObject<int>();

				}
				else if (jData.TryGetValue("Z", out zToken) && zToken.Type == JTokenType.Integer)
				{
					result.z = zToken.ToObject<int>();
				}
			}
			else if (token.Type == JTokenType.Array)
			{
				JArray jList = (JArray)token;
				for (int i = 0; i < Math.Min(3, jList.Count); i++)
				{
					switch (i)
					{
						case 0:
							result.x = jList[i].ToObject<int>();
							break;
						case 1:
							result.y = jList[i].ToObject<int>();
							break;
						case 2:
							result.z = jList[i].ToObject<int>();
							break;
					}
				}
			}
			return result;
		}
	}
}
