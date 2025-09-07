using System.Collections.Generic;

namespace BackpackSurvivors.Game.Shared.Extensions;

internal static class DictionaryExtensions
{
	internal static T2 TryGet<T1, T2>(this Dictionary<T1, T2> source, T1 key, T2 defaultValue)
	{
		if (source.ContainsKey(key))
		{
			return source[key];
		}
		return defaultValue;
	}
}
