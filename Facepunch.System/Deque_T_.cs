using System;

public class Deque<T>
{
	private T[] buffer;

	private int offset;

	private int count;

	public T Back
	{
		get
		{
			if (this.IsEmpty)
			{
				return default(T);
			}
			return this.buffer[(this.count + this.offset - 1) % this.Capacity];
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

	public T Front
	{
		get
		{
			if (this.IsEmpty)
			{
				return default(T);
			}
			return this.buffer[this.offset];
		}
	}

	public bool IsEmpty
	{
		get
		{
			return this.Count == 0;
		}
	}

	public bool IsFull
	{
		get
		{
			return this.Count == this.Capacity;
		}
	}

	public bool IsSplit
	{
		get
		{
			return this.offset > this.Capacity - this.Count;
		}
	}

	public Deque(int capacity = 8)
	{
		this.buffer = new T[capacity];
	}

	public void Clear()
	{
		int num = 0;
		int num1 = num;
		this.count = num;
		this.offset = num1;
	}

	public T PopBack()
	{
		if (this.IsEmpty)
		{
			return default(T);
		}
		T t = this.buffer[(this.count + this.offset - 1) % this.Capacity];
		this.count--;
		return t;
	}

	public T PopFront()
	{
		if (this.IsEmpty)
		{
			return default(T);
		}
		T t = this.buffer[this.offset];
		this.offset = (this.offset + 1) % this.Capacity;
		this.count--;
		return t;
	}

	public void PushBack(T value)
	{
		if (this.IsFull)
		{
			this.Resize(this.Capacity * 2);
		}
		this.buffer[(this.count + this.offset) % this.Capacity] = value;
		this.count++;
	}

	public void PushFront(T value)
	{
		if (this.IsFull)
		{
			this.Resize(this.Capacity * 2);
		}
		int num = this.offset - 1;
		this.offset = num;
		if (num < 0)
		{
			this.offset += this.Capacity;
		}
		this.buffer[this.offset] = value;
		this.count++;
	}

	public void Resize(int capacity)
	{
		if (capacity <= this.Capacity)
		{
			return;
		}
		T[] tArray = new T[capacity];
		if (!this.IsSplit)
		{
			Array.Copy(this.buffer, this.offset, tArray, 0, this.Count);
		}
		else
		{
			int num = this.Capacity - this.offset;
			Array.Copy(this.buffer, this.offset, tArray, 0, num);
			Array.Copy(this.buffer, 0, tArray, num, this.Count - num);
		}
		this.buffer = tArray;
		this.offset = 0;
	}
}