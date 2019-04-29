using System;

namespace Mono.Cecil
{
	internal abstract class MetadataTable<TRow> : MetadataTable
	where TRow : struct
	{
		internal TRow[] rows;

		internal int length;

		public sealed override int Length
		{
			get
			{
				return this.length;
			}
		}

		protected MetadataTable()
		{
		}

		public int AddRow(TRow row)
		{
			if ((int)this.rows.Length == this.length)
			{
				this.Grow();
			}
			TRow[] tRowArray = this.rows;
			int num = this.length;
			this.length = num + 1;
			tRowArray[num] = row;
			return this.length;
		}

		private void Grow()
		{
			TRow[] tRowArray = new TRow[(int)this.rows.Length * 2];
			Array.Copy(this.rows, tRowArray, (int)this.rows.Length);
			this.rows = tRowArray;
		}

		public override void Sort()
		{
		}
	}
}