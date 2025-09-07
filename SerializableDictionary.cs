using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
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

	public SerializableDictionary()
	{
		_dict = new Dictionary<TKey, TValue>();
	}

	public SerializableDictionary(IEqualityComparer<TKey> comparer)
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
}
