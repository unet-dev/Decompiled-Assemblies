using Mono;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Mono.Collections.Generic
{
	public class Collection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
	{
		internal T[] items;

		internal int size;

		private int version;

		public int Capacity
		{
			get
			{
				return (int)this.items.Length;
			}
			set
			{
				if (value < 0 || value < this.size)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.Resize(value);
			}
		}

		public int Count
		{
			get
			{
				return this.size;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= this.size)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.items[index];
			}
			set
			{
				this.CheckIndex(index);
				if (index == this.size)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.OnSet(value, index);
				this.items[index] = value;
			}
		}

		bool System.Collections.Generic.ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this.CheckIndex(index);
				try
				{
					this[index] = (T)value;
					return;
				}
				catch (InvalidCastException invalidCastException)
				{
				}
				catch (NullReferenceException nullReferenceException)
				{
				}
				throw new ArgumentException();
			}
		}

		public Collection()
		{
			this.items = Empty<T>.Array;
		}

		public Collection(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.items = new T[capacity];
		}

		public Collection(ICollection<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			this.items = new T[items.Count];
			items.CopyTo(this.items, 0);
			this.size = (int)this.items.Length;
		}

		public void Add(T item)
		{
			if (this.size == (int)this.items.Length)
			{
				this.Grow(1);
			}
			this.OnAdd(item, this.size);
			T[] tArray = this.items;
			int num = this.size;
			this.size = num + 1;
			tArray[num] = item;
			this.version++;
		}

		private void CheckIndex(int index)
		{
			if (index < 0 || index > this.size)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		public void Clear()
		{
			this.OnClear();
			Array.Clear(this.items, 0, this.size);
			this.size = 0;
			this.version++;
		}

		public bool Contains(T item)
		{
			return this.IndexOf(item) != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this.items, 0, array, arrayIndex, this.size);
		}

		public Collection<T>.Enumerator GetEnumerator()
		{
			return new Collection<T>.Enumerator(this);
		}

		internal virtual void Grow(int desired)
		{
			int num = this.size + desired;
			if (num <= (int)this.items.Length)
			{
				return;
			}
			num = System.Math.Max(System.Math.Max((int)this.items.Length * 2, 4), num);
			this.Resize(num);
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf<T>(this.items, item, 0, this.size);
		}

		public void Insert(int index, T item)
		{
			this.CheckIndex(index);
			if (this.size == (int)this.items.Length)
			{
				this.Grow(1);
			}
			this.OnInsert(item, index);
			this.Shift(index, 1);
			this.items[index] = item;
			this.version++;
		}

		protected virtual void OnAdd(T item, int index)
		{
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnInsert(T item, int index)
		{
		}

		protected virtual void OnRemove(T item, int index)
		{
		}

		protected virtual void OnSet(T item, int index)
		{
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num == -1)
			{
				return false;
			}
			this.OnRemove(item, num);
			this.Shift(num, -1);
			Array.Clear(this.items, this.size, 1);
			this.version++;
			return true;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.size)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.OnRemove(this.items[index], index);
			this.Shift(index, -1);
			Array.Clear(this.items, this.size, 1);
			this.version++;
		}

		protected void Resize(int new_size)
		{
			if (new_size == this.size)
			{
				return;
			}
			if (new_size < this.size)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.items = this.items.Resize<T>(new_size);
		}

		private void Shift(int start, int delta)
		{
			if (delta < 0)
			{
				start -= delta;
			}
			if (start < this.size)
			{
				Array.Copy(this.items, start, this.items, start + delta, this.size - start);
			}
			this.size += delta;
			if (delta < 0)
			{
				Array.Clear(this.items, this.size, -delta);
			}
		}

		IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
		{
			return new Collection<T>.Enumerator(this);
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			Array.Copy(this.items, 0, array, index, this.size);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Collection<T>.Enumerator(this);
		}

		int System.Collections.IList.Add(object value)
		{
			try
			{
				this.Add((T)value);
				return this.size - 1;
			}
			catch (InvalidCastException invalidCastException)
			{
			}
			catch (NullReferenceException nullReferenceException)
			{
			}
			throw new ArgumentException();
		}

		void System.Collections.IList.Clear()
		{
			this.Clear();
		}

		bool System.Collections.IList.Contains(object value)
		{
			return ((IList)this).IndexOf(value) > -1;
		}

		int System.Collections.IList.IndexOf(object value)
		{
			try
			{
				return this.IndexOf((T)value);
			}
			catch (InvalidCastException invalidCastException)
			{
			}
			catch (NullReferenceException nullReferenceException)
			{
			}
			return -1;
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.CheckIndex(index);
			try
			{
				this.Insert(index, (T)value);
				return;
			}
			catch (InvalidCastException invalidCastException)
			{
			}
			catch (NullReferenceException nullReferenceException)
			{
			}
			throw new ArgumentException();
		}

		void System.Collections.IList.Remove(object value)
		{
			try
			{
				this.Remove((T)value);
			}
			catch (InvalidCastException invalidCastException)
			{
			}
			catch (NullReferenceException nullReferenceException)
			{
			}
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		public T[] ToArray()
		{
			T[] tArray = new T[this.size];
			Array.Copy(this.items, 0, tArray, 0, this.size);
			return tArray;
		}

		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private Collection<T> collection;

			private T current;

			private int next;

			private readonly int version;

			public T Current
			{
				get
				{
					return this.current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					this.CheckState();
					if (this.next <= 0)
					{
						throw new InvalidOperationException();
					}
					return this.current;
				}
			}

			internal Enumerator(Collection<T> collection)
			{
				this = new Collection<T>.Enumerator()
				{
					collection = collection,
					version = collection.version
				};
			}

			private void CheckState()
			{
				if (this.collection == null)
				{
					throw new ObjectDisposedException(this.GetType().FullName);
				}
				if (this.version != this.collection.version)
				{
					throw new InvalidOperationException();
				}
			}

			public void Dispose()
			{
				this.collection = null;
			}

			public bool MoveNext()
			{
				this.CheckState();
				if (this.next < 0)
				{
					return false;
				}
				if (this.next >= this.collection.size)
				{
					this.next = -1;
					return false;
				}
				T[] tArray = this.collection.items;
				int num = this.next;
				this.next = num + 1;
				this.current = tArray[num];
				return true;
			}

			public void Reset()
			{
				this.CheckState();
				this.next = 0;
			}
		}
	}
}