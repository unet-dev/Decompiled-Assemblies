using Mono.Cecil;
using System;

namespace Mono.Cecil.PE
{
	internal sealed class TextMap
	{
		private readonly Range[] map = new Range[16];

		public TextMap()
		{
		}

		public void AddMap(TextSegment segment, int length)
		{
			this.map[(int)segment] = new Range(this.GetStart(segment), (uint)length);
		}

		public void AddMap(TextSegment segment, int length, int align)
		{
			align--;
			this.AddMap(segment, length + align & ~align);
		}

		public void AddMap(TextSegment segment, Range range)
		{
			this.map[(int)segment] = range;
		}

		private uint ComputeStart(int index)
		{
			index--;
			return this.map[index].Start + this.map[index].Length;
		}

		public DataDirectory GetDataDirectory(TextSegment segment)
		{
			uint start;
			Range range = this.map[(int)segment];
			if (range.Length == 0)
			{
				start = 0;
			}
			else
			{
				start = range.Start;
			}
			return new DataDirectory(start, range.Length);
		}

		public int GetLength(TextSegment segment)
		{
			return (int)this.map[(int)segment].Length;
		}

		public uint GetLength()
		{
			Range range = this.map[15];
			return range.Start - 8192 + range.Length;
		}

		public uint GetNextRVA(TextSegment segment)
		{
			int num = (int)segment;
			return this.map[num].Start + this.map[num].Length;
		}

		public Range GetRange(TextSegment segment)
		{
			return this.map[(int)segment];
		}

		public uint GetRVA(TextSegment segment)
		{
			return this.map[(int)segment].Start;
		}

		private uint GetStart(TextSegment segment)
		{
			int num = (int)segment;
			if (num == 0)
			{
				return (uint)8192;
			}
			return this.ComputeStart(num);
		}
	}
}