using System;
using System.Reflection;
using UnityEngine;

public class WorldSpaceGrid<T>
{
	public T[] Cells;

	public float CellSize;

	public float CellSizeHalf;

	public float CellSizeInverse;

	public float CellArea;

	public int CellCount;

	public int CellCountHalf;

	public T this[Vector3 worldPos]
	{
		get
		{
			return this[this.WorldToGridCoords(worldPos)];
		}
		set
		{
			this[this.WorldToGridCoords(worldPos)] = value;
		}
	}

	public T this[Vector2i cellCoords]
	{
		get
		{
			return this[cellCoords.x, cellCoords.y];
		}
		set
		{
			this[cellCoords.x, cellCoords.y] = value;
		}
	}

	public T this[int x, int y]
	{
		get
		{
			return this.Cells[y * this.CellCount + x];
		}
		set
		{
			this.Cells[y * this.CellCount + x] = value;
		}
	}

	public WorldSpaceGrid(float gridSize, float cellSize)
	{
		this.CellSize = cellSize;
		this.CellSizeHalf = cellSize * 0.5f;
		this.CellSizeInverse = 1f / cellSize;
		this.CellArea = cellSize * cellSize;
		this.CellCount = Mathf.CeilToInt(gridSize * this.CellSizeInverse);
		this.CellCountHalf = this.CellCount / 2;
		this.Cells = new T[this.CellCount * this.CellCount];
	}

	public Vector3 GridToWorldCoords(Vector2i cellPos)
	{
		float single = (float)(cellPos.x - this.CellCountHalf) * this.CellSize - this.CellSizeHalf;
		float single1 = (float)(cellPos.y - this.CellCountHalf) * this.CellSize - this.CellSizeHalf;
		return new Vector3(single, 0f, single1);
	}

	public Vector2i WorldToGridCoords(Vector3 worldPos)
	{
		int num = Mathf.CeilToInt(worldPos.x * this.CellSizeInverse);
		int num1 = Mathf.CeilToInt(worldPos.z * this.CellSizeInverse);
		int num2 = Mathx.Clamp(num + this.CellCountHalf, 0, this.CellCount - 1);
		num1 = Mathx.Clamp(num1 + this.CellCountHalf, 0, this.CellCount - 1);
		return new Vector2i(num2, num1);
	}
}