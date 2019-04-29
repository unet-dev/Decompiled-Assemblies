using System;

public class MaxQueue
{
	private Deque<int> data;

	private Deque<int> max;

	public int Max
	{
		get
		{
			return this.max.Front;
		}
	}

	public MaxQueue(int capacity = 8)
	{
		this.data = new Deque<int>(capacity);
		this.max = new Deque<int>(capacity);
	}

	public int Pop()
	{
		if (this.max.Front == this.data.Front)
		{
			this.max.PopFront();
		}
		return this.data.PopFront();
	}

	public void Push(int value)
	{
		this.data.PushBack(value);
		while (!this.max.IsEmpty && this.max.Back < value)
		{
			this.max.PopBack();
		}
		this.max.PushBack(value);
	}
}