using System.Collections.Generic;

namespace BackpackSurvivors.System.Helper;

public static class DictionaryHelper
{
	public static Dictionary<TKey, TValue> SetupDictionaryFromSerialableDictionary<TKey, TValue>(SerializableDictionaryBase<TKey, TValue> sourceSerializableDictionary)
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		if (sourceSerializableDictionary != null)
		{
			foreach (KeyValuePair<TKey, TValue> item in sourceSerializableDictionary)
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary;
	}
}
