using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.DataStructures
{
	public class Matrix<T>
	{
		private T[,] _matrix;

		private int _columns;

		private int _rows;

		public int columns
		{
			get
			{
				return this._columns;
			}
		}

		public T this[int column, int row]
		{
			get
			{
				if (!this.InBounds(column, row))
				{
					return default(T);
				}
				return this._matrix[column, row];
			}
		}

		public IEnumerable<T> items
		{
			get
			{
				Matrix<T> matrix = null;
				for (int i = 0; i < matrix._columns; i++)
				{
					for (int j = 0; j < matrix._rows; j++)
					{
						yield return matrix._matrix[i, j];
					}
				}
			}
		}

		public T[,] rawMatrix
		{
			get
			{
				return this._matrix;
			}
		}

		public int rows
		{
			get
			{
				return this._rows;
			}
		}

		public Matrix(T[,] matrix)
		{
			this._matrix = matrix;
			this._columns = this._matrix.GetUpperBound(0) + 1;
			this._rows = this._matrix.GetUpperBound(1) + 1;
		}

		public Matrix(int columns, int rows)
		{
			this._columns = columns;
			this._rows = rows;
			this._matrix = new T[this._columns, this._rows];
		}

		protected int AdjustColumnToBounds(int x)
		{
			if (x < 0)
			{
				return 0;
			}
			if (x <= this._columns - 1)
			{
				return x;
			}
			return this._columns - 1;
		}

		protected int AdjustRowToBounds(int z)
		{
			if (z < 0)
			{
				return 0;
			}
			if (z <= this._rows - 1)
			{
				return z;
			}
			return this._rows - 1;
		}

		public void Apply(MatrixBounds bounds, Action<T> act)
		{
			this.Apply(bounds.minColumn, bounds.maxColumn, bounds.minRow, bounds.maxRow, act);
		}

		public void Apply(int fromColumn, int toColumn, int fromRow, int toRow, Action<T> act)
		{
			int bounds = this.AdjustColumnToBounds(fromColumn);
			int num = this.AdjustColumnToBounds(toColumn);
			int bounds1 = this.AdjustRowToBounds(fromRow);
			int num1 = this.AdjustRowToBounds(toRow);
			for (int i = bounds; i <= num; i++)
			{
				for (int j = bounds1; j <= num1; j++)
				{
					act(this._matrix[i, j]);
				}
			}
		}

		public void GetConcentricNeighbours(int column, int row, int cellDistance, DynamicArray<T> neighbours)
		{
			if (cellDistance < 0)
			{
				return;
			}
			if (cellDistance == 0)
			{
				neighbours.Add(this[column, row]);
				return;
			}
			int num = column - cellDistance;
			int num1 = column + cellDistance;
			int num2 = row - cellDistance;
			int num3 = row + cellDistance;
			int bounds = this.AdjustColumnToBounds(num);
			int bounds1 = this.AdjustColumnToBounds(num1);
			int bounds2 = this.AdjustRowToBounds(num2 + 1);
			int bounds3 = this.AdjustRowToBounds(num3 - 1);
			for (int i = bounds; i <= bounds1; i++)
			{
				if (num2 >= 0)
				{
					neighbours.Add(this._matrix[i, num2]);
				}
				if (num3 < this._rows)
				{
					neighbours.Add(this._matrix[i, num3]);
				}
			}
			for (int j = bounds2; j <= bounds3; j++)
			{
				if (num >= 0)
				{
					neighbours.Add(this._matrix[num, j]);
				}
				if (num1 < this._columns)
				{
					neighbours.Add(this._matrix[num1, j]);
				}
			}
		}

		public IEnumerable<T> GetConcentricNeighbours(int column, int row, int cellDistance)
		{
			Matrix<T> matrix = null;
			int i;
			if (cellDistance < 0)
			{
				yield break;
			}
			if (cellDistance == 0)
			{
				yield return matrix[column, row];
				yield break;
			}
			int num = column - cellDistance;
			int num1 = column + cellDistance;
			int num2 = row - cellDistance;
			int num3 = row + cellDistance;
			int bounds = matrix.AdjustColumnToBounds(num);
			int bounds1 = matrix.AdjustColumnToBounds(num1);
			int bounds2 = matrix.AdjustRowToBounds(num2 + 1);
			int bounds3 = matrix.AdjustRowToBounds(num3 - 1);
			for (i = bounds; i <= bounds1; i++)
			{
				if (num2 >= 0)
				{
					yield return matrix._matrix[i, num2];
				}
				if (num3 < matrix._rows)
				{
					yield return matrix._matrix[i, num3];
				}
			}
			for (i = bounds2; i <= bounds3; i++)
			{
				if (num >= 0)
				{
					yield return matrix._matrix[num, i];
				}
				if (num1 < matrix._columns)
				{
					yield return matrix._matrix[num1, i];
				}
			}
		}

		public IEnumerable<T> GetRange(MatrixBounds bounds)
		{
			return this.GetRange(bounds.minColumn, bounds.maxColumn, bounds.minRow, bounds.maxRow);
		}

		public IEnumerable<T> GetRange(int fromColumn, int toColumn, int fromRow, int toRow)
		{
			Matrix<T> matrix = null;
			int bounds = matrix.AdjustColumnToBounds(fromColumn);
			int num = matrix.AdjustColumnToBounds(toColumn);
			int bounds1 = matrix.AdjustRowToBounds(fromRow);
			int num1 = matrix.AdjustRowToBounds(toRow);
			for (int i = bounds; i <= num; i++)
			{
				for (int j = bounds1; j <= num1; j++)
				{
					yield return matrix._matrix[i, j];
				}
			}
		}

		public void GetRange(MatrixBounds bounds, ICollection<T> result)
		{
			this.GetRange(bounds.minColumn, bounds.maxColumn, bounds.minRow, bounds.maxRow, result);
		}

		public void GetRange(MatrixBounds bounds, Func<T, bool> predicate, ICollection<T> result)
		{
			this.GetRange(bounds.minColumn, bounds.maxColumn, bounds.minRow, bounds.maxRow, predicate, result);
		}

		public void GetRange(int fromColumn, int toColumn, int fromRow, int toRow, ICollection<T> result)
		{
			int bounds = this.AdjustColumnToBounds(fromColumn);
			int num = this.AdjustColumnToBounds(toColumn);
			int bounds1 = this.AdjustRowToBounds(fromRow);
			int num1 = this.AdjustRowToBounds(toRow);
			for (int i = bounds; i <= num; i++)
			{
				for (int j = bounds1; j <= num1; j++)
				{
					result.Add(this._matrix[i, j]);
				}
			}
		}

		public void GetRange(int fromColumn, int toColumn, int fromRow, int toRow, Func<T, bool> predicate, ICollection<T> result)
		{
			int bounds = this.AdjustColumnToBounds(fromColumn);
			int num = this.AdjustColumnToBounds(toColumn);
			int bounds1 = this.AdjustRowToBounds(fromRow);
			int num1 = this.AdjustRowToBounds(toRow);
			for (int i = bounds; i <= num; i++)
			{
				for (int j = bounds1; j <= num1; j++)
				{
					T t = this._matrix[i, j];
					if (predicate(t))
					{
						result.Add(t);
					}
				}
			}
		}

		protected bool InBounds(int x, int z)
		{
			if (x < 0 || x > this._columns - 1)
			{
				return false;
			}
			if (z >= 0 && z <= this._rows - 1)
			{
				return true;
			}
			return false;
		}
	}
}