using System;
using UnityEngine;

[Serializable]
public sealed class ByteQuadtree
{
	[SerializeField]
	private int size;

	[SerializeField]
	private int levels;

	[SerializeField]
	private ByteMap[] values;

	public ByteQuadtree.Element Root
	{
		get
		{
			return new ByteQuadtree.Element(this, 0, 0, this.levels - 1);
		}
	}

	public int Size
	{
		get
		{
			return this.size;
		}
	}

	public ByteQuadtree()
	{
	}

	private ByteMap CreateLevel(int level)
	{
		int num = 1 + (level + 3) / 4;
		return new ByteMap(1 << (this.levels - level - 1 & 31), num);
	}

	public void UpdateValues(byte[] baseValues)
	{
		this.size = Mathf.RoundToInt(Mathf.Sqrt((float)((int)baseValues.Length)));
		this.levels = Mathf.RoundToInt(Mathf.Log((float)this.size, 2f)) + 1;
		this.values = new ByteMap[this.levels];
		this.values[0] = new ByteMap(this.size, baseValues, 1);
		for (int i = 1; i < this.levels; i++)
		{
			ByteMap byteMap = this.values[i - 1];
			ByteMap[] byteMapArray = this.values;
			ByteMap byteMap1 = this.CreateLevel(i);
			ByteMap byteMap2 = byteMap1;
			byteMapArray[i] = byteMap1;
			ByteMap item = byteMap2;
			for (int j = 0; j < item.Size; j++)
			{
				for (int k = 0; k < item.Size; k++)
				{
					item[k, j] = byteMap[2 * k, 2 * j] + byteMap[2 * k + 1, 2 * j] + byteMap[2 * k, 2 * j + 1] + byteMap[2 * k + 1, 2 * j + 1];
				}
			}
		}
	}

	public struct Element
	{
		private ByteQuadtree source;

		private int x;

		private int y;

		private int level;

		public int ByteMap
		{
			get
			{
				return this.level;
			}
		}

		public ByteQuadtree.Element Child1
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2, this.level - 1);
			}
		}

		public ByteQuadtree.Element Child2
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2, this.level - 1);
			}
		}

		public ByteQuadtree.Element Child3
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2 + 1, this.level - 1);
			}
		}

		public ByteQuadtree.Element Child4
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2 + 1, this.level - 1);
			}
		}

		public Vector2 Coords
		{
			get
			{
				return new Vector2((float)this.x, (float)this.y);
			}
		}

		public bool IsLeaf
		{
			get
			{
				return this.level == 0;
			}
		}

		public bool IsRoot
		{
			get
			{
				return this.level == this.source.levels - 1;
			}
		}

		public ByteQuadtree.Element MaxChild
		{
			get
			{
				ByteQuadtree.Element child1 = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child1.Value;
				uint num = child2.Value;
				uint value1 = child3.Value;
				uint num1 = child4.Value;
				if (value >= num && value >= value1 && value >= num1)
				{
					return child1;
				}
				if (num >= value1 && num >= num1)
				{
					return child2;
				}
				if (value1 >= num1)
				{
					return child3;
				}
				return child4;
			}
		}

		public ByteQuadtree.Element Parent
		{
			get
			{
				if (this.IsRoot)
				{
					throw new Exception("Element is the root and therefore has no parent.");
				}
				return new ByteQuadtree.Element(this.source, this.x / 2, this.y / 2, this.level + 1);
			}
		}

		public ByteQuadtree.Element RandChild
		{
			get
			{
				ByteQuadtree.Element child1 = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child1.Value;
				uint num = child2.Value;
				uint value1 = child3.Value;
				uint num1 = child4.Value;
				float single = (float)((float)(value + num + value1 + num1));
				float single1 = UnityEngine.Random.@value;
				if ((float)((float)value) / single >= single1)
				{
					return child1;
				}
				if ((float)((float)(value + num)) / single >= single1)
				{
					return child2;
				}
				if ((float)((float)(value + num + value1)) / single >= single1)
				{
					return child3;
				}
				return child4;
			}
		}

		public uint Value
		{
			get
			{
				return this.source.values[this.level][this.x, this.y];
			}
		}

		public Element(ByteQuadtree source, int x, int y, int level)
		{
			this.source = source;
			this.x = x;
			this.y = y;
			this.level = level;
		}
	}
}