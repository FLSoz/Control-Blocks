using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace Control_Block.ModuleLoaders
{
	public static class CustomParser
	{
		public static float LenientTryParseFloat(JObject obj, string key, float defaultValue)
		{
			JToken jtoken;
			if (obj.TryGetValue(key, out jtoken))
			{
				if (jtoken.Type == JTokenType.Float)
				{
					return jtoken.ToObject<float>();
				}
				else if (jtoken.Type == JTokenType.Integer)
				{
					return (float)jtoken.ToObject<int>();
				}
				else if (jtoken.Type == JTokenType.String)
				{
					if (float.TryParse(jtoken.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out float parsed))
					{
						return parsed;
					}
				}
				else if (jtoken.Type == JTokenType.Boolean)
				{
					return jtoken.ToObject<bool>() ? 1.0f : 0.0f;
				}
			}
			return defaultValue;
		}

		public static Vector3 GetVector3(JToken token)
		{
			Vector3 result = Vector3.zero;
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

		public static IntVector3 GetVector3Int(JToken token)
		{
			IntVector3 result = IntVector3.zero;
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

		public static Vector3 LenientTryParseVector3(JObject obj, string key, Vector3 defaultValue)
		{
			if (obj.TryGetValue(key, out JToken jtoken))
			{
				return GetVector3(jtoken);
			}
			return defaultValue;
		}

		public static IntVector3 LenientTryParseIntVector3(JObject obj, string key, IntVector3 defaultValue)
		{
			if (obj.TryGetValue(key, out JToken jtoken))
			{
				return GetVector3Int(jtoken);
			}
			return defaultValue;
		}

		private static Dictionary<string, HashSet<string>> GetCasePropertyMap(JObject jData)
		{
			Dictionary<string, HashSet<string>> lowercaseMap = new Dictionary<string, HashSet<string>>();
			foreach (JProperty property in jData.Properties())
			{
				string lower = property.Name.ToLower();
				if (lowercaseMap.ContainsKey(lower))
				{
					lowercaseMap[lower].Add(property.Name);
				}
				else
				{
					lowercaseMap[lower] = new HashSet<string> { property.Name };
				}
			}
			return lowercaseMap;
		}

		private static int CompareStringPrefix(string a, string b)
		{
			if (a is null || b is null)
			{
				return 0;
			}
			int score = 0;
			for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
			{
				if (a[i] == b[i])
				{
					score++;
				}
				else
				{
					return score;
				}
			}
			return score;
		}

		private static string GetClosestString(string value, HashSet<string> values)
		{
			int bestScore = 0;
			string bestTarget = values.First();
			foreach (string target in values)
			{
				int score = CompareStringPrefix(value, target);
				if (score > bestScore)
				{
					bestScore = score;
					bestTarget = target;
				}
			}
			return bestTarget;
		}

		public static bool TryGetBool(JObject jData, bool defaultValue, string key)
		{
			if (jData.TryGetValue(key, out JToken jToken) && jToken.Type == JTokenType.Boolean)
			{
				return jToken.ToObject<bool>();
			}
			else
			{
				Dictionary<string, HashSet<string>> lowerMap = GetCasePropertyMap(jData);
				if (lowerMap.TryGetValue(key.ToLower(), out HashSet<string> values))
				{
					if (jData.TryGetValue(GetClosestString(key, values), out jToken) && jToken.Type == JTokenType.Boolean)
					{
						return jToken.ToObject<bool>();
					}
				}
			}
			return defaultValue;
		}
	}
}
