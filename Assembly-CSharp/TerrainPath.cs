using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPath : TerrainExtension
{
	public List<PathList> Roads = new List<PathList>();

	public List<PathList> Rivers = new List<PathList>();

	public List<PathList> Powerlines = new List<PathList>();

	public List<MonumentInfo> Monuments = new List<MonumentInfo>();

	public List<RiverInfo> RiverObjs = new List<RiverInfo>();

	public List<LakeInfo> LakeObjs = new List<LakeInfo>();

	internal List<Vector3> OceanPatrolClose = new List<Vector3>();

	internal List<Vector3> OceanPatrolFar = new List<Vector3>();

	private Dictionary<string, List<PowerlineNode>> wires = new Dictionary<string, List<PowerlineNode>>();

	public TerrainPath()
	{
	}

	public void AddWire(PowerlineNode node)
	{
		string str = node.transform.root.name;
		if (!this.wires.ContainsKey(str))
		{
			this.wires.Add(str, new List<PowerlineNode>());
		}
		this.wires[str].Add(node);
	}

	public void Clear()
	{
		this.Roads.Clear();
		this.Rivers.Clear();
		this.Powerlines.Clear();
	}

	private void CreateWire(string name, List<GameObject> objects, Material material)
	{
		if (objects.Count >= 3 && material != null)
		{
			MegaWire megaWire = MegaWire.Create(null, objects, material, "Powerline Wires", null, 1f, 0.1f);
			if (megaWire)
			{
				megaWire.enabled = false;
				megaWire.RunPhysics(megaWire.warmPhysicsTime);
				megaWire.gameObject.SetHierarchyGroup(name, true, false);
			}
		}
	}

	public void CreateWires()
	{
		List<GameObject> gameObjects = new List<GameObject>();
		int count = 0;
		Material wireMaterial = null;
		foreach (KeyValuePair<string, List<PowerlineNode>> wire in this.wires)
		{
			foreach (PowerlineNode value in wire.Value)
			{
				MegaWireConnectionHelper component = value.GetComponent<MegaWireConnectionHelper>();
				if (!component)
				{
					continue;
				}
				if (gameObjects.Count != 0)
				{
					GameObject item = gameObjects[gameObjects.Count - 1];
					if (value.WireMaterial != wireMaterial || component.connections.Count != count || (item.transform.position - value.transform.position).sqrMagnitude > value.MaxDistance * value.MaxDistance)
					{
						this.CreateWire(wire.Key, gameObjects, wireMaterial);
						gameObjects.Clear();
					}
				}
				else
				{
					wireMaterial = value.WireMaterial;
					count = component.connections.Count;
				}
				gameObjects.Add(value.gameObject);
			}
			this.CreateWire(wire.Key, gameObjects, wireMaterial);
			gameObjects.Clear();
		}
	}
}