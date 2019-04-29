using System;

public class MinQueue
{
	private Deque<int> data;

	private Deque<int> min;

	public int Min
	{
		get
		{
			return this.min.Front;
		}
	}

	public MinQueue(int capacity = 8)
	{
		this.data = new Deque<int>(capacity);
		this.min = new Deque<int>(capacity);
	}

	public int Pop()
	{
		if (this.min.Front == this.data.Front)
		{
			this.min.PopFront();
		}
		return this.data.PopFront();
	}

	public void Push(int value)
	{
		this.data.PushBack(value);
		while (!this.min.IsEmpty && this.min.Back > value)
		{
			this.min.PopBack();
		}
		this.min.PushBack(value);
	}
}