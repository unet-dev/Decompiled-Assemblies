using System;

namespace Apex.DataStructures
{
	public class CircularStack<T>
	{
		private T[] _array;

		private int _used;

		private int _head;

		public int count
		{
			get
			{
				return this._used;
			}
		}

		public CircularStack(int capacity)
		{
			this._array = new T[capacity];
		}

		public void Clear()
		{
			Array.Clear(this._array, 0, (int)this._array.Length);
			this._used = 0;
			this._head = -1;
		}

		public T Peek()
		{
			if (this._used == 0)
			{
				throw new InvalidOperationException("The stack is empty.");
			}
			return this._array[this._head];
		}

		public T Pop()
		{
			if (this._used == 0)
			{
				throw new InvalidOperationException("The stack is empty.");
			}
			T t = this._array[this._head];
			this._array[this._head] = default(T);
			this._used--;
			this._head--;
			if (this._head < 0 && this._used > 0)
			{
				this._head = (int)this._array.Length - 1;
			}
			return t;
		}

		public void Push(T item)
		{
			this._head = (this._head + 1) % (int)this._array.Length;
			this._array[this._head] = item;
			if (this._used < (int)this._array.Length)
			{
				this._used++;
			}
		}
	}
}