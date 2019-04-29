using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

public class BufferList<T> : IEnumerable<T>, IEnumerable
{
	private int count;

	private T[] buffer;

	public T[] Buffer
	{
		get
		{
			return this.buffer;
		}
	}

	public int Capacity
	{
		get
		{
			return (int)this.buffer.Length;
		}
	}

	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public T this[int index]
	{
		get
		{
			return this.buffer[index];
		}
		set
		{
			this.buffer[index] = value;
		}
	}

	public BufferList(int capacity = 8)
	{
		this.buffer = new T[capacity];
	}

	public void Add(T element)
	{
		if (this.count == (int)this.buffer.Length)
		{
			Array.Resize<T>(ref this.buffer, (int)this.buffer.Length * 2);
		}
		this.buffer[this.count] = element;
		this.count++;
	}

	public void Clear()
	{
		if (this.count == 0)
		{
			return;
		}
		Array.Clear(this.buffer, 0, this.count);
		this.count = 0;
	}

	public bool Contains(T element)
	{
		return Array.IndexOf<T>(this.buffer, element) != -1;
	}

	public IEnumerator<T> GetEnumerator()
	{
		BufferList<T> ts = null;
		for (int i = 0; i < ts.count; i++)
		{
			yield return ts.buffer[i];
		}
	}

	public int IndexOf(T element)
	{
		return Array.IndexOf<T>(this.buffer, element);
	}

	public int LastIndexOf(T element)
	{
		return Array.LastIndexOf<T>(this.buffer, element);
	}

	public bool Remove(T element)
	{
		int num = Array.IndexOf<T>(this.buffer, element);
		if (num == -1)
		{
			return false;
		}
		this.RemoveAt(num);
		return true;
	}

	public void RemoveAt(int index)
	{
		for (int i = index; i < this.count - 1; i++)
		{
			this.buffer[i] = this.buffer[i + 1];
		}
		this.buffer[this.count - 1] = default(T);
		this.count--;
	}

	public void RemoveUnordered(int index)
	{
		this.buffer[index] = this.buffer[this.count - 1];
		this.buffer[this.count - 1] = default(T);
		this.count--;
	}

	public void Sort()
	{
		Array.Sort<T>(this.buffer);
	}

	IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}