using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

public class ListHashSet<T> : IEnumerable<T>, IEnumerable
{
	private Dictionary<T, int> val2idx;

	private Dictionary<int, T> idx2val;

	private BufferList<T> vals;

	public int Count
	{
		get
		{
			return this.vals.Count;
		}
	}

	public T this[int index]
	{
		get
		{
			return this.vals[index];
		}
		set
		{
			this.vals[index] = value;
		}
	}

	public BufferList<T> Values
	{
		get
		{
			return this.vals;
		}
	}

	public ListHashSet(int capacity = 8)
	{
		this.val2idx = new Dictionary<T, int>(capacity);
		this.idx2val = new Dictionary<int, T>(capacity);
		this.vals = new BufferList<T>(capacity);
	}

	public void Add(T val)
	{
		int count = this.vals.Count;
		this.val2idx.Add(val, count);
		this.idx2val.Add(count, val);
		this.vals.Add(val);
	}

	public void AddRange(List<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			this.Add(list[i]);
		}
	}

	public void Clear()
	{
		if (this.Count == 0)
		{
			return;
		}
		this.val2idx.Clear();
		this.idx2val.Clear();
		this.vals.Clear();
	}

	public bool Contains(T val)
	{
		return this.val2idx.ContainsKey(val);
	}

	public IEnumerator<T> GetEnumerator()
	{
		ListHashSet<T> ts = null;
		for (int i = 0; i < ts.vals.Count; i++)
		{
			yield return ts.vals[i];
		}
	}

	public bool Remove(T val)
	{
		int num;
		if (!this.val2idx.TryGetValue(val, out num))
		{
			return false;
		}
		this.Remove(num, val);
		return true;
	}

	private void Remove(int idx_remove, T val_remove)
	{
		int count = this.vals.Count - 1;
		T item = this.idx2val[count];
		this.vals.RemoveUnordered(idx_remove);
		this.val2idx[item] = idx_remove;
		this.idx2val[idx_remove] = item;
		this.val2idx.Remove(val_remove);
		this.idx2val.Remove(count);
	}

	public bool RemoveAt(int idx)
	{
		T t;
		if (!this.idx2val.TryGetValue(idx, out t))
		{
			return false;
		}
		this.Remove(idx, t);
		return true;
	}

	IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}