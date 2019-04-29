using Network;
using Network.Visibility;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkVisibilityGrid : MonoBehaviour, Provider
{
	public int startID = 1024;

	public int gridSize = 100;

	public int cellCount = 32;

	public int visibilityRadius = 2;

	public float switchTolerance = 20f;

	public NetworkVisibilityGrid()
	{
	}

	private void Awake()
	{
		Debug.Assert(Net.sv != null, "Network.Net.sv is NULL when creating Visibility Grid");
		Debug.Assert(Net.sv.visibility == null, "Network.Net.sv.visibility is being set multiple times");
		Net.sv.visibility = new Manager(this);
	}

	public float CellSize()
	{
		return (float)this.gridSize / (float)this.cellCount;
	}

	public uint CoordToID(int x, int y)
	{
		return (uint)(x * this.cellCount + y + this.startID);
	}

	public Bounds GetBounds(uint uid)
	{
		float single = this.CellSize();
		return new Bounds(this.GetPosition(uid), new Vector3(single, 1048576f, single));
	}

	public Group GetGroup(Vector3 vPos)
	{
		uint d = this.GetID(vPos);
		if (d == 0)
		{
			return null;
		}
		Group group = Net.sv.visibility.Get(d);
		if (!this.IsInside(group, vPos))
		{
			float single = group.bounds.SqrDistance(vPos);
			Debug.Log(string.Concat(new object[] { "Group is inside is all fucked ", d, "/", single, "/", vPos }));
		}
		return group;
	}

	public uint GetID(Vector3 vPos)
	{
		int grid = this.PositionToGrid(vPos.x);
		int num = this.PositionToGrid(vPos.z);
		if (grid < 0)
		{
			return (uint)0;
		}
		if (grid >= this.cellCount)
		{
			return (uint)0;
		}
		if (num < 0)
		{
			return (uint)0;
		}
		if (num >= this.cellCount)
		{
			return (uint)0;
		}
		uint d = this.CoordToID(grid, num);
		if ((ulong)d < (long)this.startID)
		{
			Debug.LogError(string.Concat(new object[] { "NetworkVisibilityGrid.GetID - group is below range ", grid, " ", num, " ", d, " ", this.cellCount }));
		}
		if ((ulong)d > (long)(this.startID + this.cellCount * this.cellCount))
		{
			Debug.LogError(string.Concat(new object[] { "NetworkVisibilityGrid.GetID - group is higher than range ", grid, " ", num, " ", d, " ", this.cellCount }));
		}
		return d;
	}

	public Vector3 GetPosition(uint uid)
	{
		uid -= this.startID;
		int num = (int)((ulong)uid / (long)this.cellCount);
		int num1 = (int)((ulong)uid % (long)this.cellCount);
		return new Vector3(this.GridToPosition(num), 0f, this.GridToPosition(num1));
	}

	public void GetVisibleFrom(Group group, List<Group> groups)
	{
		groups.Add(Net.sv.visibility.Get(0));
		uint d = group.ID;
		if ((ulong)d < (long)this.startID)
		{
			return;
		}
		d -= this.startID;
		int num = (int)((ulong)d / (long)this.cellCount);
		int num1 = (int)((ulong)d % (long)this.cellCount);
		groups.Add(Net.sv.visibility.Get(this.CoordToID(num, num1)));
		for (int i = 1; i <= this.visibilityRadius; i++)
		{
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num - i, num1)));
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num + i, num1)));
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num, num1 - i)));
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num, num1 + i)));
			for (int j = 1; j < i; j++)
			{
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num - i, num1 - j)));
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num - i, num1 + j)));
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num + i, num1 - j)));
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num + i, num1 + j)));
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num - j, num1 - i)));
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num + j, num1 - i)));
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num - j, num1 + i)));
				groups.Add(Net.sv.visibility.Get(this.CoordToID(num + j, num1 + i)));
			}
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num - i, num1 - i)));
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num - i, num1 + i)));
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num + i, num1 - i)));
			groups.Add(Net.sv.visibility.Get(this.CoordToID(num + i, num1 + i)));
		}
	}

	private float GridToPosition(int i)
	{
		return (float)i * this.CellSize() - (float)this.gridSize / 2f;
	}

	public bool IsInside(Group group, Vector3 vPos)
	{
		if (((0 != 0 ? true : group.ID == 0) ? true : group.bounds.Contains(vPos)))
		{
			return true;
		}
		return group.bounds.SqrDistance(vPos) < this.switchTolerance;
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Net.sv != null && Net.sv.visibility != null)
		{
			Net.sv.visibility.Dispose();
			Net.sv.visibility = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		float single = this.CellSize();
		float single1 = (float)this.gridSize / 2f;
		Vector3 vector3 = base.transform.position;
		for (int i = 0; i <= this.cellCount; i++)
		{
			float single2 = -single1 + (float)i * single - single / 2f;
			Gizmos.DrawLine(new Vector3(single1, vector3.y, single2), new Vector3(-single1, vector3.y, single2));
			Gizmos.DrawLine(new Vector3(single2, vector3.y, single1), new Vector3(single2, vector3.y, -single1));
		}
	}

	public void OnGroupAdded(Group group)
	{
		group.bounds = this.GetBounds(group.ID);
	}

	private int PositionToGrid(float f)
	{
		f = f + (float)this.gridSize / 2f;
		return Mathf.RoundToInt(f / this.CellSize());
	}
}