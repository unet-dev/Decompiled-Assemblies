using System;

public struct IntrusiveMinHeap<T>
where T : IMinHeapNode<T>
{
	private T head;

	public bool Empty
	{
		get
		{
			return this.head == null;
		}
	}

	public void Add(T item)
	{
		if (this.head == null)
		{
			this.head = item;
			return;
		}
		if (this.head.child == null && item.order <= this.head.order)
		{
			item.child = this.head;
			this.head = item;
			return;
		}
		T t = this.head;
		while (t.child != null && t.child.order < item.order)
		{
			t = t.child;
		}
		item.child = t.child;
		t.child = item;
	}

	public T Pop()
	{
		T t = this.head;
		this.head = this.head.child;
		t.child = default(T);
		return t;
	}
}