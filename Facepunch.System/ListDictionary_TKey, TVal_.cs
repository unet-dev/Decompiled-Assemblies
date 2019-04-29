using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

public class ListDictionary<TKey, TVal> : IEnumerable<KeyValuePair<TKey, TVal>>, IEnumerable
{
	private Dictionary<TKey, int> key2idx;

	private Dictionary<int, TKey> idx2key;

	private BufferList<TKey> keys;

	private BufferList<TVal> vals;

	public int Count
	{
		get
		{
			return this.vals.Count;
		}
	}

	public TVal this[TKey key]
	{
		get
		{
			return this.vals[this.key2idx[key]];
		}
		set
		{
			this.vals[this.key2idx[key]] = value;
		}
	}

	public BufferList<TKey> Keys
	{
		get
		{
			return this.keys;
		}
	}

	public BufferList<TVal> Values
	{
		get
		{
			return this.vals;
		}
	}

	public ListDictionary(int capacity = 8)
	{
		this.key2idx = new Dictionary<TKey, int>(capacity);
		this.idx2key = new Dictionary<int, TKey>(capacity);
		this.keys = new BufferList<TKey>(capacity);
		this.vals = new BufferList<TVal>(capacity);
	}

	public void Add(TKey key, TVal val)
	{
		int count = this.keys.Count;
		this.key2idx.Add(key, count);
		this.idx2key.Add(count, key);
		this.keys.Add(key);
		this.vals.Add(val);
	}

	public void Clear()
	{
		if (this.Count == 0)
		{
			return;
		}
		this.key2idx.Clear();
		this.idx2key.Clear();
		this.keys.Clear();
		this.vals.Clear();
	}

	public bool Contains(TKey key)
	{
		return this.key2idx.ContainsKey(key);
	}

	public KeyValuePair<TKey, TVal> GetByIndex(int idx)
	{
		return new KeyValuePair<TKey, TVal>(this.idx2key[idx], this.vals[idx]);
	}

	public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
	{
		ListDictionary<TKey, TVal> tKeys = null;
		for (int i = 0; i < tKeys.vals.Count; i++)
		{
			yield return tKeys.GetByIndex(i);
		}
	}

	public bool Remove(TKey key)
	{
		int num;
		if (!this.key2idx.TryGetValue(key, out num))
		{
			return false;
		}
		this.Remove(num, key);
		return true;
	}

	private void Remove(int idx_remove, TKey key_remove)
	{
		int count = this.keys.Count - 1;
		TKey item = this.idx2key[count];
		this.keys.RemoveUnordered(idx_remove);
		this.vals.RemoveUnordered(idx_remove);
		this.key2idx[item] = idx_remove;
		this.idx2key[idx_remove] = item;
		this.key2idx.Remove(key_remove);
		this.idx2key.Remove(count);
	}

	public bool RemoveAt(int idx)
	{
		TKey tKey;
		if (!this.idx2key.TryGetValue(idx, out tKey))
		{
			return false;
		}
		this.Remove(idx, tKey);
		return true;
	}

	IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public bool TryGetValue(TKey key, out TVal val)
	{
		int num;
		if (!this.key2idx.TryGetValue(key, out num))
		{
			val = default(TVal);
			return false;
		}
		val = this.vals[num];
		return true;
	}
}