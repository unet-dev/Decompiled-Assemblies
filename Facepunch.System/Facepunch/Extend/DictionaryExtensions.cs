using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Facepunch.Extend
{
	public static class DictionaryExtensions
	{
		public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dict)
		{
			Dictionary<TKey, TValue> tKeys = new Dictionary<TKey, TValue>(dict.Count, dict.Comparer);
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dict)
			{
				tKeys.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return tKeys;
		}

		public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		where TValue : new()
		{
			TValue tValue;
			if (dict.TryGetValue(key, out tValue))
			{
				return tValue;
			}
			tValue = Activator.CreateInstance<TValue>();
			dict.Add(key, tValue);
			return tValue;
		}

		public static TValue GetOrCreatePooled<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		where TValue : class, new()
		{
			TValue tValue;
			if (dict.TryGetValue(key, out tValue))
			{
				return tValue;
			}
			tValue = Pool.Get<TValue>();
			dict.Add(key, tValue);
			return tValue;
		}
	}
}