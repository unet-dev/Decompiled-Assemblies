using System;
using System.Collections;
using System.Reflection;

namespace ProtoBuf.Meta
{
	internal class BasicList : IEnumerable
	{
		private readonly static BasicList.Node nil;

		protected BasicList.Node head = BasicList.nil;

		public int Count
		{
			get
			{
				return this.head.Length;
			}
		}

		public object this[int index]
		{
			get
			{
				return this.head[index];
			}
		}

		static BasicList()
		{
			BasicList.nil = new BasicList.Node(null, 0);
		}

		public BasicList()
		{
		}

		public int Add(object value)
		{
			BasicList.Node node = this.head.Append(value);
			BasicList.Node node1 = node;
			this.head = node;
			return node1.Length - 1;
		}

		internal bool Contains(object value)
		{
			BasicList.NodeEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!object.Equals(enumerator.Current, value))
				{
					continue;
				}
				return true;
			}
			return false;
		}

		public void CopyTo(Array array, int offset)
		{
			this.head.CopyTo(array, offset);
		}

		internal static BasicList GetContiguousGroups(int[] keys, object[] values)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if ((int)values.Length < (int)keys.Length)
			{
				throw new ArgumentException("Not all keys are covered by values", "values");
			}
			BasicList basicLists = new BasicList();
			BasicList.Group group = null;
			for (int i = 0; i < (int)keys.Length; i++)
			{
				if (i == 0 || keys[i] != keys[i - 1])
				{
					group = null;
				}
				if (group == null)
				{
					group = new BasicList.Group(keys[i]);
					basicLists.Add(group);
				}
				group.Items.Add(values[i]);
			}
			return basicLists;
		}

		public BasicList.NodeEnumerator GetEnumerator()
		{
			return new BasicList.NodeEnumerator(this.head);
		}

		internal int IndexOf(BasicList.MatchPredicate predicate, object ctx)
		{
			return this.head.IndexOf(predicate, ctx);
		}

		internal int IndexOfReference(object instance)
		{
			return this.head.IndexOfReference(instance);
		}

		internal int IndexOfString(string value)
		{
			return this.head.IndexOfString(value);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new BasicList.NodeEnumerator(this.head);
		}

		public void Trim()
		{
			this.head = this.head.Trim();
		}

		internal sealed class Group
		{
			public readonly int First;

			public readonly BasicList Items;

			public Group(int first)
			{
				this.First = first;
				this.Items = new BasicList();
			}
		}

		internal delegate bool MatchPredicate(object value, object ctx);

		internal sealed class Node
		{
			private readonly object[] data;

			private int length;

			public object this[int index]
			{
				get
				{
					if (index < 0 || index >= this.length)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return this.data[index];
				}
				set
				{
					if (index < 0 || index >= this.length)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					this.data[index] = value;
				}
			}

			public int Length
			{
				get
				{
					return this.length;
				}
			}

			internal Node(object[] data, int length)
			{
				this.data = data;
				this.length = length;
			}

			public BasicList.Node Append(object value)
			{
				object[] objArray;
				int num = this.length + 1;
				if (this.data == null)
				{
					objArray = new object[10];
				}
				else if (this.length != (int)this.data.Length)
				{
					objArray = this.data;
				}
				else
				{
					objArray = new object[(int)this.data.Length * 2];
					Array.Copy(this.data, objArray, this.length);
				}
				objArray[this.length] = value;
				return new BasicList.Node(objArray, num);
			}

			internal void Clear()
			{
				if (this.data != null)
				{
					Array.Clear(this.data, 0, (int)this.data.Length);
				}
				this.length = 0;
			}

			internal void CopyTo(Array array, int offset)
			{
				if (this.length > 0)
				{
					Array.Copy(this.data, 0, array, offset, this.length);
				}
			}

			internal int IndexOf(BasicList.MatchPredicate predicate, object ctx)
			{
				for (int i = 0; i < this.length; i++)
				{
					if (predicate(this.data[i], ctx))
					{
						return i;
					}
				}
				return -1;
			}

			internal int IndexOfReference(object instance)
			{
				for (int i = 0; i < this.length; i++)
				{
					if (instance == this.data[i])
					{
						return i;
					}
				}
				return -1;
			}

			internal int IndexOfString(string value)
			{
				for (int i = 0; i < this.length; i++)
				{
					if (value == (string)this.data[i])
					{
						return i;
					}
				}
				return -1;
			}

			public void RemoveLastWithMutate()
			{
				if (this.length == 0)
				{
					throw new InvalidOperationException();
				}
				this.length--;
			}

			public BasicList.Node Trim()
			{
				if (this.length == 0 || this.length == (int)this.data.Length)
				{
					return this;
				}
				object[] objArray = new object[this.length];
				Array.Copy(this.data, objArray, this.length);
				return new BasicList.Node(objArray, this.length);
			}
		}

		public struct NodeEnumerator : IEnumerator
		{
			private int position;

			private readonly BasicList.Node node;

			public object Current
			{
				get
				{
					return this.node[this.position];
				}
			}

			internal NodeEnumerator(BasicList.Node node)
			{
				this.position = -1;
				this.node = node;
			}

			public bool MoveNext()
			{
				int length = this.node.Length;
				if (this.position > length)
				{
					return false;
				}
				BasicList.NodeEnumerator nodeEnumerator = this;
				int num = nodeEnumerator.position + 1;
				int num1 = num;
				nodeEnumerator.position = num;
				return num1 < length;
			}

			void System.Collections.IEnumerator.Reset()
			{
				this.position = -1;
			}
		}
	}
}