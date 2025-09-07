using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializableDictionaryBase<TKey, TValue> : DrawableDictionary, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ISerializationCallbackReceiver
{
	[NonSerialized]
	private Dictionary<TKey, TValue> _dict;

	[NonSerialized]
	private IEqualityComparer<TKey> _comparer;

	[SerializeField]
	private TKey[] _keys;

	[SerializeField]
	private TValue[] _values;

	public IEqualityComparer<TKey> Comparer => _comparer;

	public int Count
	{
		get
		{
			if (_dict == null)
			{
				return 0;
			}
			return _dict.Count;
		}
	}

	public ICollection<TKey> Keys
	{
		get
		{
			if (_dict == null)
			{
				_dict = new Dictionary<TKey, TValue>(_comparer);
			}
			return _dict.Keys;
		}
	}

	public ICollection<TValue> Values
	{
		get
		{
			if (_dict == null)
			{
				_dict = new Dictionary<TKey, TValue>(_comparer);
			}
			return _dict.Values;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			if (_dict == null)
			{
				throw new KeyNotFoundException();
			}
			return _dict[key];
		}
		set
		{
			if (_dict == null)
			{
				_dict = new Dictionary<TKey, TValue>(_comparer);
			}
			_dict[key] = value;
		}
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	public SerializableDictionaryBase()
	{
		_dict = new Dictionary<TKey, TValue>();
	}

	public SerializableDictionaryBase(IEqualityComparer<TKey> comparer)
	{
		_comparer = comparer;
	}

	public void Add(TKey key, TValue value)
	{
		if (_dict == null)
		{
			_dict = new Dictionary<TKey, TValue>(_comparer);
		}
		_dict.Add(key, value);
	}

	public bool ContainsKey(TKey key)
	{
		if (_dict == null)
		{
			return false;
		}
		return _dict.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		if (_dict == null)
		{
			return false;
		}
		return _dict.Remove(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		if (_dict == null)
		{
			value = default(TValue);
			return false;
		}
		return _dict.TryGetValue(key, out value);
	}

	public void Clear()
	{
		if (_dict != null)
		{
			_dict.Clear();
		}
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		if (_dict == null)
		{
			_dict = new Dictionary<TKey, TValue>(_comparer);
		}
		((ICollection<KeyValuePair<TKey, TValue>>)_dict).Add(item);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		if (_dict == null)
		{
			return false;
		}
		return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Contains(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (_dict != null)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)_dict).CopyTo(array, arrayIndex);
		}
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		if (_dict == null)
		{
			return false;
		}
		return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Remove(item);
	}

	public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
	{
		if (_dict == null)
		{
			return default(Dictionary<TKey, TValue>.Enumerator);
		}
		return _dict.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		if (_dict == null)
		{
			return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
		}
		return _dict.GetEnumerator();
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		if (_dict == null)
		{
			return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
		}
		return _dict.GetEnumerator();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		if (_keys != null && _values != null)
		{
			if (_dict == null)
			{
				_dict = new Dictionary<TKey, TValue>(_keys.Length, _comparer);
			}
			else
			{
				_dict.Clear();
			}
			for (int i = 0; i < _keys.Length; i++)
			{
				if (i < _values.Length)
				{
					_dict[_keys[i]] = _values[i];
				}
				else
				{
					_dict[_keys[i]] = default(TValue);
				}
			}
		}
		_keys = null;
		_values = null;
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		if (_dict == null || _dict.Count == 0)
		{
			_keys = null;
			_values = null;
			return;
		}
		int count = _dict.Count;
		_keys = new TKey[count];
		_values = new TValue[count];
		int num = 0;
		Dictionary<TKey, TValue>.Enumerator enumerator = _dict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_keys[num] = enumerator.Current.Key;
			_values[num] = enumerator.Current.Value;
			num++;
		}
	}
}
