using System;
using System.Reflection;

public class SimpleList<T>
{
	private const int defaultCapacity = 16;

	private readonly static T[] emptyArray;

	public T[] array;

	public int count;

	public T[] Array
	{
		get
		{
			return this.array;
		}
	}

	public int Capacity
	{
		get
		{
			return (int)this.array.Length;
		}
		set
		{
			if (value != (int)this.array.Length)
			{
				if (value > 0)
				{
					T[] tArray = new T[value];
					if (this.count > 0)
					{
						System.Array.Copy(this.array, 0, tArray, 0, this.count);
					}
					this.array = tArray;
					return;
				}
				this.array = SimpleList<T>.emptyArray;
			}
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
			return this.array[index];
		}
		set
		{
			this.array[index] = value;
		}
	}

	static SimpleList()
	{
		SimpleList<T>.emptyArray = new T[0];
	}

	public SimpleList()
	{
		this.array = SimpleList<T>.emptyArray;
	}

	public SimpleList(int capacity)
	{
		this.array = (capacity == 0 ? SimpleList<T>.emptyArray : new T[capacity]);
	}

	public void Add(T item)
	{
		if (this.count == (int)this.array.Length)
		{
			this.EnsureCapacity(this.count + 1);
		}
		T[] tArray = this.array;
		int num = this.count;
		this.count = num + 1;
		tArray[num] = item;
	}

	public void Clear()
	{
		if (this.count > 0)
		{
			System.Array.Clear(this.array, 0, this.count);
			this.count = 0;
		}
	}

	public bool Contains(T item)
	{
		for (int i = 0; i < this.count; i++)
		{
			if (this.array[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(T[] array)
	{
		System.Array.Copy(this.array, 0, array, 0, this.count);
	}

	public void EnsureCapacity(int min)
	{
		if ((int)this.array.Length < min)
		{
			int num = (this.array.Length == 0 ? 16 : (int)this.array.Length * 2);
			num = (num < min ? min : num);
			this.Capacity = num;
		}
	}
}