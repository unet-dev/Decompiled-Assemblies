using System;

public struct IntrusiveLinkedList<T>
where T : ILinkedListNode<T>
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
		item.next = this.head;
		this.head = item;
	}

	public T Pop()
	{
		T t = this.head;
		this.head = this.head.next;
		t.next = default(T);
		return t;
	}
}