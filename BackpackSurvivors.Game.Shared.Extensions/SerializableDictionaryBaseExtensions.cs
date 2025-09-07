using System.Collections.Generic;

namespace BackpackSurvivors.Game.Shared.Extensions;

internal static class SerializableDictionaryBaseExtensions
{
	internal static Dictionary<T1, T2> ToDictionary<T1, T2>(this SerializableDictionaryBase<T1, T2> source)
	{
		Dictionary<T1, T2> dictionary = new Dictionary<T1, T2>();
		foreach (KeyValuePair<T1, T2> item in source)
		{
			dictionary.Add(item.Key, item.Value);
		}
		return dictionary;
	}

	internal static T2 TryGet<T1, T2>(this SerializableDictionaryBase<T1, T2> source, T1 key, T2 defaultValue)
	{
		if (source.ContainsKey(key))
		{
			return source[key];
		}
		return defaultValue;
	}
}
