using System;
using System.Collections;
using System.Collections.Generic;

namespace Mono.Collections.Generic
{
	public sealed class ReadOnlyCollection<T> : Collection<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
	{
		private static ReadOnlyCollection<T> empty;

		public static ReadOnlyCollection<T> Empty
		{
			get
			{
				ReadOnlyCollection<T> ts = ReadOnlyCollection<T>.empty;
				if (ts == null)
				{
					ts = new ReadOnlyCollection<T>();
					ReadOnlyCollection<T>.empty = ts;
				}
				return ts;
			}
		}

		bool System.Collections.Generic.ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		private ReadOnlyCollection()
		{
		}

		public ReadOnlyCollection(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			this.Initialize(array, (int)array.Length);
		}

		public ReadOnlyCollection(Collection<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException();
			}
			this.Initialize(collection.items, collection.size);
		}

		internal override void Grow(int desired)
		{
			throw new InvalidOperationException();
		}

		private void Initialize(T[] items, int size)
		{
			this.items = new T[size];
			Array.Copy(items, 0, this.items, 0, size);
			this.size = size;
		}

		protected override void OnAdd(T item, int index)
		{
			throw new InvalidOperationException();
		}

		protected override void OnClear()
		{
			throw new InvalidOperationException();
		}

		protected override void OnInsert(T item, int index)
		{
			throw new InvalidOperationException();
		}

		protected override void OnRemove(T item, int index)
		{
			throw new InvalidOperationException();
		}

		protected override void OnSet(T item, int index)
		{
			throw new InvalidOperationException();
		}
	}
}