using System;
using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<TKey, TValue> current = enumerator.Current;
			keys.Add(current.Key);
			values.Add(current.Value);
		}
	}

	public void OnAfterDeserialize()
	{
		Clear();
		for (int i = 0; i < keys.Count; i++)
		{
			Add(keys[i], values[i]);
		}
	}
}
