using System;
using System.Collections.Generic;

public class MruDictionary<Key, Value>
{
	private int capacity;

	private Queue<LinkedListNode<KeyValuePair<Key, Value>>> recycled;

	private LinkedList<KeyValuePair<Key, Value>> list;

	private Dictionary<Key, LinkedListNode<KeyValuePair<Key, Value>>> dict;

	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	public MruDictionary(int capacity)
	{
		this.capacity = capacity;
		this.list = new LinkedList<KeyValuePair<Key, Value>>();
		this.dict = new Dictionary<Key, LinkedListNode<KeyValuePair<Key, Value>>>(capacity);
		this.recycled = new Queue<LinkedListNode<KeyValuePair<Key, Value>>>(capacity);
		for (int i = 0; i < capacity; i++)
		{
			Key key = default(Key);
			Value value = default(Value);
			this.recycled.Enqueue(new LinkedListNode<KeyValuePair<Key, Value>>(new KeyValuePair<Key, Value>(key, value)));
		}
	}

	public void Add(Key key, Value value)
	{
		LinkedListNode<KeyValuePair<Key, Value>> keyValuePair;
		if (this.dict.TryGetValue(key, out keyValuePair))
		{
			this.list.Remove(keyValuePair);
			this.list.AddFirst(keyValuePair);
			return;
		}
		if (this.dict.Count == this.capacity - 1)
		{
			this.RemoveLast();
		}
		keyValuePair = this.recycled.Dequeue();
		keyValuePair.Value = new KeyValuePair<Key, Value>(key, value);
		this.list.AddFirst(keyValuePair);
		this.dict.Add(key, this.list.First);
	}

	public void Clear()
	{
		this.list.Clear();
		this.dict.Clear();
	}

	public KeyValuePair<Key, Value> GetLast()
	{
		return this.list.Last.Value;
	}

	public void RemoveLast()
	{
		this.recycled.Enqueue(this.list.Last);
		Dictionary<Key, LinkedListNode<KeyValuePair<Key, Value>>> keys = this.dict;
		KeyValuePair<Key, Value> value = this.list.Last.Value;
		keys.Remove(value.Key);
		this.list.RemoveLast();
	}

	public bool Touch(Key key)
	{
		LinkedListNode<KeyValuePair<Key, Value>> linkedListNode;
		if (!this.dict.TryGetValue(key, out linkedListNode))
		{
			return false;
		}
		this.list.Remove(linkedListNode);
		this.list.AddFirst(linkedListNode);
		return true;
	}

	public bool TryGetValue(Key key, out Value value)
	{
		LinkedListNode<KeyValuePair<Key, Value>> linkedListNode;
		if (!this.dict.TryGetValue(key, out linkedListNode))
		{
			value = default(Value);
			return false;
		}
		value = linkedListNode.Value.Value;
		return true;
	}
}