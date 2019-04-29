using System;

namespace Apex.DataStructures
{
	public struct MatrixBounds
	{
		public readonly static MatrixBounds nullBounds;

		public int minColumn;

		public int maxColumn;

		public int minRow;

		public int maxRow;

		public int columns
		{
			get
			{
				return this.maxColumn - this.minColumn + 1;
			}
		}

		public bool isEmpty
		{
			get
			{
				if (this.minColumn > this.maxColumn)
				{
					return true;
				}
				return this.minRow > this.maxRow;
			}
		}

		public int rows
		{
			get
			{
				return this.maxRow - this.minRow + 1;
			}
		}

		static MatrixBounds()
		{
			MatrixBounds.nullBounds = new MatrixBounds(-1, -1, -2, -2);
		}

		public MatrixBounds(int minColumn, int minRow, int maxColumn, int maxRow)
		{
			this.minColumn = minColumn;
			this.minRow = minRow;
			this.maxColumn = maxColumn;
			this.maxRow = maxRow;
		}

		public int AdjustColumnToBounds(int column)
		{
			if (column < this.minColumn)
			{
				return this.minColumn;
			}
			if (column <= this.maxColumn)
			{
				return column;
			}
			return this.maxColumn;
		}

		public int AdjustRowToBounds(int row)
		{
			if (row < this.minRow)
			{
				return this.minRow;
			}
			if (row <= this.maxRow)
			{
				return row;
			}
			return this.maxRow;
		}

		public static MatrixBounds Combine(MatrixBounds first, MatrixBounds second)
		{
			if (first.isEmpty)
			{
				return second;
			}
			if (second.isEmpty)
			{
				return first;
			}
			return new MatrixBounds(Math.Min(first.minColumn, second.minColumn), Math.Min(first.minRow, second.minRow), Math.Max(first.maxColumn, second.maxColumn), Math.Max(first.maxRow, second.maxRow));
		}

		public bool Contains(int column, int row)
		{
			if (column < this.minColumn || column > this.maxColumn || row < this.minRow)
			{
				return false;
			}
			return row <= this.maxRow;
		}

		public bool Contains(MatrixBounds other)
		{
			if (other.maxRow > this.maxRow || other.minRow < this.minRow || other.maxColumn > this.maxColumn)
			{
				return false;
			}
			return other.minColumn >= this.minColumn;
		}

		public int IndexOf(int column, int row)
		{
			if (column < this.minColumn || column > this.maxColumn || row < this.minRow || row > this.maxRow)
			{
				return -1;
			}
			return (column - this.minColumn) * this.rows + (row - this.minRow);
		}

		public override string ToString()
		{
			return string.Format("xmin: {0}, zmin: {1}, xmax: {2}, zmax: {3}", new object[] { this.minColumn, this.minRow, this.maxColumn, this.maxRow });
		}
	}
}